using FluentAssertions;
using NUnit.Framework;
using Vstk.Logging.Logs;

namespace Vstk.Graphite.Client.Tests
{
    [TestFixture]
    public class SendPeriodProvider_Tests
    {
        private SendPeriodProvider provider;
        private GraphiteSinkConfig config;

        [SetUp]
        public void SetUp()
        {
            config = new GraphiteSinkConfig();
            provider = new SendPeriodProvider(config, new ConsoleLog());
        }

        [Test]
        public void If_send_is_successful_should_return_send_period()
        {
            config.SendPeriod = 50.Milliseconds();

            var first = provider.GetNext(true);
            var second = provider.GetNext(true);

            first.Should().Be(config.SendPeriod);
            second.Should().Be(config.SendPeriod);
        }

        [Test]
        public void If_send_failed_should_increase_period()
        {
            config.SendPeriod = 100.Milliseconds();

            var first = provider.GetNext(false);
            var second = provider.GetNext(false);

            first.Should().BeGreaterOrEqualTo(1.5*config.SendPeriod).And.BeLessOrEqualTo(2*config.SendPeriod);
            second.Should().BeGreaterOrEqualTo(1.5*first).And.BeLessOrEqualTo(2*first);
        }

        [Test]
        public void Send_period_cant_be_more_than_cap()
        {
            config.SendPeriod = 100.Milliseconds();
            config.SendPeriodCap = 120.Milliseconds();

            var first = provider.GetNext(false);
            var second = provider.GetNext(false);

            first.Should().Be(config.SendPeriodCap);
            second.Should().Be(config.SendPeriodCap);
        }

        [Test]
        public void Send_period_resets_to_default_after_success()
        {
            config.SendPeriod = 100.Milliseconds();

            var first = provider.GetNext(false);
            var second = provider.GetNext(true);

            first.Should().BeGreaterThan(config.SendPeriod);
            second.Should().Be(config.SendPeriod);
        }

        [TestCase(2, 1)]
        [TestCase(1, 2)]
        public void Send_period_setting_is_hot(int firstSec, int secondSec)
        {
            config.SendPeriod = firstSec.Seconds();

            var first = provider.GetNext(true);
            config.SendPeriod = secondSec.Seconds();
            var second = provider.GetNext(true);

            first.Should().Be(firstSec.Seconds());
            second.Should().Be(secondSec.Seconds());
        }
    }
}