using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AssignmentReminder.Data;
using AssignmentReminder.Models;
using AssignmentReminder.Services;


[Authorize]
public class AssignmentsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly NotificationService _notificationService;
    private readonly PostmarkEmailService _emailService;

    public AssignmentsController(ApplicationDbContext context, UserManager<IdentityUser> userManager, NotificationService notificationService, PostmarkEmailService emailService)
    {
        _context = context;
        _userManager = userManager;
        _notificationService = notificationService;
        _emailService = emailService;
    }

    public async Task<IActionResult> TestPostmarkEmail()
    {
        var success = await _emailService.SendEmailAsync(
            "n01613636@humber.ca",
            "Test Email from Postmark",
            "This is a test email using Postmark!"
        );

        return Ok(success ? "Email sent successfully!" : "Failed to send email.");
    }

    // GET: Assignments
    public async Task<IActionResult> Index()
    {
      var userId = _userManager.GetUserId(User);  // Get the logged-in user's ID
      var userEmail = User.Identity.Name;
      var now = DateTime.Now;

      // Fetch all assignments for the logged-in user
      var assignments = await _context.Assignments
        .Where(a => a.UserId == userId) 
        .ToListAsync();

      // Categorize assignments
      var overdue = assignments.Where(a => a.DueDate < now && !a.IsCompleted).ToList();
      var dueSoon = assignments.Where(a => a.DueDate >= now && a.DueDate <= now.AddDays(1) && !a.IsCompleted).OrderBy(a => a.DueDate).ToList();
      var completed = assignments.Where(a => a.IsCompleted).ToList();
      var mainAssignments = assignments
        .Where(a => a.DueDate >= now && !a.IsCompleted) // Exclude overdue and completed
        .Except(overdue.Concat(completed))
        .ToList();

      // Pass categorized assignments to the view
      ViewData["Overdue"] = overdue;
      ViewData["DueSoon"] = dueSoon;
      ViewData["Completed"] = completed;

      // Fetch notifications
      var notifications = await _notificationService.GetNotificationsAsync(userId);
      ViewData["Notifications"] = notifications;

      return View(mainAssignments); // Pass the filtered list to the view
  }

    // GET: Assignments/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);
        var assignment = await _context.Assignments
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

        if (assignment == null)
        {
            return NotFound();
        }

        return View(assignment);
    }

    // GET: Assignments/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Assignments/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("CourseCode,CourseName,WeekNumber,Type,DueDate,IsCompleted")] Assignment assignment)
    {
      Console.WriteLine("Entered the Create action.");

      // Set the UserId programmatically
      assignment.UserId = _userManager.GetUserId(User); 

      // Manually clear any validation errors for UserId
      ModelState.Remove(nameof(assignment.UserId));

      if (ModelState.IsValid)
      {
          Console.WriteLine("ModelState is valid.");

          // Add the assignment to the database
          _context.Add(assignment);
          await _context.SaveChangesAsync();

          // Set success message
          TempData["SuccessMessage"] = "Assignment created successfully!";

          return RedirectToAction(nameof(Index));
      }

      // Log validation errors if ModelState is invalid
      Console.WriteLine("ModelState is invalid.");
      foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
      {
        Console.WriteLine($"Validation Error: {error.ErrorMessage}");
      }

      // If ModelState is invalid, redisplay the form
      return View(assignment);
    }

    // GET: Assignments/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);
        var assignment = await _context.Assignments
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

        if (assignment == null)
        {
            return NotFound();
        }

        return View(assignment);
    }

    // POST: Assignments/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,CourseCode,CourseName,WeekNumber,Type,Grade,DueDate,IsCompleted")] Assignment assignment)
    {
        if (id != assignment.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                // Ensure the UserId is retained
                assignment.UserId = _userManager.GetUserId(User);

                _context.Update(assignment);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Assignment updated successfully!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AssignmentExists(assignment.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(assignment);
    }

    // GET: Assignments/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);
        var assignment = await _context.Assignments
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

        if (assignment == null)
        {
            return NotFound();
        }

        return View(assignment);
    }

    // POST: Assignments/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = _userManager.GetUserId(User);
        var assignment = await _context.Assignments
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

        if (assignment != null)
        {
            _context.Assignments.Remove(assignment);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool AssignmentExists(int id)
    {
        return _context.Assignments.Any(e => e.Id == id);
    }


}
