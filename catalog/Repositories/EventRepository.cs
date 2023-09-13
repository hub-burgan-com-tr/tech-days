using System.Text.Json;
using Dapr.Client;

namespace Eburgan.Catalog.Repositories;

public class EventRepository : IEventRepository
{
    private List<Event> events = new List<Event>();
    private readonly DaprClient daprClient;
    private readonly ILogger<EventRepository> logger;

    public EventRepository(DaprClient daprClient, ILogger<EventRepository> logger)
    {
        this.daprClient = daprClient;
        this.logger = logger;

        LoadSampleData();
    }

    private void LoadSampleData()
    {
        var zoren = Guid.Parse("{CFB88E29-4744-48C0-94FA-B25B92DEA317}");
        var eregl = Guid.Parse("{CFB88E29-4744-48C0-94FA-B25B92DEA318}");
        var ebebk = Guid.Parse("{CFB88E29-4744-48C0-94FA-B25B92DEA319}");

        events.Add(new Event
        {
            EventId = zoren,
            Name = "Zorlu Enerji",
            Price = 5,
            Artist = "zoren",
            Date = DateTime.Now.AddMonths(6),
            Description = "Zorlu Grubunun üyesi ZORLU ENERJİ ELEKTRİK ÜRETİM ANONİM ŞİRKETİ, Türkiye merkezli ve yine Türkiye’de bağımsız enerji üretimi yapan bir şirkettir.",
            ImageUrl = "/img/zoren.png",
        });

        events.Add(new Event
        {
            EventId = eregl,
            Name = "Ereğli Demir ve Çelik Fabrikaları T.A.Ş. Şirket ",
            Price = 45,
            Artist = "eregl",
            Date = DateTime.Now.AddMonths(9),
            Description = "EREĞLİ DEMİR VE ÇELİK FABRİKALARI TÜRK ANONİM ŞİRKETİ, Türkiye merkezli bir demir ve haddelenmış çelik, alaşımlı ve alaşımsız demir, demir döküm, döküm ve preslenmiş ürünler, kok ve yan ürünleri üreticisidir.",
            ImageUrl = "/img/eregl.png",
        });

        events.Add(new Event
        {
            EventId = ebebk,
            Name = "Ebebek Magazacilik AS",
            Price = 68,
            Artist = "ebebk",
            Date = DateTime.Now.AddMonths(8),
            Description = "Türkiye'nin ilk ve en büyük anne-bebek ürünleri online alışveriş sitesi ve mağazaları olan ebebek, 2001'den bu yana anne ilgisi uzman bilgisiyle yanınızda!",
            ImageUrl = "/img/ebebk.png",
        });
    }

    public async Task<IEnumerable<Event>> GetEvents()
    {
        try
        {
          var connectionString = await GetConnectionString();
          logger.LogInformation($"Connection string {connectionString}");
        }
        catch(Exception e)
        {
          logger.LogError(e, "Failed to fetch the connection string");
        }
        return events;
    }

    private async Task<string> GetConnectionString()
    {
        // using the DAPR_HTTP_PORT environment variable to detect if we're
        // not running in DAPR
        var daprPort = Environment.GetEnvironmentVariable("DAPR_HTTP_PORT");
        if (string.IsNullOrEmpty(daprPort)) return "Not using Dapr";

        // because in Kubernetes we want to use the "kubernetes" secret store,
        // we're making the secret store name configurable here
        var secretStoreName = Environment.GetEnvironmentVariable("SECRET_STORE_NAME");
        if(string.IsNullOrEmpty(secretStoreName)) secretStoreName = "secretstore";
        var secretName = "eventcatalogdb";
        var secret = await daprClient.GetSecretAsync(secretStoreName, secretName);
        return secret[secretName];
    }

    public Task<Event> GetEventById(Guid eventId)
    {
        var @event = events.FirstOrDefault(e => e.EventId == eventId);
        if (@event == null)
        {
            throw new InvalidOperationException("Event not found");
        }
        return Task.FromResult(@event);
    }

    // scheduled task calls this periodically to put one item on special offer
    public void UpdateSpecialOffer()
    {
        // reset all tickets to their default
        events.Clear();
        LoadSampleData();
        // pick a random one to put on special offer
        var random = new Random();
        var specialOfferEvent = events[random.Next(0,events.Count)];
        // 20 percent off
        specialOfferEvent.Price = (int)(specialOfferEvent.Price * 0.8);
    }
}
