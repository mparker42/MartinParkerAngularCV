using MartinParkerAngularCV.SharedUtils.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MartinParkerAngularCV.SharedUtils.Interfaces
{
    public interface IServiceBusTopicMessage
    {
        ServiceBusTopic Topic { get; }
    }
}
