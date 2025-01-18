using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.EntityFrameworkCore;
using AssignmentReminder.Data;


namespace AssignmentReminder.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public EmailService(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task SendEmailAsync(string recipientEmail, string subject, string messageBody)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(emailSettings["SenderName"], emailSettings["SenderEmail"]));
            email.To.Add(MailboxAddress.Parse(recipientEmail));
            email.Subject = subject;
            email.Body = new TextPart("html") { Text = messageBody };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(emailSettings["SmtpServer"], int.Parse(emailSettings["SmtpPort"]), MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(emailSettings["Username"], emailSettings["Password"]);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        public async Task SendDueSoonReminders()
        {
            var now = DateTime.Now;

            var assignments = await _context.Assignments
                .Where(a => a.DueDate >= now && a.DueDate <= now.AddDays(1) && !a.IsCompleted)
                .ToListAsync();

            foreach (var assignment in assignments)
            {
                var userEmail = assignment.UserId; // Assuming UserId corresponds to email
                var subject = $"Reminder: Assignment {assignment.CourseCode} is Due Soon!";
                var body = $@"
                    <p>Hi,</p>
                    <p>This is a reminder for your assignment <strong>{assignment.CourseName}</strong>, 
                    which is due on <strong>{assignment.DueDate:g}</strong>.</p>
                    <p>Regards,<br>Assignment Reminder</p>
                ";
                await SendEmailAsync(userEmail, subject, body);
            }
        }

    }
}
