using DNV.Context.AspNet;
using DNV.Context.ServiceBus.Sample.SendMessage.Models;
using System.Runtime.CompilerServices;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", false, true)
                     .AddJsonFile($"appsettings.{builder.Environment}.json", true);
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

builder.Services.AddOptions<ServiceBusConfig>().Configure(options =>
{
    builder.Configuration.GetSection("AzureServiceBus").Bind(options);
});

builder.Services.AddAspNetContext<SampleContext>(payloadCreator =>
{
    //build default context
    return (true, new SampleContext()
    {
        Name = "Sample",
        SampleData = "This is a sample context"
    });
});


// Add services to the container.
builder.Services.AddControllersWithViews();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

//register middleware for aspnetcontext
app.UseAspNetContext<SampleContext>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
