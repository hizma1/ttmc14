using Content.Shared.FixedPoint;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._MC.Xeno.Abilities.SpitToggle;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class MCXenoSpitToggleComponent : Component
{
    [DataField, AutoNetworkedField]
    public EntProtoId ProjectileId;

    [DataField, AutoNetworkedField]
    public FixedPoint2 PlasmaCost;

    [DataField, AutoNetworkedField]
    public TimeSpan Delay = TimeSpan.FromSeconds(0.5f);

    [DataField, AutoNetworkedField]
    public float Speed;

    [DataField, AutoNetworkedField]
    public SoundSpecifier? Sound  = new SoundCollectionSpecifier("XenoSpitAcid", AudioParams.Default.WithVolume(-10f));
}
