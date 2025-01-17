using System.ComponentModel.DataAnnotations;
using AssignmentReminder.Models;

namespace AssignmentReminder.Models;

public class Reminder
{
    public int Id { get; set; }

    [Required]
    public int AssignmentId { get; set; } // Foreign Key

    [Required]
    public DateTime ReminderTime { get; set; }

    public bool IsSent { get; set; } = false;

    // Navigation property
    public Assignment Assignment { get; set; }
}
