using System.Diagnostics.CodeAnalysis;
using Content.Shared.Chemistry.Components;
using Content.Shared.Inventory;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Robust.Shared.Serialization;

namespace Content.Shared._MC.Containers;
public sealed class MCSlotBasedConnectedContainerSystem : EntitySystem
{
    [Dependency] private readonly SharedContainerSystem _containers = default!;
    [Dependency] private readonly EntityWhitelistSystem _whitelistSystem = default!;
    [Dependency] private readonly InventorySystem _inventory = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<MCSlotBasedConnectedContainerComponent, AfterInteractEvent>(OnAfterInteract);
    }

    private void OnAfterInteract(Entity<MCSlotBasedConnectedContainerComponent> ent, ref AfterInteractEvent args)
    {
        if (args.Handled || args.Target is not EntityUid target)
            return;

        if (!_inventory.TryGetContainerSlotEnumerator(args.User, out var enumerator, ent.Comp.TargetSlot))
            return;

        var found = false;
        while (enumerator.NextItem(out var item))
        {
            if (item == target && !_whitelistSystem.IsWhitelistFailOrNull(ent.Comp.ContainerWhitelist, item))
            {
                found = true;
                break;
            }
        }

        if (!found)
            return;

        var doAfterArgs = new DoAfterArgs(
            EntityManager,
            args.User,
            TimeSpan.FromSeconds(ent.Comp.DoAfterTime),
            new MCSlotBasedConnectedContainerDoAfterEvent(),
            null,
            target,
            ent.Owner)
        {
            BreakOnMove = ent.Comp.DoAfterBreakOnMove,
            BreakOnDamage = ent.Comp.DoAfterBreakOnDamage,
            NeedHand = ent.Comp.DoAfterNeedHand
        };

        if (_doAfter.TryStartDoAfter(doAfterArgs))
        {
            args.Handled = true;
        }
    }
}

[ByRefEvent]
public struct MCGetConnectedContainerEvent
{
    public EntityUid? ContainerEntity;
}
