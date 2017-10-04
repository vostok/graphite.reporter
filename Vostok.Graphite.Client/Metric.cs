using System;
using System.Globalization;
using System.Text;

namespace Vostok.Graphite.Client
{
    public class Metric
    {
        public Metric(string name, double value, long timestamp)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            Name = name;
            Value = value;
            Timestamp = timestamp;
        }

        /// <summary>
        /// Имя метрики (набор сегментов, разделенных точкой).
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Значение метрики.
        /// </summary>
        public double Value { get; }

        /// <summary>
        /// Timestamp, соответствующий значению метрики.
        /// </summary>
        public long Timestamp { get; }

        public override string ToString()
        {
            var builder = new StringBuilder(Name.Length + 32);

            builder
                .Append(Name)
                .Append(' ')
                .Append(Value.ToString(CultureInfo.InvariantCulture))
                .Append(' ')
                .Append(Timestamp);

            return builder.ToString();
        }

    }
}