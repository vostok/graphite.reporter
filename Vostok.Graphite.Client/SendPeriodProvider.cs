using System;
using Vostok.Commons.Utilities;
using Vostok.Logging;

namespace Vostok.Graphite.Client
{
    internal class SendPeriodProvider : ISendPeriodProvider
    {
        private readonly GraphiteSinkConfig config;
        private readonly ILog log;
        private TimeSpan previousPeriod;

        public SendPeriodProvider(GraphiteSinkConfig config, ILog log)
        {
            this.config = config;
            this.log = log;
            previousPeriod = TimeSpan.Zero;
        }

        public TimeSpan GetNext(bool requestSucceed)
        {
            previousPeriod = TimeSpanExtensions.Max(previousPeriod, config.SendPeriod);

            if (requestSucceed)
            {
                return previousPeriod = config.SendPeriod;
            }

            var multiplier = 1.5 + 0.5*ThreadSafeRandom.NextDouble();
            var period = previousPeriod.Multiply(multiplier);
            period = TimeSpanExtensions.Max(period, config.SendPeriod);
            period = TimeSpanExtensions.Min(period, config.SendPeriodCap);
            LogIncreasedSendPeriod(period);

            return previousPeriod = period;
        }

        private void LogIncreasedSendPeriod(TimeSpan sendPeriod)
        {
            log.Debug($"Increased send period to {sendPeriod:c}");
        }
    }
}