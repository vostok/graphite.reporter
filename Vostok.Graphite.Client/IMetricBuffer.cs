using System.Collections.Generic;

namespace Vstk.Graphite.Client
{
    internal interface IMetricBuffer
    {
        void Add(IEnumerable<Metric> metrics);
        List<Metric> Reset();
    }
}