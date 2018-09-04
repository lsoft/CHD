using System.ServiceModel;

namespace CHD.Service.Wcf
{
    public interface IWcfListener
    {
        void StartListen(
            EndpointAddress endpointAddress
            );

        void StopListen();
    }
}