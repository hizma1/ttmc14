using Content.Shared.Damage;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._MC.Xeno.Abilities.Stomp;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class MCXenoStompComponent : Component
{
    [DataField, AutoNetworkedField]
    public EntProtoId Effect = "CMEffectSelfStomp";

    [DataField, AutoNetworkedField]
    public DamageSpecifier Damage;

    [DataField, AutoNetworkedField]
    public float Distance = 1.5f;

    [DataField, AutoNetworkedField]
    public TimeSpan Paralyze = TimeSpan.FromSeconds(3f);

    [DataField, AutoNetworkedField]
    public TimeSpan ThrowParalyze = TimeSpan.FromSeconds(0.5f);

    [DataField, AutoNetworkedField]
    public float ThrowDistance = 1f;

    [DataField, AutoNetworkedField]
    public float ThrowSpeed = 10f;

    [DataField, AutoNetworkedField]
    public SoundSpecifier Sound = new SoundPathSpecifier("/Audio/_RMC14/Xeno/alien_footstep_charge1.ogg");
}
