using System.Threading.Tasks;

namespace Vostok.Graphite.Client
{
    internal interface IMetricSender
    {
        Task<bool> SendAsync();
    }
}