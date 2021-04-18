using System;
using System.Threading.Tasks;
using GloboTicket.Grpc;
using GloboTicket.Web.Extensions;
using GloboTicket.Web.Models;
using GloboTicket.Web.Models.Api;
using GloboTicket.Web.Models.View;
using GloboTicket.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace GloboTicket.Web.Controllers
{
    public class EventCatalogController : Controller
    {
        private readonly Events.EventsClient eventCatalogService_gRPC;
        private readonly IEventCatalogService eventCatalogService;
        private readonly IShoppingBasketService shoppingBasketService;
        private readonly Settings settings;

        public EventCatalogController(Events.EventsClient eventCatalogService_gRPC,
                                      IEventCatalogService eventCatalogService, 
                                      IShoppingBasketService shoppingBasketService,
                                      Settings settings)
        {
            this.eventCatalogService_gRPC = eventCatalogService_gRPC;
            this.eventCatalogService = eventCatalogService;
            this.shoppingBasketService = shoppingBasketService;
            this.settings = settings;
        }

        public async Task<IActionResult> Index(Guid categoryId)
        {
            //var getCategories = eventCatalogService.GetCategories();
            //var getEvents = categoryId == Guid.Empty ? eventCatalogService.GetAll() :
            //                                           eventCatalogService.GetByCategoryId(categoryId);
            //await Task.WhenAll(new Task[] { getCategories, getEvents });            

            //return View(
            //    new EventListModel
            //    {
            //        Events = getEvents.Result,
            //        Categories = getCategories.Result,
            //        SelectedCategory = categoryId
            //    }
            //);
            var currentBasketId = Request.Cookies.GetCurrentBasketId(settings);
            var getBasket = currentBasketId == Guid.Empty ? Task.FromResult<Basket>(null) : shoppingBasketService.GetBasket(currentBasketId);

            var getCategories = eventCatalogService_gRPC.GetAllCategoriesAsync(new GetAllCategoriesRequest());
            var getEvents = categoryId == Guid.Empty ? eventCatalogService_gRPC.GetAllAsync(new GetAllEventsRequest()) :
                                                       eventCatalogService_gRPC.GetAllByCategoryIdAsync(new GetAllEventsByCategoryIdRequest { CategoryId = categoryId.ToString() });
            await Task.WhenAll(new Task[] { getCategories.ResponseAsync, getEvents.ResponseAsync, getBasket });

            var numberOfItems = getBasket.Result == null ? 0 : getBasket.Result.NumberOfItems;

            return View(
                new EventListModel
                {
                    Events = getEvents.ResponseAsync.Result.Events,
                    Categories = getCategories.ResponseAsync.Result.Categories,
                    SelectedCategory = categoryId,
                    NumberOfItems = numberOfItems
                }
            );
        }

        [HttpPost]
        public IActionResult SelectCategory([FromForm]Guid selectedCategory)
        {
            return RedirectToAction("Index", new { categoryId = selectedCategory });
        }

        public async Task<IActionResult> Detail(Guid eventId)
        {
            var currentBasketId = Request.Cookies.GetCurrentBasketId(settings);
            var getBasket = currentBasketId == Guid.Empty ? Task.FromResult<Basket>(null) : shoppingBasketService.GetBasket(currentBasketId);
            await Task.WhenAll(new Task[] { getBasket });
            ViewBag.NumberOfItems = getBasket.Result == null ? 0 : getBasket.Result.NumberOfItems;
            //var ev = await eventCatalogService.GetEvent(eventId);
            //return View(ev);
            var ev = await eventCatalogService_gRPC.GetByEventIdAsync(new GetByEventIdRequest { EventId = eventId.ToString() });
            return View(ev.Event);
        }
    }
}
