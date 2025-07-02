using Robust.Shared.GameStates;

namespace Content.Shared._MC.Xeno.Portal;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class MCXenoPortalComponent : Component
{
    [DataField, AutoNetworkedField]
    public EntityUid? LinkedEntity;
}
