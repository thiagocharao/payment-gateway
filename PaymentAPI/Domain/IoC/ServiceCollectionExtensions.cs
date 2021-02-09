namespace PaymentAPI.Domain.IoC
{
    using System.Text;
    using System.Threading.Tasks;
    using System;

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;

    using MongoDB.Bson.Serialization.Serializers;
    using MongoDB.Bson.Serialization;
    using MongoDB.Bson;
    using MongoDB.Driver;

    using Prometheus;

    using Repositories;

    using Services;


    public static class ServiceCollectionExtensions
    {
        public static void AddMongoClient(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));
        }

        public static void AddRepositories(this IServiceCollection services)
        {
            // MongoDB.Driver stores decimal as strings by default, so we force it to store as decimal...
            BsonSerializer.RegisterSerializer(new DecimalSerializer(BsonType.Decimal128));
            services.AddScoped(typeof(IRepository<Payment>), typeof(PaymentRepository));
            services.AddScoped(typeof(IRepository<User>), typeof(UserRepository));
        }

        public static void AddServices(this IServiceCollection services, string bankingApiBaseAddress)
        {
            services.AddScoped(typeof(ITokenService), typeof(TokenService));
            services.AddScoped(typeof(IPaymentService), typeof(PaymentService));
            services.AddScoped(typeof(IBankingService), typeof(BankingService));
            services.AddHttpClient<IBankingService, BankingService>(client =>
            {
                client.BaseAddress = new Uri(bankingApiBaseAddress);
            }).UseHttpClientMetrics();
        }

        public static void AddAuthenticationWithJwtValidation(this IServiceCollection services, string issuer, string secret)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = issuer,
                        IssuerSigningKey =
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Add("Token-Expired", "true");
                            }

                            return Task.CompletedTask;
                        }
                    };
                });
        }
    }
}
