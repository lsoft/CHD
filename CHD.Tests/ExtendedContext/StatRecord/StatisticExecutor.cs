namespace CHD.Tests.ExtendedContext.StatRecord
{
    //internal sealed class StatisticExecutor<TNativeMessage, TSendableMessage> : INativeClientExecutor<TNativeMessage, TSendableMessage>
    //    where TNativeMessage : NativeMessage
    //    where TSendableMessage : SendableMessage
    //{
    //    private readonly INativeClientExecutor<TNativeMessage, TSendableMessage> _executor;
    //    private readonly IStatisticRecordFactory _statisticRecordFactory;
    //    private readonly IDisorderLogger _logger;

    //    public StatisticExecutor(
    //        INativeClientExecutor<TNativeMessage, TSendableMessage> executor,
    //        IStatisticRecordFactory statisticRecordFactory,
    //        IDisorderLogger logger
    //        )
    //    {
    //        if (executor == null)
    //        {
    //            throw new ArgumentNullException("executor");
    //        }
    //        if (statisticRecordFactory == null)
    //        {
    //            throw new ArgumentNullException("statisticRecordFactory");
    //        }
    //        if (logger == null)
    //        {
    //            throw new ArgumentNullException("logger");
    //        }

    //        _executor = executor;
    //        _statisticRecordFactory = statisticRecordFactory;
    //        _logger = logger;
    //    }


    //    public void Execute(
    //        Action<INativeClientEx<TNativeMessage, TSendableMessage>> executeAction
    //        )
    //    {
    //        if (executeAction == null)
    //        {
    //            throw new ArgumentNullException("executeAction");
    //        }

    //        using (_statisticRecordFactory.Create("Execute executor"))
    //        {
    //            _executor.Execute(executeAction);
    //        }
    //    }

    //    public T Execute<T>(
    //        Func<INativeClientEx<TNativeMessage, TSendableMessage>, T> executeAction
    //        )
    //    {
    //        if (executeAction == null)
    //        {
    //            throw new ArgumentNullException("executeAction");
    //        }

    //        using (_statisticRecordFactory.Create("Execute executor"))
    //        {
    //            return 
    //                _executor.Execute(executeAction);
    //        }
    //    }
    //}
}