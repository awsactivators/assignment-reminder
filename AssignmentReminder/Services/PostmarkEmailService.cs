using Microsoft.Extensions.Configuration;
using PostmarkDotNet;

namespace AssignmentReminder.Services
{
    public class PostmarkEmailService
    {
        private readonly string _apiToken;
        private readonly string _senderEmail;

        public PostmarkEmailService(IConfiguration configuration)
        {
            _apiToken = configuration["Postmark:ApiToken"];
            _senderEmail = configuration["Postmark:SenderEmail"];
        }

        public async Task<bool> SendEmailAsync(string recipient, string subject, string textBody, string htmlBody = null)
        {
            var client = new PostmarkClient(_apiToken);

            var message = new PostmarkMessage
            {
                From = _senderEmail,
                To = recipient,
                Subject = subject,
                TextBody = textBody,
                HtmlBody = htmlBody
            };

            var response = await client.SendMessageAsync(message);

            return response.Status == PostmarkStatus.Success;
        }
    }
}
