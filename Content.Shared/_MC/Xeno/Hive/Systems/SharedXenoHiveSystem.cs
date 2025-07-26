using Content.Shared._MC.Xeno.Hive.Prototypes;
using Robust.Shared.Prototypes;
// ReSharper disable CheckNamespace

namespace Content.Shared._RMC14.Xenonids.Hive;

public partial class SharedXenoHiveSystem
{
    public void AddPsypoints(Entity<HiveComponent> entity, ProtoId<MCXenoHivePsypointTypePrototype> id, int value)
    {
        SetPsypoints(entity, id, GetPsypoints(entity, id) + value);
    }

    public void SetPsypoints(Entity<HiveComponent> entity, ProtoId<MCXenoHivePsypointTypePrototype> id, int value)
    {
        if (entity.Comp.Psypoints.TryAdd(id, value))
            return;

        entity.Comp.Psypoints[id] = value;
        Dirty(entity);
    }

    public int GetPsypoints(Entity<HiveComponent> entity, ProtoId<MCXenoHivePsypointTypePrototype> id)
    {
        return entity.Comp.Psypoints.GetValueOrDefault(id, 0);
    }

    public bool HasPsypoints(Entity<HiveComponent> entity, ProtoId<MCXenoHivePsypointTypePrototype> id, int value)
    {
        if (!entity.Comp.Psypoints.TryGetValue(id, out var count))
            return false;

        return value >= count;
    }
}
