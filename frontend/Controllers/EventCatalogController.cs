using System;
using System.Threading.Tasks;
using Eburgan.Frontend.Models;
using Eburgan.Frontend.Models.Api;
using Eburgan.Frontend.Models.View;
using Eburgan.Frontend.Services;
using Eburgan.Frontend.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Eburgan.Frontend.Controllers
{
    public class EventCatalogController : Controller
    {
        private readonly IEventCatalogService eventCatalogService;
        private readonly IShoppingBasketService shoppingBasketService;
        private readonly Settings settings;

        public EventCatalogController(IEventCatalogService eventCatalogService, IShoppingBasketService shoppingBasketService, Settings settings)
        {
            this.eventCatalogService = eventCatalogService;
            this.shoppingBasketService = shoppingBasketService;
            this.settings = settings;
        }

        public async Task<IActionResult> Index()
        {
            var currentBasketId = Request.Cookies.GetCurrentBasketId(settings);

            var getBasket = currentBasketId == Guid.Empty ? Task.FromResult<Basket>(null) :
                shoppingBasketService.GetBasket(currentBasketId);
            var getEvents = eventCatalogService.GetAll();

            await Task.WhenAll(new Task[] { getBasket, getEvents });

            var numberOfItems = getBasket.Result == null ? 0 : getBasket.Result.NumberOfItems;

            return View(
                new EventListModel
                {
                    Events = getEvents.Result,
                    NumberOfItems = numberOfItems,
                }
            );
        }

        public async Task<IActionResult> Detail(Guid eventId)
        {
            var ev = await eventCatalogService.GetEvent(eventId);
            return View(ev);
        }
    }
}
