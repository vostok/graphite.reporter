using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Commons.Synchronization;
using Vostok.Commons.Utilities;
using Vostok.Logging;

namespace Vostok.Graphite.Client
{
    internal class MetricSendDaemon : IMetricSendDaemon
    {
        private readonly IMetricBuffer metricBuffer;
        private readonly IGraphiteClient graphiteClient;
        private readonly GraphiteSinkConfig config;
        private readonly ILog log;

        private const int NotStarted = 0;
        private const int Working = 1;
        private const int Disposed = 2;
        private readonly AtomicInt state;
        private readonly object initializationSync;

        private CancellationTokenSource senderRoutineCts;
        private Task senderRoutine;

        public MetricSendDaemon(
            IMetricBuffer metricBuffer,
            IGraphiteClient graphiteClient,
            GraphiteSinkConfig config,
            ILog log)
        {
            this.metricBuffer = metricBuffer;
            this.graphiteClient = graphiteClient;
            this.config = config;
            this.log = log;

            this.state = new AtomicInt(NotStarted);
            initializationSync = new object();
        }

        public void Start()
        {
            if (state.TryIncreaseTo(Working))
            {
                lock (initializationSync)
                {
                    if (state == Working)
                    {
                        senderRoutineCts = new CancellationTokenSource();
                        senderRoutine = Task.Run(SenderRoutine);
                    }
                }
            }
        }

        private async Task SenderRoutine()
        {
            var sendPeriod = config.SendPeriod;

            while (state == Working)
            {
                await WaitForNextSend(sendPeriod);

                var (success, elapsed) = await SendAsync();
                
                if (success)
                {
                    sendPeriod = config.SendPeriod;
                }
                else
                {
                    sendPeriod = IncreaseSendPeriod(sendPeriod);
                    LogIncreasedSendPeriod(sendPeriod);
                }
                sendPeriod = TimeSpan.FromTicks(Math.Max(0, (sendPeriod - elapsed).Ticks));
            }
        }

        private async Task<(bool, TimeSpan)> SendAsync()
        {
            var snapshot = metricBuffer.Reset();
            if (snapshot == null)
            {
                return (true, TimeSpan.Zero);
            }

            var sw = Stopwatch.StartNew();

            var success = await graphiteClient.SendAsync(snapshot).ConfigureAwait(false);
            if (!success)
            {
                metricBuffer.Add(snapshot);
            }

            var elapsed = sw.Elapsed;
            LogSendAttempt(success, elapsed, snapshot.Count);

            return (success, elapsed);
        }

        private void LogSendAttempt(bool success, TimeSpan elapsed, int count)
        {
            if (success)
            {
                log.Debug($"Succesfully sent {count} metrics to Graphite. Elapsed: {elapsed:c}");
            }
            else
            {
                log.Warn($"Failed to send metrics. Snapshot length: {count}. Elapsed: {elapsed:c}");
            }
        }

        private void LogIncreasedSendPeriod(TimeSpan sendPeriod)
        {
            log.Debug($"Increased send period to {sendPeriod:c}");
        }

        private TimeSpan IncreaseSendPeriod(TimeSpan sendPeriod)
        {
            var multiplier = 1.5 + 0.5*ThreadSafeRandom.NextDouble();
            var newPeriodTicks = (long) (sendPeriod.Ticks*multiplier);

            newPeriodTicks = Math.Min(newPeriodTicks, config.SendPeriodCap.Ticks);
            newPeriodTicks = Math.Max(newPeriodTicks, config.SendPeriod.Ticks);
            return TimeSpan.FromTicks(newPeriodTicks);
        }

        private async Task WaitForNextSend(TimeSpan sendPeriod)
        {
            try
            {
                await Task.Delay(sendPeriod, senderRoutineCts.Token);
            }
            catch (OperationCanceledException)
            {}
        }

        public void Dispose()
        {
            if (state.TryIncreaseTo(Disposed))
            {
                lock (initializationSync)
                {
                    senderRoutineCts?.Cancel();
                    senderRoutine?.GetAwaiter().GetResult();
                    SendAsync().GetAwaiter().GetResult();
                }
            }
        }
    }
}