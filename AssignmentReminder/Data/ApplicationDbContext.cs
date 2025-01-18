using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AssignmentReminder.Models;

namespace AssignmentReminder.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSet for Assignments, Reminders, Notifications
     public DbSet<Assignment> Assignments { get; set; }

     public DbSet<Reminder> Reminders { get; set; }

     public DbSet<Notification> Notifications { get; set; }


}
