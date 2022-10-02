using System.ComponentModel.DataAnnotations;

namespace NemeTschek.Data
{
    public class Team
    {
        [Key]
        private int teamId;
        private string name;
        private ICollection<ApplicationUser> applicationUsers;

        public Team(string name)
        {
            Name = name;
        }

        public int Id { get; set; }
        public string Name { get => name; set => name = value; }
        public ICollection<ApplicationUser> ApplicationUsers { get => applicationUsers; set => applicationUsers = value; }
    }
}
