namespace PaymentAPI.Domain.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using MongoDB.Driver;

    public abstract class BaseRepository<TDocument> : IRepository<TDocument> where TDocument: IDocument
    {
        protected abstract string CollectionName { get; }

        private readonly IMongoCollection<TDocument> _collection;

        public BaseRepository(IMongoClient client, IConfiguration configuration)
        {
            var database = client.GetDatabase(configuration["DatabaseName"]);
            _collection = database.GetCollection<TDocument>(this.CollectionName);
        }

        public async Task<IEnumerable<TDocument>> FilterByAsync(
            Expression<Func<TDocument, bool>> filterExpression, CancellationToken ct = default)
        {
            var results = await _collection.FindAsync(filterExpression, null, ct);
            return await results.ToListAsync(ct);
        }

        public async Task<TDocument> FindOneAsync(Expression<Func<TDocument, bool>> filterExpression, CancellationToken ct = default)
        {
            var result = await _collection.FindAsync(filterExpression, null, ct);
            return await result.SingleOrDefaultAsync(ct);
        }
    }
}
