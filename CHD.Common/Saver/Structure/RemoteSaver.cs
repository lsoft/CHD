using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CHD.Common.Letter;
using CHD.Common.Native;
using CHD.Common.Others;
using CHD.Common.Serializer;
using CHD.Common.ServiceCode;
using CHD.Common.ServiceCode.Executor;

namespace CHD.Common.Saver.Structure
{
    //public sealed class RemoteSaver<TAddress, TNativeMessage, TSendableMessage> : IBinarySaver<TAddress>
    //    where TAddress : IAddress
    //    where TNativeMessage : NativeMessage
    //    where TSendableMessage : SendableMessage
    //{
    //    private readonly INativeClientExecutor<TNativeMessage, TSendableMessage> _executor;
    //    private readonly ISendableMessageFactory<TSendableMessage> _sendableMessageFactory;
    //    private readonly IVersionedSettings _versionedSettings;
    //    private readonly ISerializer _serializer;

    //    public ISerializer Serializer
    //    {
    //        get
    //        {
    //            return _serializer;
    //        }
    //    }

    //    public RemoteSaver(
    //        INativeClientExecutor<TNativeMessage, TSendableMessage> executor,
    //        ISendableMessageFactory<TSendableMessage> sendableMessageFactory,
    //        IVersionedSettings versionedSettings,
    //        ISerializer serializer
    //        )
    //    {
    //        if (executor == null)
    //        {
    //            throw new ArgumentNullException("executor");
    //        }
    //        if (sendableMessageFactory == null)
    //        {
    //            throw new ArgumentNullException("sendableMessageFactory");
    //        }
    //        if (versionedSettings == null)
    //        {
    //            throw new ArgumentNullException("versionedSettings");
    //        }
    //        if (serializer == null)
    //        {
    //            throw new ArgumentNullException("serializer");
    //        }
    //        _executor = executor;
    //        _sendableMessageFactory = sendableMessageFactory;
    //        _versionedSettings = versionedSettings;
    //        _serializer = serializer;
    //    }

    //    public bool IsTargetExists(
    //        TAddress address
    //        )
    //    {
    //        if (address == null)
    //        {
    //            throw new ArgumentNullException("address");
    //        }

    //        var result = false;

    //        _executor.Execute(
    //            client =>
    //            {
    //                var messages = ReadAndFilterAndSortMessages(
    //                    client,
    //                    address
    //                    );

    //                result = (messages != null) && (messages.Count > 0);
    //            });

    //        return
    //            result;
    //    }

    //    public void Save<T>(
    //        TAddress address,
    //        T savedObject
    //        )
    //    {
    //        if (address == null)
    //        {
    //            throw new ArgumentNullException("address");
    //        }

    //        var attachmentData = _serializer.Serialize(savedObject);

    //        _executor.Execute(
    //            client =>
    //            {
    //                var messages = ReadAndFilterAndSortMessages(
    //                    client,
    //                    address
    //                    );

    //                var childClient = client.CreateOrEnterChild(address.TargetFolder);

    //                childClient.DeleteMessages(messages);

    //                var newSubject = address.ComposeNewSubject();

    //                using (var sendableWrapper = _sendableMessageFactory.CreateStructureMessage(
    //                    newSubject,
    //                    attachmentData
    //                    ))
    //                {
    //                    childClient.StoreMessage(
    //                        sendableWrapper.Message
    //                        );
    //                }
    //            });
    //    }

    //    public T Read<T>(
    //        TAddress address
    //        )
    //    {
    //        if (address == null)
    //        {
    //            throw new ArgumentNullException("address");
    //        }

    //        using (var ms = new MemoryStream())
    //        {
    //            _executor.Execute(
    //                client =>
    //                {
    //                    var messages = ReadAndFilterAndSortMessages(
    //                        client,
    //                        address
    //                        );

    //                    if (messages != null && messages.Count > 0)
    //                    {
    //                        var lastm = messages.Last();

    //                        var childClient = client.CreateOrEnterChild(address.TargetFolder);
    //                        childClient.DecodeAttachmentTo(
    //                            lastm,
    //                            ms
    //                            );
    //                    }
    //                });

    //            ms.Position = 0;

    //            var result = _serializer.Deserialize<T>(ms);

    //            return result;
    //        }
    //    }







    //    private static List<TNativeMessage> ReadAndFilterAndSortMessages(
    //        INativeClientEx<TNativeMessage, TSendableMessage> client,
    //        TAddress address
    //        )
    //    {
    //        if (client == null)
    //        {
    //            throw new ArgumentNullException("client");
    //        }
    //        if (address == null)
    //        {
    //            throw new ArgumentNullException("address");
    //        }

