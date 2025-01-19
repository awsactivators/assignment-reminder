// using MailKit.Net.Smtp;
// using MimeKit;
// using Microsoft.EntityFrameworkCore;
// using AssignmentReminder.Data;


// namespace AssignmentReminder.Services
// {
//     public class EmailService
//     {
//         private readonly IConfiguration _configuration;
//         private readonly ApplicationDbContext _context;

//         public EmailService(IConfiguration configuration, ApplicationDbContext context)
//         {
//             _configuration = configuration;
//             _context = context;
//         }

//         public async Task SendEmailAsync(string recipientEmail, string subject, string messageBody)
//         {
//             var emailSettings = _configuration.GetSection("EmailSettings");
//             var email = new MimeMessage();
//             email.From.Add(new MailboxAddress(emailSettings["SenderName"], emailSettings["SenderEmail"]));
//             email.To.Add(MailboxAddress.Parse(recipientEmail));
//             email.Subject = subject;
//             email.Body = new TextPart("html") { Text = messageBody };

//             using var smtp = new SmtpClient();
//             await smtp.ConnectAsync(emailSettings["SmtpServer"], int.Parse(emailSettings["SmtpPort"]), MailKit.Security.SecureSocketOptions.StartTls);
//             await smtp.AuthenticateAsync(emailSettings["Username"], emailSettings["Password"]);
//             await smtp.SendAsync(email);
//             await smtp.DisconnectAsync(true);
//         }

//         public async Task SendDueSoonReminders()
//         {
//             var now = DateTime.Now;

//             var assignments = await _context.Assignments
//                 .Where(a => a.DueDate >= now && a.DueDate <= now.AddDays(1) && !a.IsCompleted)
//                 .ToListAsync();

//             foreach (var assignment in assignments)
//             {
//                 var userEmail = assignment.UserId; // Assuming UserId corresponds to email
//                 var subject = $"Reminder: Assignment {assignment.CourseCode} is Due Soon!";
//                 var body = $@"
//                     <p>Hi,</p>
//                     <p>This is a reminder for your assignment <strong>{assignment.CourseName}</strong>, 
//                     which is due on <strong>{assignment.DueDate:g}</strong>.</p>
//                     <p>Regards,<br>Assignment Reminder</p>
//                 ";
//                 await SendEmailAsync(userEmail, subject, body);
//             }
//         }

//     }
// }


using AssignmentReminder.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;


namespace AssignmentReminder.Services
{
    public class EmailService
    {
        private readonly ApplicationDbContext _context;
        private readonly PostmarkEmailService _postmarkEmailService;
        private readonly UserManager<IdentityUser> _userManager;

        public EmailService(ApplicationDbContext context, PostmarkEmailService postmarkEmailService, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _postmarkEmailService = postmarkEmailService;
            _userManager = userManager;
        }

        public async Task SendDueSoonReminders()
        {
            var now = DateTime.Now;
            var assignments = await _context.Assignments
                .Where(a => a.DueDate >= now && a.DueDate <= now.AddDays(1) && !a.IsCompleted)
                .ToListAsync();

            foreach (var assignment in assignments)
            {
              // Get the user's email address using the UserManager
              var user = await _userManager.FindByIdAsync(assignment.UserId);
              if (user == null || string.IsNullOrEmpty(user.Email))
              {
                  Console.WriteLine($"User not found or email is missing for UserId: {assignment.UserId}");
                  continue;
              }

              var userEmail = user.Email;

              var subject = $"Reminder: Assignment {assignment.CourseCode} is Due Soon!";
              var body = $@"
                  <p>Hi,</p>
                  <p>This is a friendly reminder that your assignment <strong>{assignment.CourseName}</strong> for 
                  course <strong>{assignment.CourseCode}</strong> is due on <strong>{assignment.DueDate:g}</strong>.</p>
                  <p>Please ensure to complete it on time.</p>
                  <p>Best regards,<br>Assignment Reminder Team</p>
              ";

              var success = await _postmarkEmailService.SendEmailAsync(userEmail, subject, body);
              if (success)
              {
                  Console.WriteLine($"Email sent to {userEmail} for assignment {assignment.CourseCode}.");
              }
              else
              {
                  Console.WriteLine($"Failed to send email to {userEmail} for assignment {assignment.CourseCode}.");
              }
          }
        }
    }
}
