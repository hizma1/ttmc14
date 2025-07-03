using Robust.Shared.GameStates;

namespace Content.Shared._MC.Xeno.Abilities.SpitToggle;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class MCXenoSpitToggleComponent : Component
{
    [DataField, AutoNetworkedField]
    public EntityUid? ActionId;
}
