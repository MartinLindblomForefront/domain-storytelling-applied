using TE.DomainStorytellingApplied.Domain;

namespace TE.DomainStorytellingApplied.Domain.Tests;

internal sealed class StubPaymentGateway(params bool[] results) : IPaymentGateway
{
    private readonly Queue<bool> results = new(results);
    public List<Guid> Attempts { get; } = [];
    public bool ThrowOnPayment { get; set; }

    public bool Pay(Guid paymentAttemptId)
    {
        Attempts.Add(paymentAttemptId);
        if (ThrowOnPayment)
        {
            throw new InvalidOperationException("Payment outcome unknown.");
        }

        return results.Dequeue();
    }
}
