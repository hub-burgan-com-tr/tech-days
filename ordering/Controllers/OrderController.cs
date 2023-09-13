using Dapr;
using Eburgan.Ordering.Model;
using Eburgan.Ordering.Services;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Mvc;

namespace Eburgan.Ordering.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly ILogger<OrderController> logger;
    private readonly EmailSender emailSender;
    private readonly TelemetryClient telemetryCient;

    public OrderController(ILogger<OrderController> logger, TelemetryClient telemetryClient, EmailSender emailSender)
    {
        this.logger = logger;
        this.emailSender = emailSender;
        this.telemetryCient = telemetryClient;
    }

    [HttpPost("", Name = "SubmitOrder")]
    [Topic("pubsub", "orders")]
    public async Task<IActionResult> Submit(OrderForCreation order)
    {
        logger.LogInformation($"{order.CustomerDetails.Email} mailine sahip müşteriden yeni sipariş var!!!");
        SendAppinsigtsTelemetryOrderProcesed();
        await emailSender.SendEmailForOrder(order);
        return Ok();
    }

    private void SendAppinsigtsTelemetryOrderProcesed()
    {
        MetricTelemetry telemetry = new MetricTelemetry();
        telemetry.Name = "Sipariş işlendi";

        telemetry.Count = 1 ;
        telemetryCient.TrackMetric(telemetry);
    }
}
