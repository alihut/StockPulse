using Microsoft.EntityFrameworkCore;
using StockPulse.API.Extensions;
using StockPulse.API.Hubs;
using StockPulse.API.Mappings;
using StockPulse.API.Middlewares;
using StockPulse.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.AddConfigurationSettings();


builder.Services.AddDbContext<StockPulseDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.AddJwtAuthentication();

builder.AddApplicationServices();
builder.Services.AddSignalR();

builder.Services.AddAutoMapper(typeof(AlertMappingProfile));


builder.Services.AddControllers();
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

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
