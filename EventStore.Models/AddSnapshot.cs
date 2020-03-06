namespace EventStore.Models
{
    public class AddSnapshot
    {
        public string Data { get; set; }

        public string Description { get; set; }

        public string StreamName { get; set; }
    }
}
