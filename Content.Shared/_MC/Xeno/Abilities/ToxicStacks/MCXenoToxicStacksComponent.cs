using Robust.Shared.GameStates;

namespace Content.Shared._MC.Xeno.Abilities.ToxicStacks;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class MCXenoToxicStacksComponent : Component
{
    [ViewVariables, AutoNetworkedField]
    public int Count;
}
