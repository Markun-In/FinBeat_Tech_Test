using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using FinBeat_Tech_Test.Services;
using FinBeat_Tech_Test;
using Microsoft.Extensions.Logging;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Info("Starting application...");
try
{
    
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    builder.Services.AddControllers();
    builder.Services.AddControllers()
        .AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented; // Удобное форматирование для улучшения читабельности
            options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
        });

    ConfigurationServices.SetDIConfiguration(builder.Services); // конфиг DI

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(swager =>
    {
        swager.SwaggerDoc("v1", new OpenApiInfo { Version = "v1", Title = "Values API" });
    });

    var app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
    logger.Info("Application is started");
}
catch (Exception ex)
{
    logger.Error($"Error during starting application: {ex.Message}", ex);
}
finally
{
    LogManager.Shutdown();
}

