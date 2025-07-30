using Robust.Shared.GameStates;

namespace Content.Shared._MC.Chemistry;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(SharedMCChemistrySystem))]
public sealed partial class MCDetailedExaminableSolutionComponent : Component
{
    [DataField(required: true), AutoNetworkedField]
    public string Solution = string.Empty;
}
