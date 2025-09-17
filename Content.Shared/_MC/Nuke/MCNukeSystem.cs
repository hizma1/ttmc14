using Content.Shared._MC.Nuke.Components;
using Content.Shared._MC.Nuke.UI;

namespace Content.Shared._MC.Nuke;

public sealed class MCNukeSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MCNukeComponent, MCNukeSlotBuiMessage>(OnSlotInteractMessage);
    }

    private void OnSlotInteractMessage(Entity<MCNukeComponent> entity, ref MCNukeSlotBuiMessage args)
    {
        
    }
}
