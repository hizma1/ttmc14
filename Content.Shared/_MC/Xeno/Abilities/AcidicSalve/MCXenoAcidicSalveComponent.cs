using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._MC.Xeno.Abilities.AcidicSalve;

[RegisterComponent, NetworkedComponent]
public sealed partial class MCXenoAcidicSalveComponent : Component
{
    [DataField]
    public TimeSpan Delay = TimeSpan.FromSeconds(1);

    [DataField]
    public float Range = 1.5f;

    [DataField]
    public EntProtoId EffectProtoId = "RMCEffectHealHealer";
}
