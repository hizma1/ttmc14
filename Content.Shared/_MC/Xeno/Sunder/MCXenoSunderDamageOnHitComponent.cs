using Robust.Shared.GameStates;

namespace Content.Shared._MC.Xeno.Sunder;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class MCXenoSunderDamageOnHitComponent : Component
{
    [DataField, AutoNetworkedField]
    public float Amount;
}
