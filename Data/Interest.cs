using System.ComponentModel.DataAnnotations;

namespace NemeTschek.Data
{
    public class Interest
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<ApplicationUser> Users { get; set; }

    }
}