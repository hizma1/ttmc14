using Robust.Shared.Prototypes;

namespace Content.Shared._MC.Xeno.Construction;

[ByRefEvent]
public readonly record struct MCXenoWeedsChosenEvent(EntProtoId Id, MCXenoPlantingWeedsComponent.WeedEntry Data);
