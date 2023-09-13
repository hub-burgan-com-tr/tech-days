using Dapr.Client;
using Eburgan.Ordering.Model;

namespace Eburgan.Ordering.Services;

public class EmailSender
{
    private readonly DaprClient daprClient;
    private readonly ILogger<EmailSender> logger;

    public EmailSender(DaprClient daprClient, ILogger<EmailSender> logger)
    {
        this.daprClient = daprClient;
        this.logger = logger;
    }

    public async Task SendEmailForOrder(OrderForCreation order)
    {
        logger.LogInformation($"{order.CustomerDetails.Email} mailine sahip müşteriden yeni sipariş var!!!");

        var daprEnabled = !String.IsNullOrEmpty(Environment.GetEnvironmentVariable("DAPR_HTTP_PORT"));
        if (!daprEnabled) 
        { 
            logger.LogWarning("Dapr yoksa email de yok :) ");
            return;
        }
        
        logger.LogInformation($"Sending email");
        var metadata = new Dictionary<string, string>
        {
          ["emailFrom"] = "siparis@eburgan.com",
          ["emailTo"] = order.CustomerDetails.Email,
          ["subject"] = $"Siparişiniz için teşekkürler"
        };
        var body = $"<h2>Siparişiniz alınmıştır</h2>"
            + "<p>En kısa sürede siparişiniz sizlere ulaştırılacaktır</p>";
        
        await daprClient.InvokeBindingAsync("sendmail", "create", 
            body, metadata);        
    }
}