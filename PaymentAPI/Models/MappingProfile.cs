namespace PaymentAPI.Models
{
    using AutoMapper;

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Payments
            CreateMap<PaymentRequest, Domain.Payment>();
            CreateMap<Domain.Payment, PaymentResponse>();

        }
    }
}
