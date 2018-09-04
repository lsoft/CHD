namespace CHD.Tests.ExtendedContext.StatRecord
{
    public interface IStatisticRecordFactory
    {
        IStatisticRecord Create(
            string header
            );
    }
}