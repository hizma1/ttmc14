using Robust.Shared.GameStates;

namespace Content.Shared._MC.Chemistry;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(SharedMCChemistrySystem))]
public sealed partial class MCEmptySolutionComponent : Component
{
    [DataField, AutoNetworkedField]
    public string Solution = "pen";
}
