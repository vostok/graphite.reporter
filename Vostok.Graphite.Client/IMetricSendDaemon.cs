using System;

namespace Vstk.Graphite.Client
{
    internal interface IMetricSendDaemon : IDisposable
    {
        void Start();
    }
}