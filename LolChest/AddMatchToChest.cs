using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using MimeKit;
using MimeKit.Text;
using Newtonsoft.Json;
using RiotSharp.Endpoints.Interfaces;
using RiotSharp.Endpoints.MatchEndpoint;
using RiotSharp.Misc;

namespace LolChest
{
    public class AddMatchToChest
    {
        private readonly IMatchEndpoint _matchEndpoint;
        private readonly ISmtpClient _smtpClient;
        private readonly CloudTables _cloudTables;
        private readonly LolChestConfig _lolChestConfig;

        public AddMatchToChest(IMatchEndpoint matchEndpoint, ISmtpClient smtpClient, CloudTables cloudTables,
            LolChestConfig lolChestConfig)
        {
            _matchEndpoint = matchEndpoint;
            _smtpClient = smtpClient;
            _cloudTables = cloudTables;
            _lolChestConfig = lolChestConfig;
        }

        [FunctionName("AddMatchToChest")]
        public async Task Run([ActivityTrigger] (Region region, string gameId) input, ILogger log)
        {
            var match = await _matchEndpoint.GetMatchAsync(input.region, long.Parse(input.gameId));
            log.LogInformation($"Retrieved match with id {match.GameId}");

            var matchEntity = new MatchEntity
            {
                PartitionKey = match.GameId.ToString(),
                RowKey = match.GameId.ToString(),
                GameCreation = match.GameCreation,
                MatchJson = JsonConvert.SerializeObject(match)
            };

            var registeredMatchesTable = await _cloudTables.Get("registeredmatches");
            var insertOperation = TableOperation.Insert(matchEntity);
            log.LogInformation($"Inserting match {match.GameId} in table.");
            await registeredMatchesTable.ExecuteAsync(insertOperation);
            log.LogInformation($"Inserting match {match.GameId} succeeded!");
            log.LogInformation("Sending match via email.");
            await SendMatchAsEmail(match);
            log.LogInformation("Sending match via email succeeded!");
        }

        private async Task SendMatchAsEmail(Match match)
        {
            var lolChestEntry = new LolChestEntry(match, _lolChestConfig.SummonerNames);

            var mail = new MimeMessage
            {
                Sender = new MailboxAddress("LolChest", "lolchestlol@gmail.com"),
                Subject = "New LolChest entry registered!",
                Body = new TextPart(TextFormat.Plain) {Text = ObjectDumper.Dump(lolChestEntry)},
            };

            foreach (var receiverAddress in _lolChestConfig.ReceiverAddresses)
            {
                mail.To.Add(new MailboxAddress(receiverAddress, receiverAddress));
            }

            _smtpClient.ServerCertificateValidationCallback = (s, c, h, e) => true;

            await _smtpClient.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await _smtpClient.AuthenticateAsync("lolchestlol", _lolChestConfig.GooglePassword);
            await _smtpClient.SendAsync(mail);
            await _smtpClient.DisconnectAsync(true);
        }
    }
}