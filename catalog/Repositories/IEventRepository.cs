namespace Eburgan.Catalog.Repositories;

public interface IEventRepository
{
  Task<IEnumerable<Event>> GetEvents();
  Task<Event> GetEventById(Guid eventId);
  void UpdateSpecialOffer();
}
