using System;
using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Logging.Logs;
using NSubstitute;

namespace Vostok.Graphite.Client.Tests
{
    [TestFixture]
    public class MetricSendDaemon_Tests
    {
        private MetricSendDaemon daemon;
        private IMetricSender metricSender;
        private GraphiteSinkConfig config;

        [SetUp]
        public void SetUp()
        {
            config = new GraphiteSinkConfig();
            metricSender = Substitute.For<IMetricSender>();
            daemon = new MetricSendDaemon(metricSender, config, new ConsoleLog());
        }

        [TearDown]
        public void TearDown()
        {
            daemon?.Dispose();
        }

        [Test]
        public void Should_periodically_send_metrics()
        {
            config.SendPeriod = 50.Milliseconds();
            metricSender.SendAsync().Returns(true);
            var called = 0;
            metricSender.When(s => s.SendAsync()).Do(ci => called++);

            daemon.Start();
            ShouldNotPassIn(() => { called.Should().Be(2); }, 70.Milliseconds());
            ShouldPassIn(() => { called.Should().Be(2); }, 100.Milliseconds());
        }

//        [Test]
//        public void Should_increase_sending_period_if_send_failed()
//        {
//            config.SendPeriod = 50.Milliseconds();
//            metricSender.SendAsync().Returns(false);
//        }

        private void ShouldPassIn(Action action, TimeSpan time)
        {
            var finish = DateTime.UtcNow + time;
            while (DateTime.UtcNow < finish)
            {
                try
                {
                    action();
                }
                catch (Exception)
                {
                    Thread.Sleep(5);
                    continue;
                }

                return;
            }

            Assert.Fail("Action didn't complete in specified timeout");
        }

        private void ShouldNotPassIn(Action action, TimeSpan time)
        {
            var finish = DateTime.UtcNow + time;
            while (DateTime.UtcNow < finish)
            {
                try
                {
                    action();
                }
                catch (Exception)
                {
                    Thread.Sleep(5);
                    continue;
                }
                Assert.Fail("Action completed in specified timeout");
            }
        }
    }

    [TestFixture]
    public class GraphiteMetricSender_Tests
    {
        private GraphiteMetricSender sender;
        private IGraphiteClient client;
        private IMetricBuffer buffer;

        [SetUp]
        public void SetUp()
        {
            client = Substitute.For<IGraphiteClient>();
            buffer = Substitute.For<IMetricBuffer>();
            sender = new GraphiteMetricSender(client, buffer, new ConsoleLog());
        }

        [Test]
        public void When_reset_returns_null_should_not_send_anything_and_return_true()
        {
            buffer.Reset().Returns((List<Metric>) null);

            var result = sender.SendAsync().GetAwaiter().GetResult();

            result.Should().BeTrue();
            client.DidNotReceive().SendAsync(Arg.Any<IEnumerable<Metric>>());
        }

        [Test]
        public void Should_send_metrics_with_client()
        {
            var metrics = new List<Metric>();
            buffer.Reset().Returns(metrics);
            client.SendAsync(Arg.Is(metrics)).Returns(true);

            var result = sender.SendAsync().GetAwaiter().GetResult();

            result.Should().BeTrue();
            buffer.Received(1).Reset();
            client.Received(1).SendAsync(Arg.Is(metrics));
            buffer.DidNotReceive().Add(Arg.Is(metrics));
        }

        [Test]
        public void Should_return_metrics_back_to_buffer_if_send_failed()
        {
            var metrics = new List<Metric>();
            buffer.Reset().Returns(metrics);
            client.SendAsync(Arg.Is(metrics)).Returns(false);

            var result = sender.SendAsync().GetAwaiter().GetResult();

            result.Should().BeFalse();
            buffer.Received(1).Reset();
            client.Received(1).SendAsync(Arg.Is(metrics));
            buffer.Received(1).Add(Arg.Is(metrics));
        }
    }
}