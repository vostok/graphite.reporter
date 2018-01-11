using System.Collections.Generic;

namespace Vostok.Graphite.Client
{
    public interface IGraphiteSink
    {
        void Push(IEnumerable<Metric> metrics);
    }
}