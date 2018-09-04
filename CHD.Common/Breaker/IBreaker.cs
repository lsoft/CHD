namespace CHD.Common.Breaker
{
    public interface IBreaker : IReadBreaker
    {
        void FireBreak(
            string message = ""
            );

        void ResetBreak();
    }
}