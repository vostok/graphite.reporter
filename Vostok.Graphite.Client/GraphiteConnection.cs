using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Vostok.Graphite.Client
{
    internal class GraphiteConnection : IDisposable
    {
        private readonly TcpClient client;
        private readonly StreamWriter writer;

        public GraphiteConnection(string host, int port)
        {
            client = new TcpClient
            {
                SendTimeout = 5000,
                ReceiveTimeout = 5000,
                NoDelay = true,
            };
            client.Connect(host, port);
            writer = new StreamWriter(client.GetStream(), new UTF8Encoding(false), 32 * 1024);
        }

        public async Task SendAsync(IEnumerable<Metric> metrics)
        {
            foreach (var metric in metrics)
            {
                await writer.WriteLineAsync(metric.ToString()).ConfigureAwait(false);
            }
            await writer.FlushAsync().ConfigureAwait(false);
        }

        public void Dispose()
        {
            client.Close();
        }
    }
}