using Vstk.Airlock;
using Vstk.Graphite.Client;
using Vstk.Logging;
using Vstk.Metrics;

namespace Vstk.Graphite.Reporter
{
    public class GraphiteMetricReporter : IMetricEventReporter
    {
        private readonly IGraphiteSink sink;
        private readonly string routingKeyPrefix;
        private readonly MetricConverter metricConverter;

        public GraphiteMetricReporter(IGraphiteSink sink, string routingKeyPrefix, ILog log)
        {
            this.sink = sink;
            this.routingKeyPrefix = routingKeyPrefix;
            metricConverter = new MetricConverter(new GraphiteNameBuilder(), log);
        }
        public void SendEvent(MetricEvent metricEvent)
        {
            var prefix = RoutingKey.TryAddSuffix(routingKeyPrefix, RoutingKey.AppEventsSuffix);
            var metrics = metricConverter.Convert(prefix, metricEvent);
            sink.Push(metrics);
        }

        public void SendMetric(MetricEvent metricEvent)
        {
            var prefix = RoutingKey.TryAddSuffix(routingKeyPrefix, RoutingKey.MetricsSuffix);
            var metrics = metricConverter.Convert(prefix, metricEvent);
            sink.Push(metrics);
        }
    }
}