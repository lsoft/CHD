using CHD.Common.FileSystem.Surgeon;

namespace CHD.Tests.FileSystem.Surgeon.Factory
{
    public interface ISurgeonFactory
    {
        ISurgeon Surge(
            );
    }
}