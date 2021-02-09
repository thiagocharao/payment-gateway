namespace PaymentAPI.Domain
{
    using System;

    public abstract class Document : IDocument
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }

        protected Document()
        {
            CreatedAt = DateTime.UtcNow;
        }
    }
}
