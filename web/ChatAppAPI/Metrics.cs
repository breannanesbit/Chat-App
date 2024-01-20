//using System.Diagnostics.Metrics;
using System.Diagnostics.Metrics;

namespace ChatAppAPI
{
    public static class Metrics
    {
        public static Meter m = new Meter("ChatAppApi");

        public static Counter<int> ApiCalls = m.CreateCounter<int>("api_request_counter");
        // public static Counter<int> MoviesCalls = m.CreateCounter<int>("movie calls");

    }
}
