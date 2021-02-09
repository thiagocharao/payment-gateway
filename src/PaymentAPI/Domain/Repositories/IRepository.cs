namespace PaymentAPI.Domain.Repositories
{
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using System.Threading;
    using System;

    public interface IRepository<TDocument> where TDocument : IDocument
    {
        Task<IEnumerable<TDocument>> FilterByAsync(Expression<Func<TDocument, bool>> filterExpression,
            CancellationToken ct = default);

        Task<TDocument> FindOneAsync(Expression<Func<TDocument, bool>> filterExpression,
            CancellationToken ct = default);

        Task<TDocument> InsertOneAsync(TDocument document, CancellationToken ct = default);

        Task<TDocument> ReplaceOneAsync(TDocument document, CancellationToken ct = default);
    }
}
