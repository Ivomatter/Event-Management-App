using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace NemeTschek.Data
{
    public class ApplicationUser : IdentityUser
    {
        [Key]
      //  public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? PhotoDirectory { get; set; }
        public ICollection<Event> Events { get => events; set => events = value; }
        
        public ICollection<Team> Teams { get; set; }

        public ICollection<Interest> Interests { get; set; }

        public ICollection<Event> events;

    }
}
