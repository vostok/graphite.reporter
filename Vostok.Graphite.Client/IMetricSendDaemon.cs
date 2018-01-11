using System;

namespace Vostok.Graphite.Client
{
    internal interface IMetricSendDaemon : IDisposable
    {
        void Start();
    }
}