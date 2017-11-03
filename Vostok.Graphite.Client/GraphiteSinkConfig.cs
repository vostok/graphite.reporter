using System;

namespace Vostok.Graphite.Client
{
    public class GraphiteSinkConfig
    {
        public int MaxMetricBufferCapacity { get; set; }
        public TimeSpan SendPeriod { get; set; }
        public TimeSpan SendPeriodCap { get; set; }
        public string GraphiteHost { get; set; }
        public int GraphitePort { get; set; }
    }
}