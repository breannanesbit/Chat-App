//using System.Diagnostics.Metrics;
using System.Diagnostics.Metrics;

namespace ChatAppAPI
{
    public static class Metrics
    {
        public static Meter m = new Meter("ChatAppApi");

        public static Counter<int> ApiCalls = m.CreateCounter<int>("api_request_counter");
        public static Counter<int> FailedCalls = m.CreateCounter<int>("Failed_api_calls");
        public static Counter<int> SuccessCalls = m.CreateCounter<int>("successful_api_calls");

    }
}
