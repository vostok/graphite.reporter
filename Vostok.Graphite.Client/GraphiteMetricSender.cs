using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Vostok.Logging;

namespace Vostok.Graphite.Client
{
    internal class GraphiteMetricSender : IMetricSender
    {
        private readonly IGraphiteClient client;
        private readonly IMetricBuffer buffer;
        private readonly ILog log;

        public GraphiteMetricSender(
            IGraphiteClient client,
            IMetricBuffer buffer,
            ILog log)
        {
            this.client = client;
            this.buffer = buffer;
            this.log = log;
        }

        public async Task<bool> SendAsync()
        {
            var sw = Stopwatch.StartNew();

            var toSend = buffer.Reset();
            if (toSend == null)
            {
                return true;
            }

            var isSuccessful = await client.SendAsync(toSend).ConfigureAwait(false);

            if (!isSuccessful)
            {
                buffer.Add(toSend);
            }

            LogSendAttempt(isSuccessful, toSend, sw.Elapsed);

            return isSuccessful;
        }

        private void LogSendAttempt(bool isSuccessful, List<Metric> toSend, TimeSpan elapsed)
        {
            if (isSuccessful)
            {
                log.Debug($"Successfully sent {toSend.Count} metrics to graphite in {elapsed:c}");
            }
            else
            {
                log.Warn($"Failed to send {toSend.Count} metrics to graphite in {elapsed:c}");
            }
        }
    }
}