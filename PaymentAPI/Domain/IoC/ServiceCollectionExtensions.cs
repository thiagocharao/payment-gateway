namespace PaymentAPI.Domain.IoC
{
    using Microsoft.Extensions.DependencyInjection;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.Serializers;
    using MongoDB.Driver;
    using Repositories;
    using Services;

    public static class ServiceCollectionExtensions
    {
        public static void AddMongoClient(this IServiceCollection services, string connectionString) =>
            services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));

        public static void AddRepositories(this IServiceCollection services)
        {
            // MongoDB.Driver stores decimal as strings by default, so we force it to store as decimal...
            BsonSerializer.RegisterSerializer(new DecimalSerializer(BsonType.Decimal128));
            services.AddScoped(typeof(IRepository<Payment>), typeof(PaymentRepository));
            services.AddScoped(typeof(IRepository<User>), typeof(UserRepository));
        }

        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IPaymentService), typeof(PaymentService));
        }
    }
}
