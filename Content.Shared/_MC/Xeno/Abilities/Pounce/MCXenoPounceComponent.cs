using Content.Shared.Damage;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;

namespace Content.Shared._MC.Xeno.Abilities.Pounce;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class MCXenoPounceComponent : Component
{
    [DataField, AutoNetworkedField]
    public float MaxDistance = 6;

    [DataField, AutoNetworkedField]
    public int Strength = 20;

    [DataField, AutoNetworkedField]
    public DamageSpecifier HitDamage;

    [DataField, AutoNetworkedField]
    public TimeSpan HitSelfParalyzeTime = TimeSpan.FromSeconds(0.5f);

    [DataField, AutoNetworkedField]
    public TimeSpan HitKnockdownTime = TimeSpan.FromSeconds(2f);

    [DataField, AutoNetworkedField]
    public SoundSpecifier? HitSound = new SoundPathSpecifier("/Audio/_RMC14/Xeno/alien_pounce.ogg");
}
