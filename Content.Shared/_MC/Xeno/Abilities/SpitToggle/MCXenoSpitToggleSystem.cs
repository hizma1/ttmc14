using Content.Shared._MC.Xeno.Spit;
using Content.Shared.Actions;

namespace Content.Shared._MC.Xeno.Abilities.SpitToggle;

public sealed class MCXenoSpitToggleSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actions = default!;
    [Dependency] private readonly MCSharedXenoSpitSystem _xenoSpit = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MCXenoSpitToggleComponent, MCXenoSpitToggleActionEvent>(OnAction);
    }

    private void OnAction(Entity<MCXenoSpitToggleComponent> entity, ref MCXenoSpitToggleActionEvent args)
    {
        args.Handled = true;

        if (TryComp<MCXenoSpitComponent>(entity, out var xenoSpitComponent) && !xenoSpitComponent.Enabled || !HasComp<MCXenoSpitComponent>(entity))
        {
            _xenoSpit.SetPreset(
                entity.Owner,
                entity.Comp.ProjectileId,
                entity.Comp.PlasmaCost,
                entity.Comp.Delay,
                entity.Comp.Speed,
                entity.Comp.Sound);

            _actions.SetToggled(args.Action, true);
            return;
        }

        _xenoSpit.ResetPreset(entity.Owner);
        _actions.SetToggled(args.Action, false);
    }
}
