using Content.Shared._RMC14.Sentry;
using Content.Shared._RMC14.Xenonids.Construction;
using Content.Shared._RMC14.Xenonids.Construction.Tunnel;
using Content.Shared._RMC14.Xenonids.Egg;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared.Buckle.Components;
using Content.Shared.Maps;
using Content.Shared.Tag;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using static Content.Shared.Physics.CollisionGroup;

namespace Content.Shared._MC.Xeno.Projectiles;

public sealed class MCXenoProjectileSystem : EntitySystem
{
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly SharedXenoHiveSystem _hive = default!;
    [Dependency] private readonly TurfSystem _turf = default!;
    [Dependency] private readonly SharedMapSystem _mapSystem = default!;
    [Dependency] private readonly TagSystem _tag = default!;

    private static readonly ProtoId<TagPrototype> AirlockTag = "Airlock";
    private static readonly ProtoId<TagPrototype> StructureTag = "Structure";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MCXenoSpawnConstructionOnTerminatingComponent, MapInitEvent>(OnSpawnConstructionOnTerminatingInit);
        SubscribeLocalEvent<MCXenoSpawnConstructionOnTerminatingComponent, EntityTerminatingEvent>(OnSpawnConstructionOnTerminatingTerminate);
    }

    private void OnSpawnConstructionOnTerminatingInit(Entity<MCXenoSpawnConstructionOnTerminatingComponent> entity, ref MapInitEvent args)
    {
        entity.Comp.Origin = _transform.GetMoverCoordinates(entity);
        Dirty(entity);
    }

    private void OnSpawnConstructionOnTerminatingTerminate(Entity<MCXenoSpawnConstructionOnTerminatingComponent> entity, ref EntityTerminatingEvent args)
    {
        if (_net.IsClient)
            return;

        var transform = Transform(entity);
        if (TerminatingOrDeleted(transform.ParentUid))
            return;

        var coordinates = transform.Coordinates;
        if (entity.Comp.ProjectileAdjust &&
            entity.Comp.Origin is { } origin &&
            coordinates.TryDelta(EntityManager, _transform, origin, out var delta) &&
            delta.Length() > 0)
            coordinates = coordinates.Offset(delta.Normalized() / -2);

        if (!CanPlace(coordinates))
            return;

        var spawn = SpawnAtPosition(entity.Comp.Spawn, coordinates);
        _hive.SetSameHive(entity.Owner, spawn);
    }

    public bool CanPlace(EntityCoordinates coords)
    {
        if (_transform.GetGrid(coords) is not { } gridId || !TryComp<MapGridComponent>(gridId, out var grid))
            return false;

        var tile = _mapSystem.TileIndicesFor(gridId, grid, coords);
        var anchored = _mapSystem.GetAnchoredEntitiesEnumerator(gridId, grid, tile);

        while (anchored.MoveNext(out var uid))
        {
            if (HasComp<XenoEggComponent>(uid))
                return false;

            if (HasComp<XenoConstructComponent>(uid) ||
                _tag.HasAnyTag(uid.Value, StructureTag, AirlockTag) ||
                HasComp<StrapComponent>(uid) ||
                HasComp<XenoTunnelComponent>(uid) ||
                HasComp<SentryComponent>(uid) ||
                HasComp<BlockXenoConstructionComponent>(uid))
                return false;
        }

        return !_turf.IsTileBlocked(gridId, tile, Impassable | MidImpassable | HighImpassable, grid);
    }
}
