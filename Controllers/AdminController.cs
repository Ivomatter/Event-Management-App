using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NemeTschek.Data;
using NemeTschek.Helpers;
using Spire.Xls;

namespace NemeTschek.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IExport _exportHelper;
        public AdminController(ILogger<HomeController> logger,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IExport exportHelper)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
            _exportHelper = exportHelper;
        }
        public async Task<FileResult> GetImageAsync(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            byte[] imageByteData = System.IO.File.ReadAllBytes(user.PhotoDirectory);
            return File(imageByteData, "image/png");

        }


        // GET: AdminController
        public async Task<IActionResult> Index(string searchString)
        {
            var users = _context.Users.ToList();
            var usersResult = new List<ApplicationUser>();
            if (!string.IsNullOrEmpty(searchString))
            {
                foreach (var ev in users)
                {
                    if (ev.FirstName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0
                        || ev.LastName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0
                        || ev.Email.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        usersResult.Add(ev);
                    }
                }
            }
            else
            {
                usersResult = users;
            }
            usersResult = usersResult.OrderBy(x => x.FirstName).ThenBy(y => y.LastName).ToList();
            return View(usersResult);
        }

        [Authorize]
        public ActionResult Export(string id)
        {
            Workbook wbToStream = new Workbook();
            Worksheet sheet = wbToStream.Worksheets[0];
            ApplicationUser user = _context.Users.FirstOrDefault(u => u.Id == id);
            long index = 0;
            _exportHelper.ExportSingleUser(sheet, user, ref index);

            sheet.AllocatedRange.AutoFitColumns();
            FileStream file_stream = new FileStream($"{user.FirstName} {user.LastName}.xls", FileMode.OpenOrCreate);
            wbToStream.SaveToStream(file_stream);
            file_stream.Close();
            return RedirectToAction("Index");
        }

        // GET: AdminController/Details/5
        public async Task<ActionResult> Details(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            return View(user);
        }

        // GET: AdminController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdminController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AdminController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminController/Delete/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AdminController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}