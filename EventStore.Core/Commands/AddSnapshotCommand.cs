namespace EventStore.Core.Commands
{
    public class AddSnapshotCommand
    {
        public string Data { get; }

        public string Description { get; }

        public string StreamName { get; }

        public AddSnapshotCommand(string streamName, string data, string description)
        {
            StreamName = streamName;
            Data = data;
            Description = description;
        }
    }
}
