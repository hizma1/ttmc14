using Robust.Shared.GameStates;

namespace Content.Shared._MC.Xeno.Abilities.Pounce;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class MCXenoPouncingComponent : Component
{
    [DataField, AutoNetworkedField]
    public TimeSpan End;
}
