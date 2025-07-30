using Robust.Shared.GameStates;
using Content.Shared.Chemistry.Reagent;

namespace Content.Shared._MC.Chemistry;

[Serializable, NetSerializable]
public sealed class MCSolutionSyncComponentState : ComponentState
{
    public ReagentQuantity[] Reagents;
    public FixedPoint2 Volume;
    public FixedPoint2 MaxVolume;

    public MCSolutionSyncComponentState(ReagentQuantity[] reagents, FixedPoint2 volume, FixedPoint2 maxVolume)
    {
        Reagents = reagents;
        Volume = volume;
        MaxVolume = maxVolume;
    }
}
