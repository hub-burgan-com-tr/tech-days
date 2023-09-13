﻿using System;
using System.Threading.Tasks;
using Eburgan.Frontend.Models;
using Eburgan.Frontend.Models.View;
using Eburgan.Frontend.Services;
using Eburgan.Frontend.Services.Ordering;
using Eburgan.Frontend.Extensions;
using Eburgan.Frontend.Models.Api;
using Microsoft.AspNetCore.Mvc;

namespace Eburgan.Frontend.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IShoppingBasketService shoppingBasketService;
        private readonly IOrderSubmissionService orderSubmissionService;
        private readonly Settings settings;
        private readonly ILogger<CheckoutController> logger;

        public CheckoutController(IShoppingBasketService shoppingBasketService, 
            IOrderSubmissionService orderSubmissionService,
            Settings settings,ILogger<CheckoutController> logger)
        {
            this.shoppingBasketService = shoppingBasketService;
            this.orderSubmissionService = orderSubmissionService;
            this.settings = settings;
            this.logger = logger;
        }

        public IActionResult Index()
        {
            var currentBasketId = Request.Cookies.GetCurrentBasketId(settings);
            //var basket = await shoppingBasketService.GetBasket(currentBasketId);

            return View(new CheckoutViewModel() {  BasketId = currentBasketId});
        }

        public IActionResult Thanks()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Purchase(CheckoutViewModel checkout)
        {
            if (ModelState.IsValid)
            {
                var currentBasketId = Request.Cookies.GetCurrentBasketId(settings);
                checkout.BasketId = currentBasketId;
                
                logger.LogInformation($"Received an order from {checkout.Name}");
                var orderId = await orderSubmissionService.SubmitOrder(checkout);
                await shoppingBasketService.ClearBasket(currentBasketId);

                return RedirectToAction("Thanks");
            }
            else
            {
                return View("Index");
            }
        }

    }
}
