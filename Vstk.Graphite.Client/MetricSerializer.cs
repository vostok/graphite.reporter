using System.Globalization;
using System.Text;

namespace Vstk.Graphite.Client
{
    internal class MetricSerializer
    {
        private readonly StringBuilder builder;

        public MetricSerializer()
        {
            builder = new StringBuilder();
        }

        public string Serialize(Metric metric)
        {
            builder
                .Append(metric.Name)
                .Append(' ')
                .Append(metric.Value.ToString(CultureInfo.InvariantCulture))
                .Append(' ')
                .Append(metric.Timestamp);
            var result = builder.ToString();
            builder.Clear();
            return result;
        }
    }
}