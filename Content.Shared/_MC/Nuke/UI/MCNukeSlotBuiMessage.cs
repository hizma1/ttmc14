using Robust.Shared.Serialization;

namespace Content.Shared._MC.Nuke.UI;

[Serializable, NetSerializable]
public sealed class MCNukeSlotBuiMessage : BoundUserInterfaceMessage
{
    public readonly string SlotId;

    public MCNukeSlotBuiMessage(string slotId)
    {
        SlotId = slotId;
    }
}
