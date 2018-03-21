using System.Collections.Generic;

namespace Vstk.Graphite.Client
{
    public interface IGraphiteSink
    {
        void Push(IEnumerable<Metric> metrics);
    }
}