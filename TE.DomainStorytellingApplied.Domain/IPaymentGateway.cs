namespace TE.DomainStorytellingApplied.Domain;

public enum PaymentResult
{
    Success,
    Failure
}

public interface IPaymentGateway
{
    PaymentResult Pay(Id bookingId, decimal amount);
}
