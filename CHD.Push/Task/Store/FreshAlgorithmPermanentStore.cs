using System;
using System.Collections.Generic;

namespace CHD.Push.Task.Store
{
    /// <summary>
    /// Отфильтровывает уже неактуальные пушеры при загрузке.
    /// Сохранение не модифицирует
    /// </summary>
    public class FreshAlgorithmPermanentStore : IAlgorithmPermanentStore
    {
        private readonly IAlgorithmPermanentStore _algorithmPermanentStore;

        public FreshAlgorithmPermanentStore(
            IAlgorithmPermanentStore algorithmPermanentStore
            )
        {
            if (algorithmPermanentStore == null)
            {
                throw new ArgumentNullException("algorithmPermanentStore");
            }
            _algorithmPermanentStore = algorithmPermanentStore;
        }

        public IEnumerable<IAlgorithm> Load(
            )
        {
            var delayedAlgorithms = _algorithmPermanentStore.Load();
            var freshDelayedPushers = new List<IAlgorithm>();
            foreach (var delayedAlgorithm in delayedAlgorithms)
            {
                var delayedFileWrapper = delayedAlgorithm.Pusher.FileWrapper;
                var freshFileWrapper = delayedFileWrapper.RefreshByFileSystem();
                if (freshFileWrapper.Exists == delayedFileWrapper.Exists)
                {
                    //ModificationTime doesn't make sense for checking

                    freshDelayedPushers.Add(delayedAlgorithm);
                }
            }

            return
                freshDelayedPushers;
        }

        public void Save(IAlgorithm algorithm)
        {
            _algorithmPermanentStore.Save(algorithm);
        }
    }
}