using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._MC.Weapon.Range.Flamer;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class MCFlamerTankFireTypeComponent : Component
{
    [DataField, AutoNetworkedField]
    public EntProtoId Spawn = "MCFire";
}
