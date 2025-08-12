using Robust.Shared.GameStates;

namespace Content.Shared._MC.Xeno.Spawner;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class MCXenoStructureSpawnerTargetComponent : Component
{
    [ViewVariables, AutoNetworkedField]
    public EntityUid Origin;
}
