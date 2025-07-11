using Robust.Shared.GameStates;

namespace Content.Shared._MC.Xeno.Abilities.Banish;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class MCXenoBanishComponent : Component
{
    [DataField, AutoNetworkedField]
    public TimeSpan Duration;
}
