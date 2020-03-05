namespace EventStore.Core.Commands
{
    public class DeleteSnapshotsCommand
    {
        public string StreamName { get; }

        public DeleteSnapshotsCommand(string streamName)
        {
            StreamName = streamName;
        }
    }
}
