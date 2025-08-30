using Content.Shared.Containers;
using Content.Shared.Inventory;
using Content.Shared.Whitelist;
using Robust.Shared.GameStates;
using Content.Shared._MC.Containers;

[RegisterComponent, Access(typeof(MCSlotBasedConnectedContainerSystem)), NetworkedComponent]
public sealed partial class MCSlotBasedConnectedContainerComponent : Component
{
    [DataField(required: true)]
    public SlotFlags TargetSlot;

    [DataField]
    public EntityWhitelist? ContainerWhitelist;

    [DataField]
    public float DoAfterTime = 2.0f;

    [DataField]
    public bool DoAfterBreakOnMove = true;

    [DataField]
    public bool DoAfterBreakOnDamage = true;

    [DataField]
    public bool DoAfterNeedHand = false;
}
