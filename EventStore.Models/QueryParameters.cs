namespace EventStore.Models
{
    public class QueryParameters
    {
        public string StreamName { get; set; }

        public int? StartAtVersion { get; set; }
    }
}
