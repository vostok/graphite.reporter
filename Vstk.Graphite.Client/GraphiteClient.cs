using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vstk.Commons.Collections;
using Vstk.Logging;

namespace Vstk.Graphite.Client
{
    public class GraphiteClient : IGraphiteClient, IDisposable
    {
        private readonly ILog log;
        private readonly IPool<GraphiteConnection> connectionPool;

        public GraphiteClient(string host, int port, ILog log)
        {
            this.log = log;
            connectionPool = new UnlimitedLazyPool<GraphiteConnection>(() => new GraphiteConnection(host, port));
        }

        public async Task<bool> SendAsync(IReadOnlyCollection<Metric> metrics)
        {
            PoolHandle<GraphiteConnection> handle;
            while ((handle = TryGetHandle()) != null)
            {
                var connection = handle.Resource;
                try
                {
                    await connection.SendAsync(metrics).ConfigureAwait(false);
                    handle.Dispose();
                    return true;
                }
                catch (Exception ex)
                {
                    //@ezsilmar Should not return connection to pool in this case
                    connection.Dispose();
                    log.Warn("Send failed", ex);
                }
            }

            return false;
        }

        private PoolHandle<GraphiteConnection> TryGetHandle()
        {
            try
            {
                return connectionPool.AcquireHandle();
            }
            catch (Exception ex)
            {
                log.Warn("Connection error", ex);
                return null;
            }
        }

        public void Dispose()
        {
            connectionPool.Dispose();
        }
    }
}