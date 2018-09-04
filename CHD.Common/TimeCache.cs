using System;
using System.Collections.Generic;
using System.Threading;

namespace CHD.Common
{
    public sealed class TimeCache<TCacheable> : IDisposable
        where TCacheable : IDisposable
    {
        private readonly Func<TCacheable> _clientFactory;
        private readonly int _aliveTimeoutInSeconds;

        private readonly Queue<CacheWrapper<TCacheable>> _cache = new Queue<CacheWrapper<TCacheable>>();
        private readonly object _cacheLocker = new object();

        private readonly AutoResetEvent _threadStopSignal = new AutoResetEvent(false);

        private Thread _thread;
        private readonly object _threadLocker = new object();

        private volatile bool _disposed = false;

        public TimeCache(
            Func<TCacheable> clientFactory,
            int aliveTimeoutInSeconds
            )
        {
            if (clientFactory == null)
            {
                throw new ArgumentNullException("clientFactory");
            }
            _clientFactory = clientFactory;
            _aliveTimeoutInSeconds = aliveTimeoutInSeconds;
        }

        public void AddToCache(
            TCacheable client
            )
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }

            SafelyStopBackground();

            lock (_cacheLocker)
            {
                if (_disposed)
                {
                    return;
                }

                var wrapper = new CacheWrapper<TCacheable>(client, _aliveTimeoutInSeconds);

                _cache.Enqueue(wrapper);

                if (_cache.Count == 1)
                {
                    SafelyStartBackground();
                }
            }
        }

        public TCacheable GetOrCreate(
            )
        {
            TCacheable result;

            SafelyStopBackground();

            lock (_cacheLocker)
            {
                if (_disposed)
                {
                    return
                        default(TCacheable);
                }

                if (_cache.Count > 0)
                {
                    var wrapper = _cache.Dequeue();
                    result = wrapper.Payload;
                }
                else
                {
                    result = _clientFactory();
                }

                if (_cache.Count > 0)
                {
                    SafelyStartBackground();
                }
            }

            return
                result;
        }

        public void Dispose()
        {
            _disposed = true;

            SafelyStopBackground();

            lock (_cacheLocker)
            {
                //заходя в эту блокировку, мы говорим - "надо дождаться, пока все выполняемые методы этого класса завершатся"
                //так как в них тоже осуществляется заход в блокировку

                foreach (var client in _cache)
                {
                    client.Dispose();
                }
            }
        }

        private void SafelyStartBackground()
        {
            if (_disposed)
            {
                return;
            }

            lock (_threadLocker)
            {
                if (_thread != null)
                {
                    return;
                }

                _thread = new Thread(DoWork2);
                _thread.Start();
            }
        }

        private void SafelyStopBackground()
        {
            lock (_threadLocker)
            {
                if (_thread == null)
                {
                    return;
                }

                _threadStopSignal.Set();
                _thread.Join();
                _thread = null;
            }
        }

        private void DoWork2()
        {
            while (true)
            {
                //определяем какой конекшен нужно подождать
                TimeSpan timeout;
                lock (_cacheLocker)
                {
                    repeat:
                    if (_cache.Count == 0)
                    {
                        return;
                    }

                    var wrapper = _cache.Peek();
                    timeout = wrapper.GetTimeout();

                    if (timeout.Ticks <= 0L)
                    {
                        wrapper.Dispose();

                        goto repeat;
                    }
                }

                //есть чего пождать

                var waitResult = _threadStopSignal.WaitOne(timeout);
                if (waitResult)
                {
                    //сигнал завершения потока стартанул
                    //просто выключаемся
                    return;
                }

                //сигнал не стартанул, клиента надо отключить

                lock (_cacheLocker)
                {
                    if (_cache.Count > 0)
                    {
                        var wrapper = _cache.Dequeue();
                        wrapper.Dispose();
                    }
                }
            }
        }

        private sealed class CacheWrapper<TCacheable> : IDisposable
            where TCacheable : IDisposable
        {
            private readonly int _aliveTimeoutInSeconds;

            public TCacheable Payload
            {
                get;
                private set;
            }

            public DateTime CreateTime
            {
                get;
                private set;
            }

            public CacheWrapper(
                TCacheable payload,
                int aliveTimeoutInSeconds
                )
            {
                if (payload == null)
                {
                    throw new ArgumentNullException("payload");
                }

                Payload = payload;
                _aliveTimeoutInSeconds = aliveTimeoutInSeconds;

                CreateTime = DateTime.Now;
            }

            public TimeSpan GetTimeout()
            {
                var lived = DateTime.Now - this.CreateTime;

                var timeout = TimeSpan.FromSeconds(_aliveTimeoutInSeconds) - lived;

                return
                    timeout;
            }

            public void Dispose()
            {
                Payload.Dispose();
            }
        }

    }
}