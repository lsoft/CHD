using CHD.Graveyard.ExclusiveAccess;

namespace CHD.Graveyard.Graveyard
{
    public interface IGraveyard
    {
        bool TryGetExclusiveAccess(
            out IExclusiveAccess exclusiveAccess
            );
    }
}