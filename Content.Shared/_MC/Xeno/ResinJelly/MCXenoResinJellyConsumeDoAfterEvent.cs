using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared._MC.Xeno.ResinJelly;

[Serializable, NetSerializable]
public sealed partial class MCXenoResinJellyConsumeDoAfterEvent : DoAfterEvent
{
    public override DoAfterEvent Clone()
    {
        return this;
    }
}
