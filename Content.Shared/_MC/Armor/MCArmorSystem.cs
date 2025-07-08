using Content.Shared._MC.Xeno.Sunder;
using Content.Shared._RMC14.Weapons.Ranged;
using Content.Shared.Damage;
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
    [Dependency] private readonly MCXenoSunderSystem _xenoSunder = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MCArmorComponent, DamageModifyEvent>(OnDamageModify);
    }

    private void OnDamageModify(Entity<MCArmorComponent> entity, ref DamageModifyEvent args)
    {
        var sunderModifier = _xenoSunder.GetSunder(entity.Owner);

        if (args.Tool is not { } tool)
            return;

        if (_tag.HasTag(tool, TagMelee) || HasComp<MeleeWeaponComponent>(tool))
        {
            args.Damage *= ArmorToValue(entity.Comp.Melee, args.ArmorPiercing, sunderModifier);
            return;
        }

        if (_tag.HasTag(tool, TagBullet) || HasComp<RMCBulletComponent>(tool))
        {
            args.Damage *= ArmorToValue(entity.Comp.Bullet, args.ArmorPiercing, sunderModifier);
            return;
        }
    }

    private static float ArmorToValue(int armor, int penetration, float sunder)
    {
        return Math.Clamp((100 - armor * sunder + penetration) * 0.01f, 0, 1);
    }
}
