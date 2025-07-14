namespace Content.Shared._MC.Xeno.Abilities.ToxicStacks;

public sealed class MCXenoToxicStacksSystem : EntitySystem
{
    public void Add(EntityUid uid, int count)
    {
        var component = EnsureComp<MCXenoToxicStacksComponent>(uid);
        Set(uid, component.Count + count);
    }

    public void Set(EntityUid uid, int count)
    {
        var component = EnsureComp<MCXenoToxicStacksComponent>(uid);
        component.Count = count;
        Dirty(uid, component);
    }
}
