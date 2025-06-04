using Microsoft.EntityFrameworkCore;
using StockPulse.API.Extensions;
using StockPulse.API.Hubs;
using StockPulse.API.Mappings;
using StockPulse.API.Middlewares;
using StockPulse.Infrastructure.Data;
using StockPulse.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddConfigurationSettings();


builder.Services.AddDbContext<StockPulseDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.AddJwtAuthentication();

//builder.AddRedis();
builder.AddApplicationServices();
builder.AddRepositories();
builder.Services.AddMassTransitWithRabbitMq(builder.Configuration);
builder.Services.AddSignalR();

builder.Services.AddAutoMapper(typeof(AlertMappingProfile));


builder.AddCustomCors();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMemoryCache();

builder.AddCustomSwagger();
builder.AddRedLockFactory();

builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

var app = builder.Build();

await app.ApplyMigrationsAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<AlertHub>("/alerts");

app.Run();
