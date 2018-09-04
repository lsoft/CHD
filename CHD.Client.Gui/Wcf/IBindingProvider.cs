using System.ServiceModel.Channels;

namespace CHD.Client.Gui.Wcf
{
    public interface IBindingProvider
    {
        CustomBinding CreateBinding();
    }
}