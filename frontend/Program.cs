using Eburgan.Frontend.Services;
using Eburgan.Frontend.Models;
using Eburgan.Frontend.Services.Ordering;
using Dapr.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// note: for this demo we're using the DAPR_HTTP_PORT environment variable to decide if we're using Dapr or not
var daprPort = Environment.GetEnvironmentVariable("DAPR_HTTP_PORT");
if (String.IsNullOrEmpty(daprPort))
{
    // we're not running in DAPR - use regular service invocation and an in-memory basket
    Console.WriteLine("DAPR kullanılmıyor");
    builder.Services.AddHttpClient<IEventCatalogService, EventCatalogService>((sp, c) =>
        c.BaseAddress = new Uri(sp.GetService<IConfiguration>()?["ApiConfigs:EventCatalog:Uri"] ?? throw new InvalidOperationException("Missing config")));
    builder.Services.AddSingleton<IShoppingBasketService, InMemoryShoppingBasketService>();
    builder.Services.AddHttpClient<IOrderSubmissionService, HttpOrderSubmissionService>((sp, c) =>
        c.BaseAddress = new Uri(sp.GetService<IConfiguration>()?["ApiConfigs:Ordering:Uri"] ?? throw new InvalidOperationException("Missing config")));
}
else
{
    Console.WriteLine("Yaşasın DAPR var :) ");
    builder.Services.AddDaprClient();
    // Using the DAPR SDK to create a DaprClient, in stead of fiddling with URI's our selves
    builder.Services.AddSingleton<IEventCatalogService>(sc => 
        new EventCatalogService(DaprClient.CreateInvokeHttpClient("catalog")));
    
    builder.Services.AddScoped<IShoppingBasketService, DaprClientStateStoreShoppingBasket>();
    builder.Services.AddScoped<IOrderSubmissionService, DaprOrderSubmissionService>();
}

builder.Services.AddSingleton<Settings>();
builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Turning this off to simplify the running in Kubernetes demo
// app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=EventCatalog}/{action=Index}/{id?}");

app.Run();
