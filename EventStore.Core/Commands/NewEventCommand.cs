namespace EventStore.Core.Commands
{
    public class NewEventCommand
    {
        public string Type { get; }

        public string Data { get; }

        public NewEventCommand(string type, string data)
        {
            Type = type;
            Data = data;
        }
    }
}
