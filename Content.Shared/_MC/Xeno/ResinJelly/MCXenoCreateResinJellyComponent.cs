using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._MC.Xeno.ResinJelly;

[RegisterComponent, NetworkedComponent]
public sealed partial class MCXenoCreateResinJellyComponent : Component
{
    [DataField]
    public EntProtoId ProtoId = "MCXenoResinJelly";
}
