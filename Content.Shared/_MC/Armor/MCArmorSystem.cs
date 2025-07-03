using Content.Shared._MC.Xeno.Sunder;
using Content.Shared.Damage;

namespace Content.Shared._MC.Armor;

public sealed class MCArmorSystem : EntitySystem
{
    [Dependency] private readonly MCXenoSunderSystem _xenoSunder = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MCArmorComponent, DamageModifyEvent>(OnDamageModify);
    }

    private void OnDamageModify(Entity<MCArmorComponent> entity, ref DamageModifyEvent args)
    {
        var sunderModifier = _xenoSunder.GetSunder(entity.Owner);

    }
}
