using Eburgan.Frontend.Models.Api;

namespace Eburgan.Frontend.Models.View;

public class EventListModel
{
    public IEnumerable<Event> Events { get; set; } = new List<Event>();
    public int NumberOfItems { get; set; }
}
