using Robust.Shared.GameStates;

namespace Content.Shared._MC.Xeno.Abilities.Rewind;

[RegisterComponent, NetworkedComponent]
public sealed partial class MCXenoRewindComponent : Component
{
    [DataField]
    public TimeSpan Delay = TimeSpan.FromSeconds(5);

    [DataField]
    public float Range = 5f;
}
