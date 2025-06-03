using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
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

builder.AddApplicationServices();
builder.AddRepositories();
builder.Services.AddMassTransitWithRabbitMq(builder.Configuration);
builder.Services.AddSignalR();

builder.Services.AddAutoMapper(typeof(AlertMappingProfile));

//builder.Services.AddCors(options =>
//{
//    options.AddDefaultPolicy(policy =>
//    {
//        policy
//            .AllowAnyOrigin() // ⬅️ this disables origin checks
//            .AllowAnyHeader()
//            .AllowAnyMethod();
//    });
//});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins("http://localhost:5173") // veya hangi porttan sunuyorsan
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMemoryCache();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "StockPulse API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token in the format: Bearer {token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer" // This MUST match the AddSecurityDefinition key
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);


var app = builder.Build();


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
