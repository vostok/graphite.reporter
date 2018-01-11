using Vostok.Airlock;
using Vostok.Graphite.Client;
using Vostok.Metrics;

namespace Vostok.Graphite.Reporter
{
    public class GraphiteMetricReporter : IMetricEventReporter
    {
        private readonly IGraphiteSink sink;
        private readonly string routingKeyPrefix;
        private readonly MetricConverter metricConverter;

        public GraphiteMetricReporter(IGraphiteSink sink, string routingKeyPrefix)
        {
            this.sink = sink;
            this.routingKeyPrefix = routingKeyPrefix;
            metricConverter = new MetricConverter(new GraphiteNameBuilder());
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