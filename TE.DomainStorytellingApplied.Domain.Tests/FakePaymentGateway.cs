namespace TE.DomainStorytellingApplied.Domain.Tests;

public class FakePaymentGateway : IPaymentGateway
{
    public PaymentResult ResultToReturn { get; set; } = PaymentResult.Success;

    public int CallCount { get; private set; }

    public decimal? LastAmount { get; private set; }

    public PaymentResult Pay(Id bookingId, decimal amount)
    {
        CallCount++;
        LastAmount = amount;
        return ResultToReturn;
    }
}
