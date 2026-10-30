namespace TE.DomainStorytellingApplied.Domain;

public enum PaymentStatus
{
    Pending,
    Succeeded,
    Failed
}

public class PaymentAttempt
{
    internal PaymentAttempt()
    {
        Id = Guid.NewGuid();
    }

    public Guid Id { get; }
    private volatile PaymentStatus status = PaymentStatus.Pending;
    public PaymentStatus Status => status;

    internal bool RecordResult(bool succeeded)
    {
        var result = succeeded ? PaymentStatus.Succeeded : PaymentStatus.Failed;
        if (Status == result)
        {
            return false;
        }

        if (Status != PaymentStatus.Pending)
        {
            throw new InvalidOperationException("Betalningsförsöket har redan ett annat resultat.");
        }

        status = result;
        return true;
    }
}
