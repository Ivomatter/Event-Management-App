namespace NemeTschek.Data
{
    public class UsersInterests
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int InterestId { get; set; }
        public Interest Interest { get; set; }


    }
}
