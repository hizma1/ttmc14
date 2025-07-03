using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared._MC.Xeno.Spit;

[Serializable, NetSerializable]
public sealed class MCXenoSpitEvent : EntityEventArgs
{
    public readonly NetEntity? Target;
    public readonly NetEntity Xeno;
    public readonly NetCoordinates Coordinates;

    public MCXenoSpitEvent(NetEntity? target, NetEntity xeno, NetCoordinates coordinates)
    {
        Target = target;
        Xeno = xeno;
        Coordinates = coordinates;
    }
}
