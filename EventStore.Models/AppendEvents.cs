using System.Collections.Generic;
using System.Linq;

namespace EventStore.Models
{
    public class AppendEvents
    {
        public int ExpectedVersion { get; set; }

        public IEnumerable<NewEvent> Events { get; set; } = Enumerable.Empty<NewEvent>();

        public string StreamName { get; set; }
    }
}
