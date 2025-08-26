using Content.Shared._MC.Nuke.Events;
using Content.Shared._MC.Nuke.UI;
using Content.Shared.DoAfter;

namespace Content.Shared._MC.Nuke;

public sealed class MCNukeDiskGeneratorSystem : EntitySystem
{
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MCNukeDiskGeneratorComponent, MCNukeDiskGeneratorRunBuiMessage>(OnRunMessage);
    }

    private void OnRunMessage(Entity<MCNukeDiskGeneratorComponent> entity, ref MCNukeDiskGeneratorRunBuiMessage args)
    {
        var doAfter = new DoAfterArgs(EntityManager, args.Actor, entity.Comp.InteractionTime, new MCNukeDiskGeneratorRunDoAfterEvent(), entity, entity, entity);
        _doAfter.TryStartDoAfter(doAfter);
    }
}
