using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vostok.Graphite.Client
{
    public interface IGraphiteClient
    {
        Task<bool> SendAsync(IEnumerable<Metric> metrics);
    }
}