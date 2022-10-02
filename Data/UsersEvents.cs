using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NemeTschek.Data
{
    public class UsersEvents
    {
        //[Key, Column(Order = 0)]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        //[Key, Column(Order = 1)]
        public int EventId { get; set; }
        public Event Event { get; set; }
    }
}
