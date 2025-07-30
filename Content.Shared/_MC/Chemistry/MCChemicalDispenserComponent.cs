using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._MC.Chemistry;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
[Access(typeof(SharedMCChemistrySystem))]
public sealed partial class MCChemicalDispenserComponent : Component
{
    [DataField, AutoNetworkedField]
    public FixedPoint2 Energy;

    [DataField, AutoNetworkedField]
    public FixedPoint2 MaxEnergy;

    [DataField, AutoNetworkedField]
    public FixedPoint2 CostPerUnit = 0.1;

    [DataField(required: true), AutoNetworkedField]
    public EntProtoId Network;

    [DataField, AutoNetworkedField]
    public string ContainerSlotId = "chemical_dispenser_slot";

    [DataField, AutoNetworkedField]
    public ProtoId<ReagentPrototype>[] Reagents =
    [
        "MCAluminum", "MCCarbon", "MCChlorine", "MCCopper", "MCEthanol", "MCFluorine",
        "MCHydrogen", "MCIron", "MCLithium", "MCMercury", "MCNitrogen", "MCOxygen",
        "MCPhosphorus", "MCPotassium", "MCRadium", "MCSilicon", "MCSodium", "MCSugar",
        "MCSulfur", "MCSulphuricAcid", "MCWater",
    ];

    [DataField, AutoNetworkedField]
    public FixedPoint2 DispenseSetting = 5;

    [DataField, AutoNetworkedField]
    public FixedPoint2[] Settings = [5, 10, 20, 30, 40];
}
