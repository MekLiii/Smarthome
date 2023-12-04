using ZigBeeNet;
using System;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.FileProviders;
using Smarthome.Bulbs.interfaces;
using Smarthome.Bulbs.Services;
using Smarthome.Rooms;

var builder = WebApplication.CreateBuilder(args);
var myAllowSpecificOrigins = "_myAllowSpecificOrigins";


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IBulbsService, BulbsService>();
builder.Services.AddScoped<IRoomsService, RoomsService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy(myAllowSpecificOrigins,
        builder =>
        {
            builder.WithOrigins("http://localhost:3001", "http://192.168.0.104:3000", "http://localhost:5183")
                .AllowAnyHeader().AllowAnyMethod();
        });
});

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseDefaultFiles(new DefaultFilesOptions { DefaultFileNames = new List<string> { "index.html" } });
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "fe", "assets")),
    RequestPath = "/assets"
});

app.MapGet("/", () =>
    Results.File(
        Path.Combine(app.Environment.ContentRootPath, "fe", "index.html"),
        "text/html")
);


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(myAllowSpecificOrigins);
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseHttpsRedirection();


app.Run();