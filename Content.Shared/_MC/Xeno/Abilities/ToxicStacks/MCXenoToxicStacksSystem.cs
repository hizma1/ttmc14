using Content.Shared.Projectiles;

namespace Content.Shared._MC.Xeno.Abilities.ToxicStacks;

public sealed class MCXenoToxicStacksSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MCXenoToxicStacksOnHitComponent, ProjectileHitEvent>(OnProjectileHit);
    }

    private void OnProjectileHit(Entity<MCXenoToxicStacksOnHitComponent> entity, ref ProjectileHitEvent args)
    {
        Add(args.Target, entity.Comp.Amount);
    }

    public void Add(Entity<MCXenoToxicStacksComponent?> entity, int count)
    {
        if (!Resolve(entity, ref entity.Comp, logMissing: false))
            return;

        Set(entity, entity.Comp.Count + count);
    }

    public void Set(Entity<MCXenoToxicStacksComponent?> entity, int count)
    {
        if (!Resolve(entity, ref entity.Comp, logMissing: false))
            return;

        entity.Comp.Count = count;
        Dirty(entity);
    }
}
