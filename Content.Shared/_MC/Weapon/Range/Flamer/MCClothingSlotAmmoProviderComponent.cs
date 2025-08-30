
using Content.Shared.Weapons.Ranged.Systems;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.GameStates;

namespace Content.Shared._MC.Weapon.Range.Flamer;

[RegisterComponent, NetworkedComponent, Access(typeof(SharedGunSystem))]
public sealed partial class MCClothingSlotAmmoProviderComponent : AmmoProviderComponent;
