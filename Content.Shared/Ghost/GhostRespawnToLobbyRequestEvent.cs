using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Ghost;

[Serializable, NetSerializable]
public sealed class GhostRespawnToLobbyRequestEvent : EntityEventArgs
{
}
