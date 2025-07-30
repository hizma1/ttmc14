using Robust.Shared.GameStates;

namespace Content.Shared._MC.Chemistry;

[RegisterComponent, NetworkedComponent]
[Access(typeof(SharedMCChemistrySystem))]
public sealed partial class MCNoMixingReagentsComponent : Component;
