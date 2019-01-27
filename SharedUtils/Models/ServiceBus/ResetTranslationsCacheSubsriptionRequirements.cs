using System;
using System.Collections.Generic;
using System.Text;
using MartinParkerAngularCV.SharedUtils.Enums;
using MartinParkerAngularCV.SharedUtils.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace MartinParkerAngularCV.SharedUtils.Models.ServiceBus
{
    public class ResetTranslationsCacheSubsriptionRequirements : ISerivceBusTopicSubscription
    {
        public ResetTranslationsCacheSubsriptionRequirements(ServiceBusTopic topic, string context, IDistributedCache distributedCache)
        {
            Topic = topic;
            Context = context;
            DistributedCache = distributedCache;
        }

        public ServiceBusTopic Topic { get; }
        public string Context { get; }
        public IDistributedCache DistributedCache { get; }
    }
}
