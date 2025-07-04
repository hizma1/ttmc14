using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Shared._MC.Xeno.Projectiles;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(MCXenoProjectileSystem))]
public sealed partial class MCXenoSpawnConstructionOnTerminatingComponent : Component
{
    [DataField, AutoNetworkedField]
    public EntityCoordinates? Origin;

    [DataField(required: true), AutoNetworkedField]
    public EntProtoId Spawn;

    [DataField, AutoNetworkedField]
    public bool ProjectileAdjust = true;
}
