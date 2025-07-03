using Robust.Shared.GameStates;

namespace Content.Shared._MC.Xeno.Sunder;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class MCXenoSunderComponent : Component
{
    [DataField, AutoNetworkedField]
    public float Multiplier = 1;

    [DataField, AutoNetworkedField]
    public float Recover = 1;

    [DataField, AutoNetworkedField]
    public float Value = 100;
}
