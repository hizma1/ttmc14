
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;
using System;

namespace Content.Shared._MC.Containers
{
    [Serializable, NetSerializable]
    public sealed partial class MCSlotBasedConnectedContainerDoAfterEvent : SimpleDoAfterEvent
    {
    }
}
