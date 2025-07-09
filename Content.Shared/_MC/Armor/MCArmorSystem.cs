using Content.Shared._MC.Xeno.Sunder;
using Content.Shared._RMC14.Armor;
using Content.Shared._RMC14.Weapons.Ranged;
using Content.Shared.Damage;
using Content.Shared.Inventory;
using Content.Shared.Tag;
using Content.Shared.Weapons.Melee;
using Robust.Shared.Prototypes;

namespace Content.Shared._MC.Armor;

public sealed class MCArmorSystem : EntitySystem
{
    private static ProtoId<TagPrototype> TagMelee = "MCDamageMelee";
    private static ProtoId<TagPrototype> TagBullet = "MCDamageBullet";
    private static ProtoId<TagPrototype> TagLaser = "MCDamageLaser";
    private static ProtoId<TagPrototype> TagEnergy = "MCDamageEnergy";
    private static ProtoId<TagPrototype> TagBomb = "MCDamageBomb";
    private static ProtoId<TagPrototype> TagBio = "MCDamageBio";
    private static ProtoId<TagPrototype> TagFire = "MCDamageFire";
    private static ProtoId<TagPrototype> TagAcid = "MCDamageAcid";

    [Dependency] private readonly TagSystem _tag = default!;
    [Dependency] private readonly InventorySystem _inventory = default!;
    [Dependency] private readonly MCXenoSunderSystem _xenoSunder = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MCArmorComponent, MCArmorGetEvent>(OnGet);
        SubscribeLocalEvent<MCArmorComponent, InventoryRelayedEvent<MCArmorGetEvent>>(OnGetRelayed);

        SubscribeLocalEvent<MCArmorComponent, DamageModifyEvent>(OnDamageModify);

        SubscribeLocalEvent<InventoryComponent, MCArmorGetEvent>(_inventory.RelayEvent);
    }

    private void OnGet(Entity<MCArmorComponent> entity, ref MCArmorGetEvent args)
    {
        args.Melee += entity.Comp.Melee;
        args.Bullet += entity.Comp.Bullet;
        args.Laser += entity.Comp.Laser;
        args.Energy += entity.Comp.Energy;
        args.Bomb += entity.Comp.Bomb;
        args.Bio += entity.Comp.Bio;
        args.Fire += entity.Comp.Fire;
        args.Acid += entity.Comp.Acid;
    }

    private void OnGetRelayed(Entity<MCArmorComponent> entity, ref InventoryRelayedEvent<MCArmorGetEvent> args)
    {
        args.Args.Melee += entity.Comp.Melee;
        args.Args.Bullet += entity.Comp.Bullet;
        args.Args.Laser += entity.Comp.Laser;
        args.Args.Energy += entity.Comp.Energy;
        args.Args.Bomb += entity.Comp.Bomb;
        args.Args.Bio += entity.Comp.Bio;
        args.Args.Fire += entity.Comp.Fire;
        args.Args.Acid += entity.Comp.Acid;
    }

    private void OnDamageModify(Entity<MCArmorComponent> entity, ref DamageModifyEvent args)
    {
        DamageModify(entity, ref args);
    }

    private void DamageModify(EntityUid entityUid, ref DamageModifyEvent args)
    {
        var ev = new MCArmorGetEvent(SlotFlags.OUTERCLOTHING | SlotFlags.INNERCLOTHING);
        RaiseLocalEvent(entityUid, ref ev);

        var sunderModifier = _xenoSunder.GetSunder(entityUid);

        if (args.Tool is not { } tool)
            return;

        if (_tag.HasTag(tool, TagMelee) || HasComp<MeleeWeaponComponent>(tool))
        {
            args.Damage *= ArmorToValue(ev.Melee, args.ArmorPiercing, sunderModifier);
            return;
        }

        if (_tag.HasTag(tool, TagBullet) || HasComp<RMCBulletComponent>(tool))
        {
            args.Damage *= ArmorToValue(ev.Bullet, args.ArmorPiercing, sunderModifier);
            return;
        }
    }

    private static float ArmorToValue(int armor, int penetration, float sunder)
    {
        return Math.Clamp((100 - armor * sunder + penetration) * 0.01f, 0, 1);
    }
}
