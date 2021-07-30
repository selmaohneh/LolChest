using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.SimpleEmailV2;
using Amazon.SimpleEmailV2.Model;

namespace LolChest.Lambda.Core
{
    public class EmailSender
    {
        private readonly AmazonSimpleEmailServiceV2Client _sesClient;

        public EmailSender(AmazonSimpleEmailServiceV2Client sesClient)
        {
            _sesClient = sesClient;
        }

        public async Task SendSummaryAsEmail(string subject, string text, List<string> toAddresses)
        {
            var request = new SendEmailRequest
            {
                FromEmailAddress = "lolchest@neumann.sh",
                Destination = new Destination
                {
                    ToAddresses = toAddresses
                },
                Content = new EmailContent
                {
                    Simple = new Message
                    {
                        Body = new Body
                        {
                            Text = new Content
                            {
                                Data = text
                            }
                        },
                        Subject = new Content
                        {
                            Data = subject
                        }
                    }
                }
            };

            await _sesClient.SendEmailAsync(request);
        }
    }
}