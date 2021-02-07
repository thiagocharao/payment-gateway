namespace PaymentAPI.Domain.Repositories
{
    using Microsoft.Extensions.Configuration;
    using MongoDB.Driver;

    public class UserRepository : BaseRepository<User>
    {
        protected override string CollectionName => "users";

        public UserRepository(IMongoClient client, IConfiguration configuration) : base(client, configuration)
        {
        }
    }
}
