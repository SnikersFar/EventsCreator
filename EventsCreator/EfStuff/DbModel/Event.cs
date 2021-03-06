namespace EventsCreator.EfStuff.DbModel
{
    public class Event
    {
        public long Id { get; set; }
        public string NameOfEvent { get; set; }
        public string? Description { get; set; }
        public string? Speaker {  get; set; }
        public DateTime EventTime { get; set; }
        public virtual List<User> Participants { get; set; }
        public virtual User Creator { get; set; }
    }
}
