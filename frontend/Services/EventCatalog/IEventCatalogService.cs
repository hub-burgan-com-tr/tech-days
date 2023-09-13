using Eburgan.Frontend.Models.Api;

namespace Eburgan.Frontend.Services;

public interface IEventCatalogService
{
    Task<IEnumerable<Event>> GetAll();

    Task<Event> GetEvent(Guid id);

}
