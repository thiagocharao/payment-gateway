namespace PaymentAPI.Domain.Repositories
{
    using Microsoft.Extensions.Configuration;

    using MongoDB.Driver;

    public class PaymentRepository : BaseRepository<Payment>
    {
        protected override string CollectionName => "payments";

        public PaymentRepository(IMongoClient client, IConfiguration configuration) : base(client, configuration)
        {
        }
    }
}
