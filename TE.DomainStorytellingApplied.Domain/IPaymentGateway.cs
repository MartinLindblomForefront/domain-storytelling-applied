namespace TE.DomainStorytellingApplied.Domain;

public interface IPaymentGateway
{

    bool Pay(Guid paymentAttemptId);
}
