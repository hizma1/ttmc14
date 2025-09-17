using Robust.Shared.GameStates;

namespace Content.Shared._MC.Nuke.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class MCNukeComponent : Component
{
    public const string SlotRedId = "disk_red";
    public const string SlotBlueId = "disk_blue";
    public const string SlotGreenId = "disk_green";

    [DataField, AutoNetworkedField]
    public TimeSpan TimeMin = TimeSpan.FromSeconds(360);

    [DataField, AutoNetworkedField]
    public TimeSpan TimeMax = TimeSpan.FromSeconds(1200);
}
