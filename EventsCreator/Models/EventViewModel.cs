using System.ComponentModel.DataAnnotations;

namespace EventsCreator.Models
{
    public class EventViewModel
    {
        [Required(ErrorMessage = "NameOfEvent is empty")]
        public string NameOfEvent { get; set; }
        public string? Description { get; set; }
        public string? Speaker { get; set; }
        public DateTime EventTime { get; set; }
    }
}
