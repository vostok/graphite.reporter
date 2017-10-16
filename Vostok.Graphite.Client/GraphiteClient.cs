using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vostok.Commons.Collections;

namespace Vostok.Graphite.Client
{
    public class GraphiteClient : IDisposable
    {
        private readonly IPool<GraphiteConnection> connectionPool;

        public GraphiteClient(string host, int port)
        {
            connectionPool = new UnlimitedLazyPool<GraphiteConnection>(() => new GraphiteConnection(host, port));
        }

        public async Task SendAsync(IEnumerable<Metric> metrics)
        {
            var poolHandle = connectionPool.AcquireHandle();
            var connection = poolHandle.Resource;
            try
            {
                await connection.SendAsync(metrics).ConfigureAwait(false);
            }
            catch (Exception)
            {
                //@ezsilmar Should not return connection to pool in this case
                connection.Dispose();
                throw;
            }
            poolHandle.Dispose();
        }

        public void Dispose()
        {
            connectionPool.Dispose();
        }
    }
}