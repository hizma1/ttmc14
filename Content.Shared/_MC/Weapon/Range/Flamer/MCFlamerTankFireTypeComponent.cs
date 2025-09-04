using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._MC.Weapon.Range.Flamer;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class MCFlamerTankFireTypeComponent : Component
{
    [DataField, AutoNetworkedField]
    public EntProtoId Spawn = "MCFire";

    [DataField, AutoNetworkedField]
    public int MaxIntensity = 80;

    [DataField, AutoNetworkedField]
    public int MaxDuration = 50;
}
