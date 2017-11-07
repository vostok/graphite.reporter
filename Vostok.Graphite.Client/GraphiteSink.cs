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
        private readonly GraphiteClient graphiteClient;

        public GraphiteSink(GraphiteSinkConfig config, ILog log = null)
        {
            log = (log ?? new SilentLog()).ForContext(this);

            metricBuffer = new MetricBuffer(config.MaxMetricBufferCapacity);
            graphiteClient = new GraphiteClient(config.GraphiteHost, config.GraphitePort, log);
            var metricSender = new GraphiteMetricSender(graphiteClient, metricBuffer, log);
            metricSendDaemon = new MetricSendDaemon(metricSender, config, log);
        }

        public void Dispose()
        {
            metricSendDaemon.Dispose();
            graphiteClient.Dispose();
        }

        public void Push(IEnumerable<Metric> metrics)
        {
            metricSendDaemon.Start();
            metricBuffer.Add(metrics);
        }
    }
}