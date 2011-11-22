using System.Collections.Generic;

namespace Rebus.RavenDB
{
    public class RebusSubscription
    {
        public string Id { get; set; }
        public List<string> Endpoints { get; set; } 
    }
}