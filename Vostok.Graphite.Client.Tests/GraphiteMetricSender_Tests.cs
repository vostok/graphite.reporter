using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Logging.Logs;
using NSubstitute;

namespace Vostok.Graphite.Client.Tests
{
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