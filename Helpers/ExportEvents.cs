


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NemeTschek.Data;
using Spire.Xls;
using System.Data.Entity;
using System.Drawing;
using System.Security.Claims;

namespace NemeTschek.Helpers
{

    public interface IExport
    {
        void ExportSingleUser(Worksheet sheet, ApplicationUser user, ref long index);
        void ExportAllUsers(ApplicationUser user);
    }

    public class ExportEvents : IExport
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ExportEvents(ApplicationDbContext context)
        {
            _context = context;
        }

        //public Worksheet MakeTableEventsExport(Worksheet sheet, int index = 0)
        //{
        //    sheet.Range[$"A{++index}"].Text = "Full name:";
        //    sheet.Range[$"A{++index}"].Text = "Email:";
        //    sheet.Range[$"A{++index}"].Text = "Phone:";
        //    sheet.Range[$"A{++index}"].Text = "Teams:";
        //    sheet.Range[$"A{++index}"].Text = "Interests:";
        //    sheet.Range[$"A{++index}"].Text = "Events:";
        //    Color color = Color.FromArgb(255, 237, 118, 76);
        //    sheet.Range["A1:A6"].Style.Color = color;
        //    return sheet;
        //}
        [Authorize]
        public void ExportSingleUserFromAdmin()
        {

        }

        [Authorize]
        public void ExportSingleUser(Worksheet sheet, ApplicationUser user, ref long index)
        {
            Color color = Color.FromArgb(248, 203, 173);

            sheet.Range[$"A{index+1}:A{index + 6}"].Style.Color = color;
            sheet.Range[$"A{++index}"].Text = "Full name:";
            sheet.Range[$"B{index}"].Text = user.FirstName + " " + user.LastName;
            sheet.Range[$"A{++index}"].Text = "Email:";
            sheet.Range[$"B{index}"].Text = user.Email;

            sheet.Range[$"A{++index}"].Text = "Phone:";
            if (user.PhoneNumber == null)
            {
                sheet.Range[$"B{index}"].Text = "No phone";
            }
            else
            {
                sheet.Range[$"B{index}"].Text = user.PhoneNumber;
            }

            sheet.Range[$"A{++index}"].Text = "Teams:";
            List<UsersTeams> userTeams = _context.UsersTeams.Where(t => t.UserId == user.Id).ToList();
            char col = 'A';
            if (userTeams.Count==0)
            {
                sheet.Range[$"{++col}{index}"].Text = "No team name";
            }
            else
            {
                foreach (var ut in userTeams)
                {
                    sheet.Range[$"{++col}{index}"].Text = _context.Team.FirstOrDefault(t=>t.Id==ut.TeamId).Name;
                }
            }

            
            col = 'A';

            sheet.Range[$"A{++index}"].Text = "Interests:";
            List<UsersInterests> usersInterests = _context.UsersInterests.Where(t => t.UserId == user.Id).ToList();
            if (usersInterests.Count==0)
            {
                sheet.Range[$"{++col}{index}"].Text = "No interests";
            }
            else
            {
                foreach (var ut in usersInterests)
                {
                    sheet.Range[$"{++col}{index}"].Text = _context.Interests.FirstOrDefault(i=>i.Id==ut.InterestId).Name;

                }
            }

            col = 'A';
            sheet.Range[$"A{++index}"].Text = "Events:";
            List<UsersEvents> usersEvents = _context.UsersEvents.Where(t => t.UserId == user.Id).ToList();
            if (usersEvents.Count==0)
            {
                sheet.Range[$"{++col}{index}"].Text = "No events";
            }
            else
            {
                foreach (var ut in usersEvents)
                {
                    sheet.Range[$"{++col}{index}"].Text = _context.Event.FirstOrDefault(e => e.EventId == ut.EventId).Name;
                }
            }
            index++;
            //foreach(UsersEvents usersEvents in eventsUsers)
            //{
            //    events.Add(_context.Event.First(t => t.EventId == usersEvents.EventId));
            //}

            //foreach(Event ev in events)
            //{ 
            //    sheet.Range[$"C{position + startPostion}"].Text =ev.Name;
            //    position++;
            //}
            // wbToStream.Worksheets.Add(sheet);


        }


        [Authorize]        
        public void ExportAllUsers(ApplicationUser user)
        {

            Workbook wbToStream = new Workbook();
            Worksheet sheet = wbToStream.Worksheets[0];
            Worksheet sheet2 = wbToStream.Worksheets[1];
            sheet.Name = "Users";
            sheet2.Name = "AdminDetails";
            sheet2.Range["A1"].Text = "Exported by:";
            sheet2.Range["A2"].Text = user.FirstName + ' ' + user.LastName;
            sheet2.Range["B1"].Text = "Email:";
            sheet2.Range["B2"].Text = user.Email;
            sheet2.Range["C1"].Text = "Date:";
            sheet2.Range["C2"].Text = DateTime.Now.ToString();

            Color color = Color.FromArgb(248, 203, 173);
            
            sheet2.Range[$"A1:C1"].Style.Color = color;
            long index = 0;
            foreach (var userr in _context.Users)
            {
                ExportSingleUser(sheet, userr, ref index);
            }
            sheet.AllocatedRange.AutoFitColumns();
            sheet2.AllocatedRange.AutoFitColumns();
            FileStream file_stream = new FileStream($"AllUsersExport.xls", FileMode.OpenOrCreate);
            wbToStream.SaveToStream(file_stream);
            file_stream.Close();
        }

    }
}
