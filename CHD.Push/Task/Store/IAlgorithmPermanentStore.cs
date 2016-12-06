using System.Collections.Generic;

namespace CHD.Push.Task.Store
{
    public interface IAlgorithmPermanentStore
    {
        IEnumerable<IAlgorithm> Load(
            );

        void Save(
            IAlgorithm algorithm
            );
    }
}