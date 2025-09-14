using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared.Damage;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Melee.Events;

namespace Content.Shared._MC.Xeno.Plasma;

public sealed class MCXenoPlasmaSystem : EntitySystem
{
    [Dependency] private readonly XenoPlasmaSystem _xenoPlasma = default!;

    private EntityQuery<XenoPlasmaComponent> _query;
    private EntityQuery<MobStateComponent> _mobStateQuery;

    public override void Initialize()
    {
        base.Initialize();

        _query = GetEntityQuery<XenoPlasmaComponent>();
        _mobStateQuery = GetEntityQuery<MobStateComponent>();

        SubscribeLocalEvent<MCXenoPlasmaDamageOnHitComponent, ProjectileHitEvent>(OnDamageHit);

        SubscribeLocalEvent<MCXenoPlasmaOnAttackComponent, MeleeHitEvent>(OnDamage);
        SubscribeLocalEvent<MCXenoPlasmaOnAttackedComponent, DamageChangedEvent>(OnDamaged);
    }

    private void OnDamageHit(Entity<MCXenoPlasmaDamageOnHitComponent> entity, ref ProjectileHitEvent args)
    {
        if (!_query.TryComp(args.Target, out var plasmaComponent))
            return;

        _xenoPlasma.RemovePlasma((args.Target, plasmaComponent), entity.Comp.Amount + entity.Comp.Multiplier * plasmaComponent.MaxPlasma);
    }

    private void OnDamage(Entity<MCXenoPlasmaOnAttackComponent> ent, ref MeleeHitEvent args)
    {
        if (!args.IsHit)
            return;

        foreach (var hit in args.HitEntities)
        {
            if (!_mobStateQuery.TryComp(hit, out var mobStateComponent))
                continue;

            if (mobStateComponent.CurrentState == MobState.Dead)
                continue;

            _xenoPlasma.RegenPlasma(ent.Owner, args.BaseDamage.GetTotal() * ent.Comp.Multiplier);
        }
    }

    private void OnDamaged(Entity<MCXenoPlasmaOnAttackedComponent> ent, ref DamageChangedEvent args)
    {
        if (!args.DamageIncreased || args.DamageDelta is null)
            return;

        _xenoPlasma.RegenPlasma(ent.Owner, args.DamageDelta.GetTotal() * ent.Comp.Multiplier);
    }
}
