using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NemeTschek.Data;
using NemeTschek.Helpers;
using NemeTschek.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace NemeTschek.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IExport _exportHelper;
        public HomeController(ILogger<HomeController> logger,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IExport exportHelper)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
            _exportHelper = exportHelper;
        }

        public string findRole(string userId)
        {
            foreach (var query in _context.UserRoles)
            {
                if (query.UserId.ToString() == userId)
                {
                    return query.RoleId.ToString();
                }
            }

            return "RoleNotFound";
        }
        public async Task<IActionResult> Index()
        {
            
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.currentRole = findRole(currentUserId);

            var events = _context.Event.ToList();

            events.Reverse();
            
            return View(events);
        }
        public async Task<IActionResult> Export()
        {
            var usr = await _userManager.GetUserAsync(User);
            if (usr != null)
            {
                ApplicationUser user = await _userManager.FindByEmailAsync(usr.Email);
                _exportHelper.ExportAllUsers(user);
            }
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}