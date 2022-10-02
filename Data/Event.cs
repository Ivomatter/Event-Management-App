namespace NemeTschek.Data
{
    public class Event
    {
        private int eventId;
        private string name;
        private string description;
        private DateTime startDate;
        private DateTime endDate;
        private string location;
        private uint minPeople;
        private uint maxPeople;
        private ICollection<ApplicationUser> applicationUsers;
        public Event(int eventId, string name, string description, DateTime startDate, DateTime endDate, string location, uint minimalPeopleCount, uint maximumPeopleCount)
           {
               EventId = eventId;
               Name = name;
               Description = description;
               StartDate = startDate;
               EndDate = endDate;
               Location = location;
               MinPeople = minimalPeopleCount;
               MaxPeople = maximumPeopleCount;
               applicationUsers = new List<ApplicationUser>();
           }

           public Event()
           {
               eventId = 0;
               name = "\0";
               description = "\0";
               location = "\0";
               maxPeople = 0;
               minPeople = 0;
               startDate = new DateTime();
               endDate = new DateTime();
               applicationUsers = new List<ApplicationUser>();
        }
        public int EventId { get => eventId; set => eventId = value; }
        public string Name { get => name; set => name = value; }
        public string Description { get => description; set => description = value; }
        public DateTime StartDate { get => startDate; set => startDate = value; }
        public DateTime EndDate { get => endDate; set => endDate = value; }
        public string Location { get => location; set => location = value; }
        public uint MinPeople { get => minPeople; set => minPeople = value; }
        public uint MaxPeople { get => maxPeople; set => maxPeople = value; }
        public ICollection<ApplicationUser> ApplicationUsers { get => applicationUsers; set => applicationUsers = value; }

    }
}
