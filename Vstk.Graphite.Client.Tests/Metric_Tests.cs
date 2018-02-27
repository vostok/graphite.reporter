using System;
using NUnit.Framework;

namespace Vstk.Graphite.Client.Tests
{
    [TestFixture]
    public class Metric_Tests
    {
        [TestCase("Vstk.TestApp.rps")]
        [TestCase("Vstk")]
        [TestCase("Vstk.123.testapp")]
        [TestCase("ABC")]
        [TestCase("a_b")]
        public void Can_create_metric_with_valid_name(string name)
        {
            // ReSharper disable once UnusedVariable
            var metric = new Metric(name, 123, 123);
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("a..b")]
        [TestCase(".b")]
        [TestCase("b.")]
        [TestCase("абвгд")]
        public void Validation_fails_if_metric_name_is_incorrect(string name)
        {
            Assert.Throws<ArgumentException>(
                () =>
                {
                    new Metric(name, 123, 123);
                });
        }
    }
}