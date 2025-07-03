namespace Content.Shared._MC.Xeno.Sunder;

public sealed class MCXenoSunderSystem : EntitySystem
{
    private EntityQuery<MCXenoSunderComponent> _sunderQuery;

    public override void Initialize()
    {
        base.Initialize();

        _sunderQuery = GetEntityQuery<MCXenoSunderComponent>();
    }

    public float GetSunder(Entity<MCXenoSunderComponent?> entity)
    {
        if (!_sunderQuery.Resolve(entity, ref entity.Comp))
            return 1f;

        return 1 - entity.Comp.Value * 0.01f;
    }
}
