namespace Shared.Data;

public partial class Message
{
    public int Id { get; set; }
    public Guid ClientId { get; set; }
    public int LamportCounter { get; set; }
    public Dictionary<Guid, int> VectorClock { get; set; }
    public string? Sender { get; set; }

    public string? MessageText { get; set; }

    public string? ImagePath { get; set; }

    public DateTime? Timestamp { get; set; }

    public virtual ICollection<MessageContainerLocation> MessageContainerLocations { get; set; } = new List<MessageContainerLocation>();
}
