using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLog;
using Presentation.ActionFilters;
using Repositories.EFCore;
using Services;
using Services.Contracts;
using WebAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

LogManager.LoadConfiguration(String.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

builder.Services.AddControllers(config =>
{
    config.RespectBrowserAcceptHeader = true;
    config.ReturnHttpNotAcceptable = true;
    config.CacheProfiles.Add("5mins",new CacheProfile() { Duration = 300});
})
    //Xml formatinda response saglandi
    .AddXmlDataContractSerializerFormatters()
    //Csv formatinda response saglandi (Extension yazildi)
    .AddCustomCsvFormatter()
    .AddApplicationPart(typeof(Presentation.AssemblyReference).Assembly);
    //.AddNewtonsoftJson();



builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureLoggerService();
builder.Services.AddAutoMapper(typeof(Program));
//ValidationFilters
builder.Services.ConfigureActionFilters();
//Cors options
builder.Services.ConfigureCors();
// Data Shaper
builder.Services.ConfigureDataShaper();
builder.Services.AddCustomMediaTypes();
builder.Services.AddScoped<IBookLinks, BookLinks>();
// Api Versioning
builder.Services.ConfigureVersioning();
// Caching
builder.Services.ConfigureResponseCaching();
builder.Services.ConfigureHttpCacheHeaders();
// Rate Limiting
builder.Services.AddMemoryCache();
builder.Services.ConfigureRateLimitingOptions();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILoggerService>();
app.ConfigureExceptionHandler(logger);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsProduction())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

//Corsdan once rate limiting
app.UseIpRateLimiting();
//ConfigureCors icerisinde CorsPolicy tanimlandi. Oradaki ozellikleri kullanmasini soylemis olduk.
app.UseCors("CorsPolicy");
// Caching Corsdan sonra yazilmasi oneriliyor
app.UseResponseCaching();
app.UseHttpCacheHeaders();

app.UseAuthorization();

app.MapControllers();

app.Run();
