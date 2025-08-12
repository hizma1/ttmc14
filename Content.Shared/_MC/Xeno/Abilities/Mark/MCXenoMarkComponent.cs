using Robust.Shared.GameStates;

namespace Content.Shared._MC.Xeno.Abilities.Mark;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class MCXenoMarkComponent : Component
{
    [ViewVariables, AutoNetworkedField]
    public EntityUid? Target;
}
