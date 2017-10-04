using System.Collections.Generic;
using System.Threading.Tasks;
using Vostok.Commons.Collections;

namespace Vostok.Graphite.Client
{
    public class GraphiteClient
    {
        private readonly IPool<GraphiteConnection> connectionPool;

        public GraphiteClient(string host, int port)
        {
            connectionPool = new UnlimitedLazyPool<GraphiteConnection>(() => new GraphiteConnection(host, port));
        }

        public async Task SendAsync(IEnumerable<Metric> metrics)
        {
            using (var poolHandle = connectionPool.AcquireHandle())
            {
                var connection = (GraphiteConnection)poolHandle;
                await connection.SendAsync(metrics).ConfigureAwait(false);
            }
        }

        public void Dispose()
        {
            connectionPool.Dispose();
        }
    }
}