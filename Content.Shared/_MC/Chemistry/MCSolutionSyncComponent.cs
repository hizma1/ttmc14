using Robust.Shared.GameStates;

namespace Content.Shared._MC.Chemistry;

[RegisterComponent, NetworkedComponent]
public sealed partial class MCSolutionSyncComponent : Component
{
    [DataField]
    public string Solution = "solution";
}
