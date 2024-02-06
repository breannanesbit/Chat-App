namespace Shared
{
    public class Message
    {
        public int Id { get; set; }
        public string Sender { get; set; }
        public string MessageText { get; set; }
        public DateTime Timestamp { get; set; }
        public string ImagePath { get; set; }
        public int ContainerLocationId { get; set; }
    }

}
