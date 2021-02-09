namespace PaymentAPI
{
    using System;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.OpenApi.Models;

    using Domain.IoC;

    using Models;

    using Prometheus;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var databaseConnectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING") ??
                                           Configuration["DevelopmentConnectionString"];
            var bankingApiBaseAddress = Environment.GetEnvironmentVariable("BANKING_API_BASE_ADDRESS") ??
                                        Configuration["BankingApiBaseAddress"];

            services.AddMongoClient(databaseConnectionString);
            services.AddRepositories();
            services.AddServices(bankingApiBaseAddress);
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddControllers();
            services.AddAuthenticationWithJwtValidation(Configuration["JWTIssuer"], Configuration["JWTSecret"]);
            services.AddAuthorization(options =>
            {
                options.AddPolicy("UserIdRequired", policy => policy.RequireClaim("userId"));
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "PaymentAPI",
                    Version = "v1"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description =
                        "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer MY_SUPER_SECRET_TOKEN\"",
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme, Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PaymentAPI v1"));
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseHttpMetrics(); // Must be called after app.UseRouting
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapMetrics();
            });
            
            app.Use((context, next) =>
            {
                // Http Context
                var counter = Metrics.CreateCounter("payment_api_path_counter", "Counts requests to Payment API endpoints", 
                    new CounterConfiguration
                    {
                        LabelNames = new[] { "method", "endpoint" }
                    });
                counter.WithLabels(context.Request.Method, context.Request.Path).Inc();
                return next();
            });
        }
    }
}
