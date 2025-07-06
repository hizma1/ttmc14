using Robust.Shared.GameStates;

namespace Content.Shared._MC.Droppod.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class MCDroppodUserComponent : Component
{
    [DataField, AutoNetworkedField]
    public EntityUid DroppodEntity;
}
