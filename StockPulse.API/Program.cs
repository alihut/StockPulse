using StockPulse.API.Hubs;
using StockPulse.API.Mappings;
using StockPulse.API.Services;
using StockPulse.Application.Interfaces;
using StockPulse.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//builder.Services.AddSingleton<IStockPriceProvider, StockPriceSimulator>();
builder.Services.AddHostedService<StockPriceSimulator>();

builder.Services.AddScoped<IAlertService, AlertService>();
builder.Services.AddScoped<IStockPriceService, StockPriceService>();
builder.Services.AddScoped<INotificationService, SignalRNotificationService>();
builder.Services.AddScoped<IAlertEvaluationService, AlertEvaluationService>();
builder.Services.AddSignalR();

builder.Services.AddAutoMapper(typeof(AlertMappingProfile));


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapHub<AlertHub>("/alerts");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
