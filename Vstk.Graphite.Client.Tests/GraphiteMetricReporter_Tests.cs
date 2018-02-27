using System.Threading;
using NUnit.Framework;
using Vstk.Airlock;
using Vstk.Commons.Extensions.UnitConvertions;
using Vstk.Graphite.Reporter;
using Vstk.Logging.Logs;
using Vstk.Metrics;

namespace Vstk.Graphite.Client.Tests
{
    public class GraphiteMetricReporter_Tests
    {
        [Test, Ignore("Manual test")]
        public void Test()
        {
            var prefix = RoutingKey.CreatePrefix("vstk", "ci", "test");
            var log = new ConsoleLog();
            var config = new GraphiteSinkConfig()
            {
                GraphiteHost = "localhost",
                GraphitePort = 2003
            };
            var graphiteSink = new GraphiteSink(config, log);
            var reporter = new GraphiteMetricReporter(graphiteSink, prefix, log);

            IMetricScope rootMetricScope = new RootMetricScope(
                new MetricConfiguration
                {
                    Reporter = reporter
                });
            var timeSpan = 10.Seconds();
            var counter = rootMetricScope.Counter(timeSpan, "cnt");
            MetricClocks.Get(timeSpan).Start();
            for (var i = 0; i < 3; i++)
            {
                counter.Add(11);
                Thread.Sleep(1000);
            }
            Thread.Sleep(20000);
        }
    }
}