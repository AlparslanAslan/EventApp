namespace EventApp.Models;
public class EventsForUpdate
{
    public Event _event { get; set; }
    public IEnumerable<Event> _events {get;set;}
}