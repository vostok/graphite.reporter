using System.Threading.Tasks;

namespace Vstk.Graphite.Client
{
    internal interface IMetricSender
    {
        Task<bool> SendAsync();
    }
}