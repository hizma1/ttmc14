using Content.Shared._MC.Xeno.Sunder;
using Content.Shared.FixedPoint;
using Robust.Shared.GameStates;
using Robust.Shared.Utility;
using static Robust.Shared.Utility.SpriteSpecifier;

namespace Content.Shared._RMC14.Xenonids.Pheromones;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(SharedXenoPheromonesSystem), typeof(MCXenoSunderSystem))]
public sealed partial class XenoRecoveryPheromonesComponent : Component
{
    [DataField, AutoNetworkedField]
    public SpriteSpecifier Icon = new Rsi(new ResPath("/Textures/_RMC14/Interface/xeno_pheromones_hud.rsi"), "recovery");

    [DataField, AutoNetworkedField]
    public FixedPoint2 Multiplier;
}
