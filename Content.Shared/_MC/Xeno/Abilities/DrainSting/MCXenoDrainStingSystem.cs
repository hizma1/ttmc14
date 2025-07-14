using Content.Shared._MC.Xeno.Abilities.ToxicStacks;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Emote;
using Content.Shared._RMC14.Xenonids.Heal;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared.Damage;
using Content.Shared.Popups;
using Content.Shared.Stunnable;

namespace Content.Shared._MC.Xeno.Abilities.DrainSting;

public sealed class MCXenoDrainStingSystem : EntitySystem
{
    [Dependency] private readonly RMCActionsSystem _rmcActions = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly SharedStunSystem _stun = default!;
    [Dependency] private readonly SharedRMCEmoteSystem _emote = default!;
    [Dependency] private readonly SharedXenoHealSystem _xenoHeal = default!;
    [Dependency] private readonly XenoPlasmaSystem _xenoPlasma = default!;
    [Dependency] private readonly MCXenoToxicStacksSystem _xenoToxicStacks = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MCXenoDrainStingComponent, MCXenoDrainStingActionEvent>(OnAction);
    }

    private void OnAction(Entity<MCXenoDrainStingComponent> entity, ref MCXenoDrainStingActionEvent args)
    {
        if (args.Handled)
            return;

        if (!TryComp<MCXenoToxicStacksComponent>(args.Target, out var toxicStacksComponent))
        {
            _popup.PopupClient("Immune to intoxication", entity, entity);
            return;
        }

        if (toxicStacksComponent.Count == 0)
        {
            _popup.PopupClient("Not intoxicated", entity, entity);
            return;
        }

        if (!_rmcActions.TryUseAction(entity, args.Action, entity))
            return;

        args.Handled = true;

        var stacks = toxicStacksComponent.Count;
        var drainPotency = stacks * 6;

        if (stacks > toxicStacksComponent.Max - 10)
            _emote.TryEmoteWithChat(args.Target, "Scream");

        // TODO: bonus armor

        _damageable.TryChangeDamage(args.Target, entity.Comp.Damage * drainPotency / 5);
        _stun.TryKnockdown(args.Target, TimeSpan.FromSeconds(Math.Max(0.1f, (stacks - 10f) / 10f)), true);
        _xenoHeal.Heal(entity, drainPotency);
        _xenoPlasma.RegenPlasma(entity.Owner, drainPotency * 3.5f);
        _xenoToxicStacks.Add(args.Target, (int) -Math.Round(stacks * 0.7f));
    }
}
