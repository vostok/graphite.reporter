using System;
using System.Collections.Generic;
using Vstk.Graphite.Client;
using Vstk.Logging;
using Vstk.Metrics;

namespace Vstk.Graphite.Reporter
{
    public class MetricConverter
    {
        private readonly IGraphiteNameBuilder graphiteNameBuilder;
        private readonly ILog log;

        public MetricConverter(IGraphiteNameBuilder graphiteNameBuilder, ILog log)
        {
            this.graphiteNameBuilder = graphiteNameBuilder;
            this.log = log;
        }

        public IEnumerable<Metric> Convert(string routingKey, MetricEvent metricEvent)
        {
            var prefix = graphiteNameBuilder.BuildPrefix(routingKey, metricEvent.Tags);
            foreach (var pair in metricEvent.Values)
            {
                var name = graphiteNameBuilder.BuildName(prefix, pair.Key);
                var timestamp = metricEvent.Timestamp.ToUnixTimeSeconds();
                Metric metric = null;
                try
                {
                    metric = new Metric(name, pair.Value, timestamp);
                }
                catch (Exception e)
                {
                    log.Error(e);
                }

                if (metric != null)
                    yield return metric;
            }
        }
    }
}