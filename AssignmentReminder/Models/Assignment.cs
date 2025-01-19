using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AssignmentReminder.Models;

public class Assignment
{
    public int Id { get; set; }

    [Required]
    public string CourseCode { get; set; }

    [Required]
    public string CourseName { get; set; }

    [Required]
    public int WeekNumber { get; set; }

    [Required]
    public string Type { get; set; } // Lab, Project, or Assignment

    [Required]
    public DateTime DueDate { get; set; }

    public bool IsCompleted { get; set; } = false;

    // user association
    [BindNever]
    public string? UserId { get; set; }

    public double? Grade { get; set; }
}
