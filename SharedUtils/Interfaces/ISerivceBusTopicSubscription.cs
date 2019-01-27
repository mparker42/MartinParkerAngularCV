using MartinParkerAngularCV.SharedUtils.Enums;

namespace MartinParkerAngularCV.SharedUtils.Interfaces
{
    public interface ISerivceBusTopicSubscription
    {
        ServiceBusTopic Topic { get; }
        string Context { get; }
    }
}
