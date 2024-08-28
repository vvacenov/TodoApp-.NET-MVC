using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TodoApp.Models;


namespace TodoApp.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var errMessage = TempData["ErrMessage"] as string ?? ViewData["ErrMessage"] as string;
            var errDetails = TempData["ErrDetails"] as string;
            var viewModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                ErrorMessage = errMessage,
                ErrorDetails = errDetails,

            };
            return View(viewModel);
        }

        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult NotFoundPage()
        {
            return View();
        }
    }
}
