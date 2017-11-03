using System;
using System.Collections.Generic;
using Vostok.Logging;
using Vostok.Logging.Logs;

namespace Vostok.Graphite.Client
{
    public class GraphiteSink : IGraphiteSink, IDisposable
    {
        private readonly IMetricBuffer metricBuffer;
        private readonly IMetricSendDaemon metricSendDaemon;

        public GraphiteSink(GraphiteSinkConfig config, ILog log = null)
        {
            log = (log ?? new SilentLog()).ForContext(this);

            metricBuffer = new MetricBuffer(config.MaxMetricBufferCapacity);
            var graphiteClient = new GraphiteClient(config.GraphiteHost, config.GraphitePort, log);
            metricSendDaemon = new MetricSendDaemon(metricBuffer, graphiteClient, config, log);
        }

        public void Dispose()
        {
            metricSendDaemon.Dispose();
        }

        public void Push(IEnumerable<Metric> metrics)
        {
            metricSendDaemon.Start();
            metricBuffer.Add(metrics);
        }
    }
}