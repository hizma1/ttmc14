using Content.Shared.Eye.Blinding.Systems;
using Robust.Shared.Prototypes;

namespace Content.Shared.EntityEffects.Effects;

/// <summary>
/// Heal or apply eye damage (supports fractional values)
/// </summary>
public sealed partial class ChemHealEyeDamage : EntityEffect
{
    /// <summary>
    /// How much eye damage to add. Can be fractional (e.g. 0.5).
    /// </summary>
    [DataField]
    public float Amount = -1f;

    protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
        => Loc.GetString("reagent-effect-guidebook-cure-eye-damage",
                         ("chance", Probability),
                         ("deltasign", MathF.Sign(Amount)));

    public override void Effect(EntityEffectBaseArgs args)
    {
        if (args is EntityEffectReagentArgs reagentArgs)
            if (reagentArgs.Scale != 1f)
                return;

        // Convert to int (round to nearest)
        var intAmount = (int)MathF.Round(Amount);
        if (intAmount != 0)
        {
            args.EntityManager.EntitySysManager
                .GetEntitySystem<BlindableSystem>()
                .AdjustEyeDamage(args.TargetEntity, intAmount);
        }
    }
}
