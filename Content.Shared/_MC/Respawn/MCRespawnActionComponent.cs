using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._MC.Respawn;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class MCRespawnActionComponent : Component
{
	[DataField, AutoNetworkedField]
	public EntProtoId ActionId = "MCRespawnAction";

	[DataField, AutoNetworkedField]
	public EntityUid? Action;
}
