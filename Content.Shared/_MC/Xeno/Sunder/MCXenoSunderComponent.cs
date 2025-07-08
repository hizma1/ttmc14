using Content.Shared.Alert;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared._MC.Xeno.Sunder;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(fieldDeltas: true), AutoGenerateComponentPause]
public sealed partial class MCXenoSunderComponent : Component
{
    [DataField, AutoNetworkedField]
    public float Multiplier = 1;

    [DataField, AutoNetworkedField]
    public float Recover = 1;

    [DataField, AutoNetworkedField]
    public float Value = 100;

    [DataField, AutoNetworkedField]
    public TimeSpan RegenCooldown = TimeSpan.FromSeconds(1);

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer)), AutoNetworkedField, AutoPausedField]
    public TimeSpan NextRegenTime;

    [DataField, AutoNetworkedField]
    public ProtoId<AlertPrototype> Alert = "MCXenoArmor";
}
