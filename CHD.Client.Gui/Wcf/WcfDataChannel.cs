using System;
using System.Collections.Generic;
using System.ServiceModel;
using CHD.Client.Gui.ViewModel.Main;
using CHD.Client.Gui.ViewModel.Main.Wrapper.Common;
using CHD.Client.Gui.ViewModel.Main.Wrapper.Journal;
using CHD.Client.Gui.ViewModel.Main.Wrapper.Sync;
using CHD.WcfChannel;
using CHD.WcfChannel.Journal;

namespace CHD.Client.Gui.Wcf
{
    public sealed class WcfDataChannel : IDataChannel
    {
        private volatile bool _disposed = false;

        private readonly ChannelFactory<IWcfDataChannel> _factory;
        private readonly IWcfDataChannel _channel;

        public WcfDataChannel(
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

            var endpoint = new EndpointAddress(enpointAddress);

            //создаем соединение
            var binding = bindingProvider.CreateBinding();
            _factory = new ChannelFactory<IWcfDataChannel>(binding);
            _channel = _factory.CreateChannel(endpoint);

            if ((IContextChannel)_channel != null)
            {
                ((IContextChannel)_channel).OperationTimeout = new TimeSpan(0, 10, 0);

            }

            //открываем соединение
            ((System.ServiceModel.IClientChannel)_channel).Open();
        }

        public CommonWrapper GetCommonInfo()
        {
            var wcfr = _channel.GetCommonInfo();

            var result = new CommonWrapper(
                wcfr.WatchFolder
                );

            return
                result;
        }

        public SyncWrapper GetSyncInfo()
        {
            var wcfr = _channel.GetSyncInfo();

            var result = new SyncWrapper(
                wcfr
                );

            return
                result;
        }

        public List<WcfHistorySyncReport> GetJournalInfo(DateTime? since)
        {
            var result = _channel.GetJournalInfo(since);

            return
                result;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                var c = ((System.ServiceModel.IClientChannel)_channel);

                try
                {
                    if (c.State != CommunicationState.Faulted && c.State != CommunicationState.Closed && c.State != CommunicationState.Closing)
                    {
                        c.Close();
                    }
                }
                catch
                {
                    c.Abort();
                }

                try
                {
                    if (_factory.State != CommunicationState.Faulted && _factory.State != CommunicationState.Closed && _factory.State != CommunicationState.Closing)
                    {
                        _factory.Close();
                    }
                }
                catch
                {
                    _factory.Abort();
                }

                _disposed = true;
            }
        }
    }
}
