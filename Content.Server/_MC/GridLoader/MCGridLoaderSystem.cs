using Robust.Server.GameObjects;
using Robust.Shared.EntitySerialization.Systems;

namespace Content.Server._MC.GridLoader;

public sealed class MCGridLoaderSystem : EntitySystem
{
    [Dependency] private readonly MapLoaderSystem _mapLoader = default!;
    [Dependency] private readonly TransformSystem _transform = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MCGridLoaderComponent, MapInitEvent>(OnMapInit);
    }

    private void OnMapInit(Entity<MCGridLoaderComponent> entity, ref MapInitEvent args)
    {
        var transform = Transform(entity);
        var position = _transform.GetMapCoordinates(transform).Position;
        var rotation = transform.LocalRotation;

        _mapLoader.TryLoadGrid(transform.MapID, entity.Comp.Map, out _, offset: position, rot: rotation);
    }
}
