using ZigBeeNet;
using System;
using Smarthome.Bulbs.interfaces;
using Smarthome.Bulbs.Services;
using Smarthome.Rooms;

var builder = WebApplication.CreateBuilder(args);
const string myAllowSpecificOrigins = "_myAllowSpecificOrigins";


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IBulbsService, BulbsService>();
builder.Services.AddScoped<IRoomsService, RoomsService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("http://localhost:3001")
                .AllowAnyHeader()
                .AllowAnyMethod();
            
            builder.WithOrigins("192.168.0.104:3001")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseHttpsRedirection();
app.UseCors(myAllowSpecificOrigins);


app.Run();