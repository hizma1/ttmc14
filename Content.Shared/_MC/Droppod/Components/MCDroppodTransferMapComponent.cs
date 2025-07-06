using Robust.Shared.GameStates;

namespace Content.Shared._MC.Droppod.Components;

[RegisterComponent, NetworkedComponent]
public sealed partial class MCDroppodTransferMapComponent : Component
{
    [DataField]
    public string Parallax = "FastSpace";
}
