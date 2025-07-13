using Robust.Shared.GameStates;

namespace Content.Shared._MC.Xeno.Abilities.Banish;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class MCXenoBanishComponent : Component
{
    [ViewVariables, AutoNetworkedField]
    public EntityUid? Target;

    [DataField, AutoNetworkedField]
    public TimeSpan Duration = TimeSpan.FromSeconds(10);

    [DataField, AutoNetworkedField]
    public float Range = 3;
}