    //        List<TNativeMessage> result = null;

    //        INativeClientEx<TNativeMessage, TSendableMessage> childClient;
    //        if (client.TryGetChild(address.TargetFolder, out childClient))
    //        {
    //            result = childClient.ReadAndFilterMessages(
    //                address.Filter
    //                );

    //            result.Sort(address.GetComparer<TNativeMessage>());
    //        }

    //        return result;
    //    }
    //}

    public sealed class RemoteSaver<TAddress, TNativeMessage, TSendableMessage> : IBinarySaver<TAddress>
        where TAddress : IAddress
        where TNativeMessage : NativeMessage
        where TSendableMessage : SendableMessage
    {
        private readonly INativeClientExecutor<TNativeMessage, TSendableMessage> _executor;
        private readonly ISendableMessageFactory<TSendableMessage> _sendableMessageFactory;
        private readonly IVersionedSettings _versionedSettings;
        private readonly ISerializer _serializer;

        public ISerializer Serializer
        {
            get
            {
                return _serializer;
            }
        }

        public RemoteSaver(
            INativeClientExecutor<TNativeMessage, TSendableMessage> executor,
            ISendableMessageFactory<TSendableMessage> sendableMessageFactory,
            IVersionedSettings versionedSettings,
            ISerializer serializer
            )
        {
            if (executor == null)
            {
                throw new ArgumentNullException("executor");
            }
            if (sendableMessageFactory == null)
            {
                throw new ArgumentNullException("sendableMessageFactory");
            }
            if (versionedSettings == null)
            {
                throw new ArgumentNullException("versionedSettings");
            }
            if (serializer == null)
            {
                throw new ArgumentNullException("serializer");
            }
            _executor = executor;
            _sendableMessageFactory = sendableMessageFactory;
            _versionedSettings = versionedSettings;
            _serializer = serializer;
        }

        public bool IsTargetExists(
            TAddress address
            )
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }

            var result = false;

            _executor.Execute(
                client =>
                {
                    var messages = ReadAndFilterAndSortMessages(
                        client, 
                        address
                        );

                    result = (messages != null) && (messages.Count > 0);
                });

            return
                result;
        }

        public void Save<T>(
            TAddress address,
            T savedObject
            )
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }

            var attachmentData = _serializer.Serialize(savedObject);

            _executor.Execute(
                client =>
                {
                    var messages = ReadAndFilterAndSortMessages(
                        client,
                        address
                        );

                    string newSubject;
                    if (messages != null && messages.Count > 0)
                    {
                        var lastMessage = messages.LastOrDefault();

                        newSubject = address.ComposeNewSubject(lastMessage.Subject);
                    }
                    else
                    {
                        newSubject = address.ComposeNewSubject();
                    }

                    var childClient = client.CreateOrEnterChild(address.TargetFolder);

                    using (var sendableWrapper = _sendableMessageFactory.CreateStructureMessage(
                        newSubject,
                        attachmentData
                        ))
                    {
                        childClient.StoreMessage(
                            sendableWrapper.Message
                            );
                    }

                    //удаляем только после сохранения новой версии
                    //оставляя то количество снепшотов, которое надо сохранить по настройкам
                    if (messages != null && messages.Count > 0)
                    {
                        var d = messages.TakeWithoutTail(_versionedSettings.StoredSnapshotCount).ToArray();
                        childClient.DeleteMessages(d);
                    }
                });
        }

        public T Read<T>(
            TAddress address
            )
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }

            using (var ms = new MemoryStream())
            {
                _executor.Execute(
                    client =>
                    {
                        var messages = ReadAndFilterAndSortMessages(
                            client,
                            address
                            );

                        if (messages != null && messages.Count > 0)
                        {
                            var lastm = messages.Last();

                            var childClient = client.CreateOrEnterChild(address.TargetFolder);
                            childClient.DecodeAttachmentTo(
                                lastm,
                                ms
                                );
                        }
                    });

                ms.Position = 0;

                var result = _serializer.Deserialize<T>(ms);

                return result;
            }
        }







        private static List<TNativeMessage> ReadAndFilterAndSortMessages(
            INativeClientEx<TNativeMessage, TSendableMessage> client,
            TAddress address
            )
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }

            List<TNativeMessage> result = null;

            INativeClientEx<TNativeMessage, TSendableMessage> childClient;
            if (client.TryGetChild(address.TargetFolder, out childClient))
            {
                result = childClient.ReadAndFilterMessages(
                    address.Filter
                    );

                result.Sort(address.GetComparer<TNativeMessage>());
            }

            return result;
        }
    }
}