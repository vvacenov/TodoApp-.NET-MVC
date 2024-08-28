using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TodoApp.Data;
using TodoApp.Models.Auth;
using TodoApp.Models.Todos;
using TodoApp.Services.Auth;

namespace TodoApp.Controllers
{
    /// <summary>
    /// Controller responsible for managing todo operations.
    /// Requires authorization for all actions.
    /// </summary>
    [Authorize]

 
public class TodosController : Controller
    {
        private readonly TodoDbContext _context;
        private readonly UserManager<UserAccountModel> _userManager;

        /// <summary>
        /// Constructor for TodosController.
        /// </summary>
        public TodosController(TodoDbContext context, UserManager<UserAccountModel> userManager)
        {
            _context = context;
            _userManager = userManager;
        }



        /// <summary>
        /// Displays the list of todos, with optional filtering.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index(string? statusId = null, string? due = null)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return Challenge();  
            }
            var filter = new FilterViewModel($"{due ?? "all"}-{statusId ?? "all"}");
            var userId = await User.GetUser(_userManager);

            if (!userId.HasValue)
            {
                return RedirectToAction("Index", "Home");
            }
            var userData = User;
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
        
            var idusr = userIdClaim?.Value;
            

            // Build the query based on filters
            IQueryable<Todo> query = _context.Todos
                .Include(todo => todo.Status)
                .Where(todo => todo.OwnerId == userId);

            if (filter.HasStatus)
            {
                query = query.Where(todo => todo.StatusId == filter.StatusId);
            }

            if (filter.HasDue)
            {
                var today = DateTime.Today;
                if (filter.isToday)
                {
                    query = query.Where(todo => todo.DueDate.HasValue && todo.DueDate.Value.Date == today);
                }
                else if (filter.isPrevious)
                {
                    query = query.Where(todo => todo.DueDate.HasValue && todo.DueDate.Value.Date < today);
                }
                else if (filter.isUpcoming)
                {
                    query = query.Where(todo => todo.DueDate.HasValue && todo.DueDate.Value.Date > today);
                }
            }

            try
            {
                var todosResult = await query.OrderByDescending(todo => todo.CreatedAt).ToListAsync();
                ViewBag.Statuses = await _context.Statuses.ToListAsync() ?? new List<Status>();
                ViewBag.DueFilters = FilterViewModel.DueFilterValues;
                ViewBag.CurrentFilter = filter;
                return View(todosResult);
            }
            catch (Exception ex)
            {
                TempData["ErrMessage"] = "An unexpected error occurred while fetching todos. Please try again later.";
                TempData["ErrDetails"] = ex.Message.Trim();
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Handles filtering of todos.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Filter(string statusFilter, string dueFilter)
        {
            return RedirectToAction("Index", new { statusId = statusFilter, due = dueFilter });
        }

        /// <summary>
        /// Deletes all finished todos.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFinished(string id)
        {
            try
            {
                var delete = _context.Todos.Where(todo => todo.StatusId == "finished").ToList();
                foreach (var todo in delete)
                {
                    _context.Todos.Remove(todo);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrMessage"] = "An unexpected error occurred while deleting finished todos. Please try again later.";
                TempData["ErrDetails"] = ex.Message.Trim();
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Displays the form to add a new todo.
        /// </summary>
        [HttpGet]
        public IActionResult Add()
        {
            return View(new Todo { StatusId = "new" });
        }

        /// <summary>
        /// Handles the addition of a new todo.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Todo todo)
        {
            if (ModelState.IsValid)
            {
                var today = DateTime.Today;

                // Validate that the due date is not in the past
                if (todo.DueDate.HasValue && todo.DueDate.Value < today)
                {
                    ModelState.AddModelError("DueDate", "Sorry, can't select a due date in the past.");
                    return View(todo);
                }

                try
                {
                    var user = await User.GetUser(_userManager);
                    if (user == null)
                    {
                        return NotFound("User not found.");
                    }

                    todo.OwnerId = user.Value;
                    todo.StatusId = "new";

                    _context.Add(todo);

                    //finally save the todo in db
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["ErrMessage"] = "An unexpected error occurred while adding the todo. Please try again later.";
                    TempData["ErrDetails"] = ex.Message.Trim();
                    return RedirectToAction("Error", "Home");
                }
            }

            return View(todo);
        }

        /// <summary>
        /// Marks a todo as started.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkStarted([FromQuery] string id, Todo selected)
        {
            try
            {
                selected = await _context.Todos.FindAsync(selected.Id);
                if (selected != null)
                {
                    _context.Attach(selected);
                    selected.StatusId = "started";
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("Index", new { ID = id });
            }
            catch (Exception ex)
            {
                TempData["ErrMessage"] = "An unexpected error occurred while marking the todo as started. Please try again later.";
                TempData["ErrDetails"] = ex.Message.Trim();
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Marks a todo as finished.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkFinished([FromQuery] string id, Todo selected)
        {
            try
            {
                selected = await _context.Todos.FindAsync(selected.Id);
                if (selected != null)
                {
                    _context.Attach(selected);
                    selected.StatusId = "finished";
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("Index", new { ID = id });
            }
            catch (Exception ex)
            {
                TempData["ErrMessage"] = "An unexpected error occurred while marking the todo as finished. Please try again later.";
                TempData["ErrDetails"] = ex.Message.Trim();
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Deletes a single todo.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSingle([FromRoute] string id)
        {
            try
            {
                var todoToDelete = await _context.Todos.FindAsync(long.Parse(id));
                if (todoToDelete != null)
                {
                    _context.Todos.Attach(todoToDelete);
                    _context.Todos.Remove(todoToDelete);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("Index", new { id });
            }
            catch (Exception ex)
            {
                TempData["ErrMessage"] = "An unexpected error occurred while deleting the todo. Please try again later.";
                TempData["ErrDetails"] = ex.Message.Trim();
                return RedirectToAction("Error", "Home");
            }
        }
    }
}