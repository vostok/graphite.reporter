using System.Collections.Generic;

namespace Vostok.Graphite.Client
{
    internal interface IMetricBuffer
    {
        void Add(IEnumerable<Metric> metrics);
        List<Metric> Reset();
    }
}