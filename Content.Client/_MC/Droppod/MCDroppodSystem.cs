using Content.Shared._MC.Droppod;
using Content.Shared._MC.Droppod.Components;
using Content.Shared.Buckle.Components;
using Robust.Client.GameObjects;

namespace Content.Client._MC.Droppod;

public sealed class MCDroppodSystem : MCSharedDroppodSystem
{
    protected override void UpdateSprite(Entity<MCDroppodComponent> entity)
    {
        base.UpdateSprite(entity);

        if (!TryComp<StrapComponent>(entity, out var strapComponent))
            return;

        if (!TryComp<SpriteComponent>(entity, out var spriteComponent))
            return;

        if (!spriteComponent.LayerMapTryGet(MCDroppodVisualLayers.Base, out var layer))
            return;

        var state = "singlepod_green";
        if (entity.Comp.State != MCDroppodState.Active)
        {
            state += entity.Comp is { LaunchAllowed: true, OperationStarted: true }
                ? "_active"
                : "_inactive";
        }

        spriteComponent.LayerSetState(layer, state);
    }
}
