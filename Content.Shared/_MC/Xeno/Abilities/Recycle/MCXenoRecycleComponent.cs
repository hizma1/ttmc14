using Robust.Shared.Prototypes;
using Content.Shared.FixedPoint;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;

namespace Content.Shared._MC.Xeno.Abilities.Recycle;

[RegisterComponent, AutoGenerateComponentState]
[Access(typeof(MCXenoRecycleSystem))]
public sealed partial class MCXenoRecycleComponent : Component
{
    [DataField, AutoNetworkedField]
    public TimeSpan RecycleDelay = TimeSpan.FromSeconds(7);

    [DataField, AutoNetworkedField]
    public FixedPoint2 PlasmaCost = 750;

    [DataField, AutoNetworkedField]
    public SoundSpecifier Sound = new SoundPathSpecifier("/Audio/_MC/Effects/recycler.ogg", AudioParams.Default.WithVolume(-11));
}
