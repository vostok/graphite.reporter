using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Vostok.Graphite.Client.Tests
{
    [TestFixture]
    public class MetricBuffer_Tests
    {
        [Test]
        public void Reset_returns_what_was_added()
        {
            var buffer = new MetricBuffer(10);
            var metrics1 = new []{GetMetric(1), GetMetric(2)};
            var metrics2 = new[] {GetMetric(3), GetMetric(4)};

            buffer.Add(metrics1);
            buffer.Add(metrics2);
            var result = buffer.Reset();

            result.Should().BeEquivalentTo(metrics1.Concat(metrics2));
        }

        [Test]
        public void Can_not_add_more_than_capacity()
        {
            var buffer = new MetricBuffer(1);
            var metrics = new[] {GetMetric(1), GetMetric(2)};

            buffer.Add(metrics);
            var result = buffer.Reset();

            result.Count.Should().Be(1);
        }

        [Test]
        public void Reset_should_return_null_when_there_are_no_metrics_in_buffer()
        {
            var buffer = new MetricBuffer(10);

            var result = buffer.Reset();

            result.Should().BeNull();
        }

        [Test]
        public void Full_capacity_is_available_after_reset()
        {
            var buffer = new MetricBuffer(2);

            buffer.Add(new[]{ GetMetric(1)});
            buffer.Reset();
            var metrics = new[] {GetMetric(2), GetMetric(3)};
            buffer.Add(metrics);
            var result = buffer.Reset();

            result.Should().BeEquivalentTo(metrics);
        }

        private Metric GetMetric(int id)
        {
            return new Metric($"metric.{id}", 100, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
        }
    }
}
