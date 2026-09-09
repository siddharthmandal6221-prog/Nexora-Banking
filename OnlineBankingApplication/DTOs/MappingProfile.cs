using AutoMapper;
using OnlineBankingApplication.Models;

namespace OnlineBankingApplication.DTOs
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Customer, CustomerDto>();
            CreateMap<CustomerDto, Customer>();

            CreateMap<BankAccount, BankAccountDto>();
            CreateMap<BankAccountDto, BankAccount>();

            CreateMap<Beneficiary, BeneficiaryDto>();
            CreateMap<BeneficiaryDto, Beneficiary>();

            CreateMap<Transaction, TransactionDto>();
            CreateMap<TransactionDto, Transaction>();

            CreateMap<BillPayment, BillPaymentDto>()
                .ForMember(
                    dest => dest.BankAccountNumber,
                    opt => opt.MapFrom(src =>
                        src.BankAccount != null
                            ? src.BankAccount.AccountNumber
                            : null));

            CreateMap<BillPaymentDto, BillPayment>();

            CreateMap<Card, CardDto>()
                .ForMember(
                    dest => dest.BankAccountNumber,
                    opt => opt.MapFrom(src =>
                        src.BankAccount != null
                            ? src.BankAccount.AccountNumber
                            : null));

            CreateMap<CardDto, Card>();

            CreateMap<Subscription, SubscriptionDto>()
                .ForMember(
                    dest => dest.CustomerName,
                    opt => opt.MapFrom(src =>
                        src.Customer != null
                            ? src.Customer.FullName
                            : null))
                .ForMember(
                    dest => dest.BankAccountNumber,
                    opt => opt.MapFrom(src =>
                        src.BankAccount != null
                            ? src.BankAccount.AccountNumber
                            : null));

            CreateMap<SubscriptionDto, Subscription>();

            // Cheque Book Request
            CreateMap<ChequeBookRequest, ChequeBookRequestDto>()
                .ForMember(
                    dest => dest.BankAccountNumber,
                    opt => opt.MapFrom(src =>
                        src.BankAccount != null
                            ? src.BankAccount.AccountNumber
                            : null))
                .ForMember(
                    dest => dest.BankAccountType,
                    opt => opt.MapFrom(src =>
                        src.BankAccount != null
                            ? src.BankAccount.AccountType
                            : null));

            CreateMap<ChequeBookRequestDto, ChequeBookRequest>();
        }
    }
}