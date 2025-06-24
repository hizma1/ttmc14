using Content.Shared._RMC14.Xenonids.Damage;
using Content.Shared.Damage;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Rounding;

namespace Content.Shared._MC.Xeno.Damage;

public sealed class MCXenoDamageVisualsSystem: EntitySystem
{
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly MobThresholdSystem _thresholds = default!;

    private EntityQuery<MobThresholdsComponent> _mobThresholdsQuery;

    public override void Initialize()
    {
        _mobThresholdsQuery = GetEntityQuery<MobThresholdsComponent>();

        SubscribeLocalEvent<MCXenoDamageVisualsComponent, DamageChangedEvent>(OnVisualsDamageChanged);
    }

    private void OnVisualsDamageChanged(Entity<MCXenoDamageVisualsComponent> ent, ref DamageChangedEvent args)
    {
        if (!_mobThresholdsQuery.TryComp(ent, out var thresholds) ||
            !_thresholds.TryGetIncapThreshold(ent, out var threshold, thresholds))
            return;

        var damage = args.Damageable.TotalDamage.Double();
        var max = threshold.Value.Double();
        var level = damage > threshold
            ? ent.Comp.States + 1
            : ContentHelpers.RoundToEqualLevels(damage, max, ent.Comp.States + 1);
        _appearance.SetData(ent, RMCDamageVisuals.State, level);
    }
}
