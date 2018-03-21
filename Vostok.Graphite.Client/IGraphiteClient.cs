using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vstk.Graphite.Client
{
    public interface IGraphiteClient
    {
        Task<bool> SendAsync(IReadOnlyCollection<Metric> metrics);
    }
}