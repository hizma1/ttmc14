using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared._RMC14.Xenonids.Projectile;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs.Components;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Network;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._MC.Xeno.Spit;

public abstract class MCSharedXenoSpitSystem : EntitySystem
{
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly SharedGunSystem _gun = default!;
    [Dependency] private readonly SharedPhysicsSystem _physics = default!;

    [Dependency] private readonly XenoSystem _xeno = default!;
    [Dependency] private readonly XenoPlasmaSystem _xenoPlasma = default!;
    [Dependency] private readonly SharedXenoHiveSystem _xenoHive = default!;

    protected EntityQuery<MCXenoSpitComponent> XenoSpitQuery;

    public override void Initialize()
    {
        base.Initialize();

        XenoSpitQuery = GetEntityQuery<MCXenoSpitComponent>();

        SubscribeAllEvent<MCXenoSpitEvent>(OnSpit);
    }

    public void SetPreset(Entity<MCXenoSpitComponent?> entity, EntProtoId projectileId, FixedPoint2 plasmaCost, TimeSpan delay, float speed, SoundSpecifier? sound)
    {
        entity.Comp ??= EnsureComp<MCXenoSpitComponent>(entity);

        entity.Comp.Enabled = true;
        entity.Comp.ProjectileId = projectileId;
        entity.Comp.PlasmaCost = plasmaCost;
        entity.Comp.Delay = delay;
        entity.Comp.Speed = speed;
        entity.Comp.Sound = sound;
        Dirty(entity);
    }

    public void ResetPreset(Entity<MCXenoSpitComponent?> entity)
    {
        entity.Comp ??= EnsureComp<MCXenoSpitComponent>(entity);
        entity.Comp.Enabled = false;
        Dirty(entity);
    }

    private void OnSpit(MCXenoSpitEvent ev)
    {
        var entityUid = GetEntity(ev.Xeno);
        var targetUid = GetEntity(ev.Target);
        var coordinates = GetCoordinates(ev.Coordinates);

        if (targetUid is not null && HasComp<MobStateComponent>(targetUid) && !_xeno.CanAbilityAttackTarget(entityUid, targetUid.Value))
            targetUid = null;

        if (!XenoSpitQuery.TryComp(entityUid, out var xenoSpitComponent))
            return;

        if (!xenoSpitComponent.Enabled)
            return;

        if (xenoSpitComponent.NextShot > _timing.CurTime)
            return;

        if (!_xenoPlasma.TryRemovePlasmaPopup(entityUid, xenoSpitComponent.PlasmaCost))
            return;

        xenoSpitComponent.NextShot = xenoSpitComponent.Delay + _timing.CurTime;
        Dirty(entityUid, xenoSpitComponent);

        var transform = Transform(entityUid);

        var delta = coordinates.Position - transform.Coordinates.Position;
        var velocity = _physics.GetMapLinearVelocity(entityUid);

        if (xenoSpitComponent.Sound is not null)
            _audio.PlayPredicted(xenoSpitComponent.Sound, entityUid, entityUid);

        if (_net.IsClient)
            return;

        var instance = Spawn(xenoSpitComponent.ProjectileId, transform.Coordinates);
        delta *= xenoSpitComponent.Speed / delta.Length();

        _gun.ShootProjectile(instance, delta, velocity, entityUid, entityUid, xenoSpitComponent.Speed);

        EnsureComp<XenoProjectileComponent>(instance);
        _xenoHive.SetSameHive(entityUid, instance);

        if (targetUid is not null)
        {
            var targeted = EnsureComp<TargetedProjectileComponent>(instance);
            targeted.Target = targetUid.Value;
            Dirty(instance, targeted);
        }
    }
}
