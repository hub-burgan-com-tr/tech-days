﻿using Eburgan.Frontend.Models;
using Eburgan.Frontend.Models.View;
using Eburgan.Frontend.Services;
using Eburgan.Frontend.Services.Ordering;
using Eburgan.Frontend.Extensions;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Mvc;

namespace Eburgan.Frontend.Controllers;

public class CheckoutController : Controller
{
    private readonly IShoppingBasketService shoppingBasketService;
    private readonly IOrderSubmissionService orderSubmissionService;
    private readonly Settings settings;
    private readonly ILogger<CheckoutController> logger;
    private readonly TelemetryClient telemetryClient;

    public CheckoutController(IShoppingBasketService shoppingBasketService,
        IOrderSubmissionService orderSubmissionService,TelemetryClient telemetry,
        Settings settings, ILogger<CheckoutController> logger)
    {
        this.shoppingBasketService = shoppingBasketService;
        this.orderSubmissionService = orderSubmissionService;
        this.settings = settings;
        this.logger = logger;
        this.telemetryClient = telemetry;
    }

    public IActionResult Index()
    {
        var currentBasketId = Request.Cookies.GetCurrentBasketId(settings);

        return View(new CheckoutViewModel() { BasketId = currentBasketId });
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
            SendAppinsigtsTelemetryOrderPlaced();

            var orderId = await orderSubmissionService.SubmitOrder(checkout);
            await shoppingBasketService.ClearBasket(currentBasketId);

            return RedirectToAction("Thanks");
        }
        else
        {
            return View("Index");
        }


    }
    private void SendAppinsigtsTelemetryOrderPlaced()
    {
        MetricTelemetry telemetry = new MetricTelemetry();
        telemetry.Name = "Order Placed";
        
        telemetry.Sum = Random.Shared.Next(600);
        telemetryClient.TrackMetric(telemetry);
    }

}
