using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._MC.Droppod.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class MCDroppodComponent : Component
{
    [DataField, AutoNetworkedField]
    public List<EntProtoId> Actions = new()
    {
        "MCActionDroppodTarget",
        "MCActionDroppodLaunch",
    };

    [DataField, AutoNetworkedField]
    public List<EntityUid> ActionEntities = new();

    [DataField, AutoNetworkedField]
    public MCDroppodState State = MCDroppodState.Ready;

    [DataField, AutoNetworkedField]
    public bool LaunchAllowed = true;

    [DataField, AutoNetworkedField]
    public bool OperationStarted;

    [DataField, AutoNetworkedField]
    public TimeSpan TransitDelay = TimeSpan.FromSeconds(10f);

    [DataField, AutoNetworkedField]
    public TimeSpan FallDelay = TimeSpan.FromSeconds(0.6f);

    [ViewVariables, AutoNetworkedField]
    public MapCoordinates? Target;
}

[Serializable, NetSerializable]
public sealed class MCDroppodTagetBuiMsg : BoundUserInterfaceMessage
{
    public readonly Vector2i Tile;

    public MCDroppodTagetBuiMsg(Vector2i tile)
    {
        Tile = tile;
    }
}

[Serializable, NetSerializable]
public enum MCDroppodUI
{
    Key,
}

public enum MCDroppodVisualLayers : byte
{
    Base,
}

[Serializable, NetSerializable]
public enum MCDroppodState
{
    Ready,
    Active,
    Landed,
}
