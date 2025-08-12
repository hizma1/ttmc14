using System.Linq;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Atmos;
using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Heal;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Pheromones;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Robust.Shared.Network;

namespace Content.Shared._MC.Xeno.Abilities.AcidicSalve;

public sealed class MCXenoAcidicSlaveSystem : EntitySystem
{
    [Dependency] private readonly INetManager _net = default!;

    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly RMCActionsSystem _rmcActions = default!;

    [Dependency] private readonly SharedInteractionSystem _interaction = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
    [Dependency] private readonly SharedXenoHiveSystem _xenoHive = default!;
    [Dependency] private readonly SharedXenoHealSystem _xenoHeal = default!;
    [Dependency] private readonly SharedRMCFlammableSystem _flammable = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MCXenoAcidicSalveComponent, MCXenoAcidicSlaveActionEvent>(OnAction);
        SubscribeLocalEvent<MCXenoAcidicSalveComponent, MCXenoAcidicSlaveDoAfterEvent>(OnDoAfter);
    }

    private void OnAction(Entity<MCXenoAcidicSalveComponent> entity, ref MCXenoAcidicSlaveActionEvent args)
    {
        if (args.Handled)
            return;

        if (!HasComp<XenoComponent>(entity) || !HasComp<XenoComponent>(args.Target))
            return;

        if (!_interaction.InRangeUnobstructed(entity.Owner, args.Target, entity.Comp.Range))
            return;

        if (_mobState.IsDead(args.Target))
            return;

        if (_flammable.IsOnFire(args.Target))
            return;

        if (!_xenoHive.FromSameHive(entity.Owner, args.Target))
            return;

        if (TryComp(args.Target, out DamageableComponent? damageComp) && damageComp.TotalDamage == 0)
            return;

        if (!_rmcActions.TryUseAction(entity, args.Action, entity))
            return;

        args.Handled = true;

        var ev = new MCXenoAcidicSlaveDoAfterEvent();
        var doAfter = new DoAfterArgs(EntityManager, entity, entity.Comp.Delay, ev, entity, args.Target)
        {
            BreakOnMove = true,
            RequireCanInteract = true,
        };

        _doAfter.TryStartDoAfter(doAfter);
    }

    private void OnDoAfter(Entity<MCXenoAcidicSalveComponent> entity, ref MCXenoAcidicSlaveDoAfterEvent args)
    {
        if (args.Target is null)
            return;

        var pheromones = CompOrNull<XenoRecoveryPheromonesComponent>(args.Target)?.Multiplier ?? 1f;
        var health = CompOrNull<MobThresholdsComponent>(args.Target)
            ?.Thresholds.FirstOrDefault(e => e.Value == MobState.Critical)
            .Key ?? 0;

        var value = 50 + pheromones * health * 0.01f;
        _xenoHeal.Heal(args.Target.Value, value);

        if(_net.IsServer)
            SpawnAttachedTo(entity.Comp.EffectProtoId, args.Target.Value.ToCoordinates());
    }
}
