using Content.Shared.Whitelist;
using Robust.Shared.GameStates;

namespace Content.Shared._MC.Chemistry;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(SharedMCChemistrySystem))]
public sealed partial class MCSolutionTransferWhitelistComponent : Component
{
    [DataField, AutoNetworkedField]
    public LocId Popup;

    [DataField, AutoNetworkedField]
    public EntityWhitelist? SourceWhitelist;

    [DataField, AutoNetworkedField]
    public EntityWhitelist? TargetWhitelist;
}
