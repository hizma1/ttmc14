using Content.Server.Atmos.EntitySystems;
using Content.Shared._MC.Flammable;

namespace Content.Server._MC.Flammable;

public sealed class MCFlammableSystem : MCSharedFlammableSystem
{
    [Dependency] private readonly FlammableSystem _flammable = default!;

    public override void RemoveStacks(EntityUid uid, float stacks)
    {
        base.RemoveStacks(uid, stacks);
        _flammable.AdjustFireStacks(uid, -stacks);
    }
}
