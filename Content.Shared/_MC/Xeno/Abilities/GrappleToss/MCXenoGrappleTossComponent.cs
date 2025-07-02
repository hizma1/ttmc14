using Robust.Shared.GameStates;

namespace Content.Shared._MC.Xeno.Abilities.GrappleToss;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class MCXenoGrappleTossComponent : Component
{
    [DataField, AutoNetworkedField]
    public float Distance = 4;

    [DataField, AutoNetworkedField]
    public float Speed = 10;

    [DataField, AutoNetworkedField]
    public TimeSpan SlowdownDuration = TimeSpan.FromSeconds(3f);

    [DataField, AutoNetworkedField]
    public TimeSpan ParalyzeDuration = TimeSpan.FromSeconds(0.5f);
}
