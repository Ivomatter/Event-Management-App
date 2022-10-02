// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NemeTschek.Data;

namespace NemeTschek.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private ApplicationDbContext _context;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }


        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
            [MaxLength(50)]
            [Display(Name = "FirstName")]
            public string FirstName { get; set; }
            [MaxLength(50)]
            [Display(Name = "LastName")]
            public string LastName { get; set; }
            public List<Team> Teams { get; set; }
            public List<int> TeamsId { get; set; }
            public List<Interest> Interests { get; set; }
            public List<int> InterestsIds { get; set; }


        }

        [Display(Name = "Choose a profile photo.")]
        [DataType(DataType.ImageUrl)]
        public string ProfilePhoto { get; set; }
        [BindProperty]
        public FileUpload fileUpload { get; set; }

        private string uPath = "/uploadimage/";
        public class FileUpload
        {
            
            [Display(Name = "File")]
            public IFormFile FormFile { get; set; }
            public string SuccessMessage { get; set; }
        }
        //public class FileUpload
        //{
        //    [Display(Name = "File")]
        //    public IFormFile FormFile { get; set; }
        //    public string SuccessMessage { get; set; }
        //}



        //upload action as shown:
        //private string fullPath = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "UploadImages";
        public async Task<IActionResult> OnPostUploadAsync(FileUpload fileUpload)
        {
            //Creating upload folder
            if (!Directory.Exists(uPath))
            {
                Directory.CreateDirectory(uPath);
            }

            var formFile = fileUpload.FormFile;
            if (formFile.Length > 0)
            {
                var fName = formFile.FileName;
                fName = _userManager.GetUserId(User) + Path.GetExtension(fName);

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), ("wwwroot/uploadimage/" + fName ));

                using (var stream = System.IO.File.Create(filePath))
                {
                    formFile.CopyToAsync(stream);
                }
                var currentUser = await _userManager.GetUserAsync(User);

                currentUser.PhotoDirectory = "/uploadimage/" + fName;

                await _userManager.UpdateAsync(currentUser);
            }

            // Process uploaded files
            // Don't rely on or trust the FileName property without validation.
            ViewData["SuccessMessage"] = formFile.FileName.ToString() + " files uploaded!!";
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            await LoadAsync(user);
            return Page();
        }


        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            Username = userName;
            var teams = _context.Team.ToList();
            List<Team> selectedTeams = new List<Team>();
            foreach (Team team in teams)
            {
                if (_context.UsersTeams.FirstOrDefault(t => t.UserId == user.Id && t.TeamId == team.Id) != null)
                {
                    selectedTeams.Add(team);
                }

            }
            ViewData["participatingTeams"] = selectedTeams;

            //var profilePhoto = await _userManager.Get;

            var interests = _context.Interests.ToList();
            List<Interest> selectedInterests = new List<Interest>();
            foreach (Interest interest in interests)
            {
                if (_context.UsersInterests.FirstOrDefault(t => t.UserId == user.Id && t.InterestId == interest.Id) != null)
                {
                    selectedInterests.Add(interest);
                }

            }
            ViewData["UserInterests"] = selectedInterests;



            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Teams = teams,
                TeamsId = teams.Select(t => t.Id).ToList(),
                Interests = interests,
                InterestsIds = interests.Select(t => t.Id).ToList()
            };

            //if(ProfilePhoto != null)
            //{
            //    string folder = "ProfilePictures";

            //}
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }


            if (Input.PhoneNumber != user.PhoneNumber)
            {
                user.PhoneNumber = Input.PhoneNumber;
            }
            if (Input.FirstName != user.FirstName)
            {
                user.FirstName = Input.FirstName;
            }
            if (Input.LastName != user.LastName)
            {
                user.LastName = Input.LastName;
            }


            var selectedTeamsIds = Input.TeamsId;
            DeleteTeamsUserContections(user);
            foreach (int teamIds in selectedTeamsIds)
            {
                UsersTeams usersTeams = new UsersTeams();
                usersTeams.UserId = user.Id;
                usersTeams.TeamId = teamIds;
                _context.UsersTeams.Add(usersTeams);
            }

            var selectedInterestIds = Input.InterestsIds;
            DeleteInterestsUserContections(user);
            foreach (int interestId in selectedInterestIds)
            {
                UsersInterests userInterest = new UsersInterests();
                userInterest.UserId = user.Id;
                userInterest.InterestId = interestId;
                _context.UsersInterests.Add(userInterest);
            }

            var updatingResult = await _userManager.UpdateAsync(user);
            if (!updatingResult.Succeeded)
            {
                StatusMessage = "Unexpected error when trying to set your information.";
                return RedirectToPage();
            }
            var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
            if (!setPhoneResult.Succeeded)
            {
                StatusMessage = "Unexpected error when trying to set phone number.";
                return RedirectToPage();
            }
            //if (Input.ProfilePhoto != profilePicture)
            //{
            //    var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
            //    if (!setPhoneResult.Succeeded)
            //    {
            //        StatusMessage = "Unexpected error when trying to set phone number.";
            //        return RedirectToPage();
            //    }
            //}

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }

        private void DeleteInterestsUserContections(ApplicationUser user)
        {
            var userInterest = _context.UsersInterests;
            foreach (var us in userInterest)
            {
                if (us.UserId == user.Id)
                {
                    _context.UsersInterests.Remove(us);
                }
            }
            _context.SaveChanges();
        }

        private void DeleteTeamsUserContections(ApplicationUser user)
        {
            var userTeams = _context.UsersTeams;
            foreach (var us in userTeams)
            {
                if (us.UserId == user.Id)
                {
                    _context.UsersTeams.Remove(us);
                }
            }
            _context.SaveChanges();
        }



    }
}
