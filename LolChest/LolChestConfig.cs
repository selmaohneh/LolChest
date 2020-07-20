using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using RiotSharp.Misc;

namespace LolChest
{
    /// <summary>
    /// This class wraps the settings that are stored inside
    /// the settings.json and even sets some more.
    /// </summary>
    public class LolChestConfig
    {
        public IEnumerable<string> SummonerNames
        {
            get
            {
                var json = Environment.GetEnvironmentVariable("SummonerNamesJson");
                var summonerNames = JsonConvert.DeserializeObject<IEnumerable<string>>(json);

                return summonerNames;
            }
        }

        public Region Region => Enum.Parse<Region>(Environment.GetEnvironmentVariable("Region"));
        public string ApiKey => Environment.GetEnvironmentVariable("ApiKey");

        public string GooglePassword => Environment.GetEnvironmentVariable("GooglePassword");
        public string SenderAddress => Environment.GetEnvironmentVariable("SenderAddress");

        public IEnumerable<string> ReceiverAddresses
        {
            get
            {
                var json = Environment.GetEnvironmentVariable("ReceiverAddresses");

                var addresses = JsonConvert.DeserializeObject<IEnumerable<string>>(json);
                return addresses;
            }
        }
    }
}