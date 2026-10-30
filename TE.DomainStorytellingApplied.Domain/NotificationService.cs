namespace TE.DomainStorytellingApplied.Domain;


public class NotificationService
{
    private readonly TextWriter output;

    public NotificationService(TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(output);
        this.output = output;
    }

    private readonly object gate = new();

    public void Send(string recipient, string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(recipient);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        lock (gate)
        {
            output.WriteLine($"Till: {recipient}");
            output.WriteLine(message);
        }
    }
}
