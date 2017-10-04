using System;
using System.Linq;
using System.Threading.Tasks;

namespace Vostok.Graphite.Client.Example
{
    class Program
    {
        static void Main()
        {
            GraphiteClient graphiteClient = null;
            try
            {
                graphiteClient = new GraphiteClient("graphite-relay.skbkontur.ru", 2003);

                Parallel.For(0, 10, (i, s) =>
                {
                    try
                    {
                        Send(graphiteClient, i);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.WriteLine("Success");
            Console.ReadLine();
            graphiteClient?.Dispose();
        }

        private static void Send(GraphiteClient graphiteClient, int threadNumber)
        {
            var random = new Random();
            const int metricsCount = 10;
            const int periodPerSeconds = 5;
            var startTimestamp = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 01, 01)).TotalSeconds;
            for (var j = 0; j < 5; j++)
            {
                var timestamp = startTimestamp - periodPerSeconds*metricsCount*j;

                var metrics = Enumerable.Range(0, metricsCount)
                    .Select(i => new Metric("Vostok.GraphiteClient_Example.Thread" + threadNumber, random.Next(10), (long) (timestamp + i*5)));
                graphiteClient.SendAsync(metrics).Wait();
            }
        }
    }
}
