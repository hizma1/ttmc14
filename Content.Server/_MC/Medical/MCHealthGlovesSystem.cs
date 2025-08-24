using Content.Shared.Medical.Components;
using Content.Server.Medical.Components;
using Content.Shared.Interaction;
using Robust.Shared.GameObjects;
using Robust.Server.GameObjects;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.MedicalScanner;

namespace Content.Server.Medical.Systems;

public sealed class MCHealthGlovesSystem : EntitySystem
{
    [Dependency] private readonly InventorySystem _inventory = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<InteractHandEvent>(OnAnyInteractHand);

        SubscribeLocalEvent<AfterInteractEvent>(OnAnyAfterInteract, before: new[] { typeof(Content.Shared._RMC14.Medical.Scanner.HealthScannerSystem) });

        SubscribeLocalEvent<MCHealthGlovesComponent, GotEquippedEvent>(OnGlovesGotEquipped);
    }

    private void OnAnyInteractHand(InteractHandEvent args)
    {
        var user = args.User;
        var target = args.Target;

    if (!_inventory.TryGetSlotEntity(user, "gloves", out var glovesNullable) || glovesNullable == default)
            return;

        var gloves = glovesNullable.Value;

        if (!TryComp<MCHealthGlovesComponent>(gloves, out var glovesComp))
            return;

    if (!TryComp<Content.Shared._RMC14.Medical.Scanner.HealthScannerComponent>(gloves, out var analyzer))
            return;

    var clickLocation = Transform(target).Coordinates;
        var canReach = true;

        var after = new AfterInteractEvent(user, gloves, target, clickLocation, canReach);
        RaiseLocalEvent(gloves, after);
    }

    private void OnAnyAfterInteract(AfterInteractEvent args)
    {
        var used = args.Used;

        if (!TryComp<Content.Shared._RMC14.Medical.Scanner.HealthScannerComponent>(used, out var _))
            return;

        if (!TryComp<MCHealthGlovesComponent>(used, out var _))
            return;

        if (!_inventory.TryGetSlotEntity(args.User, "gloves", out var worn) || worn != used)
        {
            args.Handled = true;
        }
    }

    private void OnGlovesGotEquipped(EntityUid uid, MCHealthGlovesComponent component, GotEquippedEvent args)
    {
        if (TryComp<Content.Shared._RMC14.Medical.Scanner.HealthScannerComponent>(uid, out var scanner))
        {
            Dirty(uid, scanner);
        }
    }
}
