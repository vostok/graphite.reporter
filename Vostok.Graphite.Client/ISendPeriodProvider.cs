using System;

namespace Vostok.Graphite.Client
{
    internal interface ISendPeriodProvider
    {
        TimeSpan GetNext(bool requestSucceed);
    }
}