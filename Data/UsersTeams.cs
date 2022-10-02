namespace NemeTschek.Data
{
    public class UsersTeams
    {
    
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int TeamId { get; set; }
        public Team Team { get; set; }
    }
}
