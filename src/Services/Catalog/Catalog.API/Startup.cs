using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.API.Data;
using Catalog.API.Repositories;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Catalog.API
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddControllers().AddNewtonsoftJson(opt =>
      {
        opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
      })
     .Services
     .AddCustomMVC(Configuration)
     .AddSwagger(Configuration);

     services.AddScoped<ICatalogContext, CatalogContext>();
     services.AddScoped<IProductRepository, ProductRepository>();

      services.AddHealthChecks().AddMongoDb(Configuration["DatabaseSettings:ConnectionString"], "MongoDb Health", HealthStatus.Degraded);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      //app.UseHttpsRedirection();

      app.UseRouting();

      app.UseAuthorization();

      app.UseSwagger()
            .UseSwaggerUI(c =>
            {
              c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog.API V1");
            });

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
        endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
      });
    }
  }

  public static class CustomExtensionMethods
  {

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
          Title = "Catalog.API",
          Version = "v1",
          Description = "The Catalog Microservice",
          Contact = new OpenApiContact
          {
            Name = "M Haris",
            Email = string.Empty,
            Url = new Uri("https://github.com/haristauqir"),
          }
        });
      });

      return services;

    }
  }
}
