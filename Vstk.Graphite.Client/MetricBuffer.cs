using System.Collections.Generic;

namespace Vstk.Graphite.Client
{
    internal class MetricBuffer : IMetricBuffer
    {
        private readonly int maxCapacity;
        private readonly object syncObject = new object();
        private List<Metric> buffer;

        public MetricBuffer(int maxCapacity)
        {
            this.maxCapacity = maxCapacity;
        }

        public void Add(IEnumerable<Metric> metrics)
        {
            lock (syncObject)
            {
                if (buffer == null)
                {
                    buffer = new List<Metric>();
                }

                foreach (var metric in metrics)
                {
                    if (buffer.Count >= maxCapacity)
                    {
                        return;
                    }

                    buffer.Add(metric);
                }
            }
        }

        public List<Metric> Reset()
        {
            lock (syncObject)
            {
                var snapshot = buffer;
                buffer = null;
                return snapshot;
            }
        }
    }
}