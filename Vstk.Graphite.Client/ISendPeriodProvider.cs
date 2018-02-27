using System;

namespace Vstk.Graphite.Client
{
    internal interface ISendPeriodProvider
    {
        TimeSpan GetNext(bool requestSucceed);
    }
}