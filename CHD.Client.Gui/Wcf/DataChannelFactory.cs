using System;

namespace CHD.Client.Gui.Wcf
{
    public sealed class DataChannelFactory : IDataChannelFactory
    {
        private readonly string _enpointAddress;
        private readonly IBindingProvider _bindingProvider;

        public DataChannelFactory(
            string enpointAddress,
            IBindingProvider bindingProvider
            )
        {
            if (enpointAddress == null)
            {
                throw new ArgumentNullException("enpointAddress");
            }
            if (bindingProvider == null)
            {
                throw new ArgumentNullException("bindingProvider");
            }
            _enpointAddress = enpointAddress;
            _bindingProvider = bindingProvider;
        }

        public IDataChannel OpenChannel()
        {
            var result = new WcfDataChannel(
                _enpointAddress,
                _bindingProvider
                );

            return
                result;
        }
    }
}