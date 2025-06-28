using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared.Actions;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Movement.Pulling.Systems;
using Robust.Shared.Map;

namespace Content.Shared._MC.Xeno.Abilities.Blink;

public sealed class MCXenoBlinkSystem : EntitySystem
{
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly ExamineSystemShared _examine = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly RMCActionsSystem _rmcActions = default!;
    [Dependency] private readonly SharedActionsSystem _actions = default!;
    [Dependency] private readonly PullingSystem _pulling = default!;
    [Dependency] private readonly SharedXenoHiveSystem _xenoHive = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly RMCSlowSystem _rmcSlow = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MCXenoBlinkComponent, MCXenoBlinkActionEvent>(OnAction);
        SubscribeLocalEvent<MCXenoBlinkComponent, MCXenoBlinkDoAfterEvent>(OnDoAfter);
    }

    private void OnAction(Entity<MCXenoBlinkComponent> entity, ref MCXenoBlinkActionEvent args)
    {
        if (args.Handled)
            return;

        var origin = _transform.GetMapCoordinates(entity);
        var target = _transform.ToMapCoordinates(args.Target);
        var distance = (origin.Position - target.Position).LengthSquared();

        if (distance > entity.Comp.Range * entity.Comp.Range)
            return;

        if (!_examine.InRangeUnOccluded(origin, target, entity.Comp.Range, null))
            return;

        if (!_rmcActions.CanUseActionPopup(entity, args.Action, entity))
            return;

        if (TryComp<PullableComponent>(entity, out var pullable) && pullable.BeingPulled)
            _pulling.TryStopPull(entity, pullable);

        args.Handled = true;

        if (CompOrNull<PullerComponent>(entity)?.Pulling is not { } targetUid)
        {
            if (!_rmcActions.TryUseAction(entity, args.Action, entity))
                return;

            DebuffAoe(entity, origin);
            DebuffAoe(entity, target);

            _transform.SetMapCoordinates(entity, target);

            IncreaseCooldown(entity, entity.Comp.DragFriendlyMultiplier);
            return;
        }

        // Fo friendly targets
        if (_xenoHive.FromSameHive(entity.Owner, targetUid))
        {
            if (!_rmcActions.TryUseAction(entity, args.Action, entity))
                return;

            _transform.SetMapCoordinates(targetUid, target);

            // Still continue base teleportation process
            DebuffAoe(entity, origin);
            DebuffAoe(entity, target);

            _transform.SetMapCoordinates(entity, target);

            IncreaseCooldown(entity, entity.Comp.DragFriendlyMultiplier);
            return;
        }

        // For enemy target (or items idk)
        var ev = new MCXenoBlinkDoAfterEvent(origin, target, GetNetEntity(args.Action));
        var doAfter = new DoAfterArgs(EntityManager, entity, entity.Comp.DragDelay, ev, entity, targetUid)
        {
            NeedHand = true,
            BreakOnMove = true,
            RequireCanInteract = true,
        };

        _doAfter.TryStartDoAfter(doAfter);
    }

    private void OnDoAfter(Entity<MCXenoBlinkComponent> entity, ref MCXenoBlinkDoAfterEvent args)
    {
        if (args.Handled || args.Cancelled || args.Target is not { } targetUid)
            return;

        if (!_rmcActions.TryUseAction(entity, GetEntity(args.Action), entity))
            return;

        args.Handled = true;

        _transform.SetMapCoordinates(targetUid, args.TargetCoordinates);

        // Still continue base teleportation process
        DebuffAoe(entity, args.OriginCoordinates);
        DebuffAoe(entity, args.TargetCoordinates);

        _transform.SetMapCoordinates(entity, args.TargetCoordinates);

        IncreaseCooldown(entity, entity.Comp.DragMultiplier);
    }

    private void IncreaseCooldown(Entity<MCXenoBlinkComponent> entity, float modifier)
    {
        foreach (var (actionId, action) in _actions.GetActions(entity))
        {
            if (action.BaseEvent is not MCXenoBlinkActionEvent)
                continue;

            if (action.Cooldown is null)
                continue;

            _actions.SetCooldown(actionId, action.Cooldown.Value.Start, action.Cooldown.Value.End + (action.UseDelay ?? TimeSpan.Zero) * modifier);
        }
    }

    private void DebuffAoe(Entity<MCXenoBlinkComponent> entity, MapCoordinates position)
    {
        foreach (var taget in _lookup.GetEntitiesInRange<MobStateComponent>(position, entity.Comp.Range))
        {
            if (_mobState.IsDead(taget, taget.Comp))
                continue;

            if (HasComp<XenoComponent>(taget) && _xenoHive.FromSameHive(entity.Owner, taget.Owner))
                continue;

            // TODO: Stagger
            _rmcSlow.TrySlowdown(taget, entity.Comp.StaggerDuration);
        }
    }
}
