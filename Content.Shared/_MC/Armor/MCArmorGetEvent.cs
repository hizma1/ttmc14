using Content.Shared.Inventory;

namespace Content.Shared._MC.Armor;

[ByRefEvent]
public record struct  MCArmorGetEvent(
    SlotFlags TargetSlots,
    int Melee = 0,
    int Bullet = 0,
    int Laser = 0,
    int Energy = 0,
    int Bomb = 0,
    int Bio = 0,
    int Fire = 0,
    int Acid = 0
) : IInventoryRelayEvent;
