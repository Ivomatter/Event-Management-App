using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using NemeTschek.Data;
using Microsoft.AspNetCore.Authorization;
using System.Reflection.Metadata.Ecma335;

namespace NemeTschek.Controllers
{
    [Authorize]
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public int returnInDays()
        {

            return 0;
        }

        public EventsController(ApplicationDbContext context, 
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }


        // GET api/role
        public string findRole(string userId)
        {
            foreach(var query in _context.UserRoles)
            {
                if(query.UserId.ToString() == userId)
                {
                    return query.RoleId.ToString();
                }
            }

            return "RoleNotFound";
        }

        //Helper function to check whether a user is enrolled or not
        public bool isEnrolled(int? eventId, string userId)
        {
            foreach(var e in _context.UsersEvents)
            {
                if(e.UserId == userId && e.EventId == eventId)
                {
                    return true;
                }
            }
            return false;
        }

        //Helper function that returns a list of participants in an event through eventId
        public List<string>? participantsInEvent(int? eventId)
        {
            List<string> result = new List<string>();
            foreach(var user in _context.Users)
            {
                if(isEnrolled(eventId,user.Id))
                {
                    string name = user.FirstName + " " + user.LastName;
                    result.Add(name);
                }
            }
            if(result.Count == 0)
            {
                return null;
            }
            return result;
        }


        // GET: Events
        public async Task<IActionResult> Index(string searchString)
        {

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.currentRole = findRole(currentUserId);
            // ADMIN == c589321f-e40a-472d-aa5b-bb8e87e8c463
            // EMPLOYEE == 2c633b18-bc64-4eb7-9735-07c68e1162dd


            var events = _context.Event.ToList();
            var eventsResult = new List<Event>();
            if (!string.IsNullOrEmpty(searchString))
            {
                foreach (var ev in events)
                {
                    if (ev.Name.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >=0 
                        || ev.Location.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0 
                        || ev.Description.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        if(ev.EndDate > DateTime.Now)
                            eventsResult.Add(ev);
                    }
                }
            }
            else
            {
                foreach (var ev in events)
                {
                    if (ev.EndDate > DateTime.Now)
                        eventsResult.Add(ev);
                }
                //eventsResult = events;
            }
            
            //Alphabetical sort
            eventsResult = eventsResult.OrderBy(x => x.Name).ToList();

            Dictionary<int, bool> enrollList = new Dictionary<int, bool>();
            foreach (var e in eventsResult)
            {
                enrollList.Add(e.EventId, isEnrolled(e.EventId, currentUserId));
            }

            ViewData["enrolledEvents"] = enrollList;

            return View(eventsResult);
        }

        //GET : Events/PastEvents
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> PastEvents(string searchString)
        {
            var events = _context.Event.ToList();
            var eventsResult = new List<Event>();
            if (!string.IsNullOrEmpty(searchString))
            {
                foreach (var ev in events)
                {
                    if (ev.Name.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0
                        || ev.Location.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0
                        || ev.Description.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        if (ev.EndDate < DateTime.Now)
                            eventsResult.Add(ev);
                    }
                }
            }
            else
            {
                foreach (var ev in events)
                {
                    if (ev.EndDate < DateTime.Now)
                        eventsResult.Add(ev);
                }
            }
            eventsResult = eventsResult.OrderBy(x => x.Name).ToList();
            return View(eventsResult);
        }


        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Set<Event>() == null)
            {
                return NotFound();
            }

            var @event = await _context.Set<Event>()
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (@event == null)
            {
                return NotFound();
            }

            bool registeredUser = false;
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            ViewBag.currentRole = findRole(currentUserId);
            // ADMIN == c589321f-e40a-472d-aa5b-bb8e87e8c463
            // EMPLOYEE == 2c633b18-bc64-4eb7-9735-07c68e1162dd
            
            foreach (var entry in _context.UsersEvents)
            {
                if(entry.UserId == currentUserId && entry.EventId == id)
                {
                    registeredUser = true;
                    break;
                }
            }

            ViewBag.isUserRegistered = registeredUser;
            ViewData["enrolledUsers"] = participantsInEvent(id);
            bool past = @event.StartDate < DateTime.Now;
            ViewBag.past = past;

            return View(@event);
        }

        // GET: Events/Create

        [Authorize(Roles = "Administrator")]
        public IActionResult Create(string validationError = "")
        {
            //validationError gets error message string from the POST: Events/Create method
            ViewBag.Message = validationError;
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([Bind("EventId,Name,Description,StartDate,EndDate,Location,MinPeople,MaxPeople")] Event @event)
        {

            //Scuffed validation of whether the input data is correct by use of comparison operators
            //and returns a corresponding error message to the user

            if(@event.StartDate > @event.EndDate)
                return RedirectToAction(nameof(Create), new { validationError = "End date cannot be earlier than the start date." });
            if(@event.MinPeople > @event.MaxPeople)
                return RedirectToAction(nameof(Create), new { validationError = "Maximum amount of people cannot be lower than the minimum." });
            if(@event.MinPeople == 0)
                return RedirectToAction(nameof(Create), new { validationError = "Minimum amount of people cannot be lower than 1." });
            if (@event.MinPeople <= 1)
                return RedirectToAction(nameof(Create), new { validationError = "Minimum amount of people cannot be lower than 1." });


            if (ModelState.IsValid)
            {
                _context.Add(@event);
                await _context.SaveChangesAsync();
                return Redirect(nameof(Details) + "/" + @event.EventId);
            }
            return View(@event);
        }

        // GET: Events/Edit/5


        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Set<Event>() == null)
            {
                return NotFound();
            }

            var @event = await _context.Set<Event>().FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id, [Bind("EventId,Name,Description,StartDate,EndDate,Location,MinPeople,MaxPeople")] Event @event)
        {
            if (id != @event.EventId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.EventId))
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
            return View(@event);
        }

        // GET: Events/Delete/5
        
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Set<Event>() == null)
            {
                return NotFound();
            }

            var @event = await _context.Set<Event>()
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Set<Event>() == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Set<Event>()'  is null.");
            }
            var @event = await _context.Set<Event>().FindAsync(id);
            if (@event != null)
            {
                _context.Set<Event>().Remove(@event);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Enroll(int id)
        {
            if(!EventExists(id))
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEvent = new UsersEvents();
            userEvent.EventId = id;
            userEvent.UserId = userId;
            
            _context.Add(userEvent);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Events", new { id });

        }

       public async Task<IActionResult> UnEnroll(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var @userEvent = await _context.Set<UsersEvents>().FindAsync(userId, id);
            if (@userEvent != null)
            {
                _context.Set<UsersEvents>().Remove(@userEvent);
            }
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Events", new { id });
        }

        private bool EventExists(int id)
        {
            return _context.Set<Event>().Any(e => e.EventId == id);
        }
    }
}
