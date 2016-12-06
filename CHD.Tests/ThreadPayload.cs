using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using CHD.Graveyard.Token.Factory;

namespace CHD.Tests
{
    internal class ThreadPayload
    {
        private bool _isAllowedToStartIteration = false;
        private bool _isAllowedToCleanup = false;

        private readonly int _threadCount;
        private readonly ConcurrentBag<ITokenController> _controllers;
        private readonly ConcurrentDictionary<int, bool> _results = new ConcurrentDictionary<int, bool>();


        public ThreadPayload(
            params ITokenController[] controllers
            )
        {
            _threadCount = controllers.Length;

            _controllers = new ConcurrentBag<ITokenController>(
                controllers
                );
        }




        public ITokenController GetMyTokenController()
        {
            ITokenController result;
            _controllers.TryTake(out result);

            return
                result;
        }





        public bool IsAllowedToStartIteration()
        {
            return
                Volatile.Read(ref _isAllowedToStartIteration);
        }

        public bool IsAllowedToCleanup()
        {
            return
                Volatile.Read(ref _isAllowedToCleanup);
        }



        public void LetShowBegin()
        {
            Volatile.Write(ref _isAllowedToStartIteration, true);
        }

        public void NotifyAboutResult(bool result)
        {
            _results[Thread.CurrentThread.ManagedThreadId] = result;

            if (_results.Count == _threadCount)
            {
                Volatile.Write(ref _isAllowedToCleanup, true);
            }
        }



        public bool IsTestOk()
        {
            foreach (var pair in _results)
            {
                if (pair.Value)
                {
                    Debug.WriteLine("Thread {0} SUCCESSed to obtain token", pair.Key);
                }
                else
                {
                    Debug.WriteLine("Thread {0} fails to obtain token", pair.Key);
                }
            }

            if (_results.Count != _threadCount)
            {
                return
                    false;
            }

            if (_results.Count(j => j.Value) != 1)
            {
                return
                    false;
            }

            return
                true;
        }

    }
}