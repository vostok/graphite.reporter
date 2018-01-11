using System;

namespace Vostok.Graphite.Client
{
    public class Metric
    {
        public Metric(string name, double value, long timestamp)
        {
            ValidateMetricName(name);

            Name = name;
            Value = value;
            Timestamp = timestamp;
        }

        /// <summary>
        /// <para>Metric name contains of several segments</para>
        /// <para>Segments are separated by dot '.'</para>
        /// <para>Only a-zA-Z0-9_ chars are valid inside a segment</para>
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Metric value
        /// </summary>
        public double Value { get; }

        /// <summary>
        /// Timestamp in Unix format
        /// </summary>
        public long Timestamp { get; }

        public override string ToString()
        {
            return new MetricSerializer().Serialize(this);
        }

        private static void ValidateMetricName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Metric name can't be null or empty");
            }

            if (name[0] == '.' || name[name.Length - 1] == '.')
            {
                throw new ArgumentException($"Metric name can't start or end with dot: {name}");
            }

            var isSegmentStart = true;
            for (var i = 0; i < name.Length; i++)
            {
                if (name[i] == '.')
                {
                    if (isSegmentStart)
                    {
                        throw new ArgumentException($"Metric can't have a segment with zero length. Position ${i}: {name}");
                    }
                    isSegmentStart = true;
                    continue;
                }

                var isValidSegmentChar =
                    name[i] >= 'a' && name[i] <= 'z'
                    || name[i] >= 'A' && name[i] <= 'Z'
                    || name[i] >= '0' && name[i] <= '9'
                    || name[i] == '_';
                if (!isValidSegmentChar)
                {
                    throw new ArgumentException($"Invalid char '{name[i]}' in metric name. Position {i}: {name}");
                }
                isSegmentStart = false;
            }
        }
    }
}