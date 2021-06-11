using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Basket.API.Extensions
{

  public static class CustomStartupExtensionMethods
  {

    public static IServiceCollection AddCustomOptions(this IServiceCollection services, IConfiguration configuration)
    {
      services.Configure<BasketSetttings>(configuration);
      return services;
    }

    public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
    {
      return services.AddStackExchangeRedisCache(options =>
      {
          options.Configuration = configuration.GetValue<string>("CacheSettings:ConnectionString");
      });
    }

    public static IServiceCollection AddCustomMVC(this IServiceCollection services, IConfiguration configuration)
    {
      services.AddCors(options =>
      {
        options.AddPolicy("CorsPolicy",
                  builder => builder
                  .SetIsOriginAllowed((host) => true)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials());
      });

      return services;
    }

    public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration)
    {
      services.AddSwaggerGen(options =>
      {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
          Title = "Basket.API",
          Version = "v1",
          Description = "The Basket Microservice"
        });
      });

      return services;

    }
  }
}