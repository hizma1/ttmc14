using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._MC.Chemistry;

[Serializable, NetSerializable]
public enum MCChemicalDispenserUi
{
    Key,
}

[Serializable, NetSerializable]
public sealed class MCChemicalDispenserDispenseSettingBuiMsg(FixedPoint2 amount) : BoundUserInterfaceMessage
{
    public readonly FixedPoint2 Amount = amount;
}

[Serializable, NetSerializable]
public sealed class MCChemicalDispenserBeakerBuiMsg(FixedPoint2 amount) : BoundUserInterfaceMessage
{
    public readonly FixedPoint2 Amount = amount;
}

[Serializable, NetSerializable]
public sealed class MCChemicalDispenserEjectBeakerBuiMsg : BoundUserInterfaceMessage;

[Serializable, NetSerializable]
public sealed class MCChemicalDispenserDispenseBuiMsg(ProtoId<ReagentPrototype> reagent) : BoundUserInterfaceMessage
{
    public readonly ProtoId<ReagentPrototype> Reagent = reagent;
}
