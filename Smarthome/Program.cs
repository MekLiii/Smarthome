using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.FileProviders;
using Smarthome.Bulbs.interfaces;
using Smarthome.Bulbs.Services;
using Smarthome.mqtt;
using Smarthome.mqtt.interfaces;
using Smarthome.Rooms;



var builder = WebApplication.CreateBuilder(args);
const string myAllowSpecificOrigins = "_myAllowSpecificOrigins";


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IBulbsService, BulbsService>();
builder.Services.AddScoped<IRoomsService, RoomsService>();
builder.Services.AddSingleton<ImqttService>(_ => new mqttService());

builder.Services.AddCors(options =>
{
    options.AddPolicy(myAllowSpecificOrigins,
        policyOptions =>
        {
            policyOptions.WithOrigins("http://localhost:3001", "http://192.168.0.104:3000", "http://localhost:5183")
                .AllowAnyHeader().AllowAnyMethod();
        });
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseDefaultFiles(new DefaultFilesOptions { DefaultFileNames = new List<string> { "index.html" } });
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "fe")),
    RequestPath = "", 
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Cache-Control", "public, max-age=3600");
    }
});


app.MapGet("/", () =>
    Results.File(
        Path.Combine(app.Environment.ContentRootPath, "fe", "index.html"),
        "text/html")
);

app.UseCors(myAllowSpecificOrigins);
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// app.UseHttpsRedirection();
var serviceProvider = app.Services;
var mqttService = serviceProvider.GetRequiredService<ImqttService>();
await mqttService.ConnectMqttAsync();

app.Run();