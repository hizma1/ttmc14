using Robust.Shared.Map;

// ReSharper disable once CheckNamespace
namespace Content.Shared._RMC14.Power;

public abstract partial class SharedRMCPowerSystem
{
    private bool AnyReactorsOnGrid(EntityUid gridUid)
    {
        var reactors = EntityQueryEnumerator<RMCFusionReactorComponent, TransformComponent>();
        while (reactors.MoveNext(out var uid,  out var comp, out var xform))
        {
            if (comp.State == RMCFusionReactorState.Working && _transform.GetGrid(uid) is { } reactorGridUid && reactorGridUid == gridUid)
                return true;
        }

        return false;
    }
}
