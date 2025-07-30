using Robust.Shared.GameStates;

namespace Content.Shared._MC.Chemistry;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(SharedMCChemistrySystem))]
public sealed partial class MCToggleableSolutionTransferComponent : Component
{
    [DataField(required: true), AutoNetworkedField]
    public string Solution;

    [DataField, AutoNetworkedField]
    public SolutionTransferDirection Direction = SolutionTransferDirection.Input;
}
public enum SolutionTransferDirection
{
    Input, Output,
}
