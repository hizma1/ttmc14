using Content.Shared._MC.Nuke.Components;
using Content.Shared._MC.Nuke.Events;
using Content.Shared._MC.Nuke.UI;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Robust.Shared.Timing;

namespace Content.Shared._MC.Nuke;

public sealed class MCNukeDiskGeneratorSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MCNukeDiskGeneratorComponent, MCNukeDiskGeneratorRunBuiMessage>(OnRunMessage);
        SubscribeLocalEvent<MCNukeDiskGeneratorComponent, MCNukeDiskGeneratorRunDoAfterEvent>(OnRunDoAfter);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<MCNukeDiskGeneratorComponent, MCNukeDiskGeneratorRunningComponent>();
        while (query.MoveNext(out var uid, out var generator, out var running))
        {
            generator.OverallProgress = FixedPoint2.Clamp(generator.CheckpointProgress + generator.StepSize * ((_timing.CurTime - running.StartTime) / generator.StepDuration), generator.OverallProgress, generator.CheckpointProgress + generator.StepSize);
            Dirty(uid, generator);

            if (_timing.CurTime < running.StartTime + generator.StepDuration)
                continue;

            generator.CheckpointProgress += generator.StepSize;
            RemCompDeferred<MCNukeDiskGeneratorRunningComponent>(uid);
        }
    }

    private void OnRunMessage(Entity<MCNukeDiskGeneratorComponent> entity, ref MCNukeDiskGeneratorRunBuiMessage args)
    {
        if (HasComp<MCNukeDiskGeneratorRunningComponent>(entity))
            return;

        _doAfter.TryStartDoAfter(new DoAfterArgs(EntityManager, args.Actor, entity.Comp.InteractionTime, new MCNukeDiskGeneratorRunDoAfterEvent(), entity, entity, entity));
    }

    private void OnRunDoAfter(Entity<MCNukeDiskGeneratorComponent> entity, ref MCNukeDiskGeneratorRunDoAfterEvent args)
    {
        if (args.Handled || args.Cancelled)
            return;

        args.Handled = true;

        if (HasComp<MCNukeDiskGeneratorRunningComponent>(entity))
            return;

        var state = new MCNukeDiskGeneratorRunningComponent
        {
            StartTime = _timing.CurTime,
        };

        AddComp(entity, state, true);
        Dirty(entity, state);
    }
}
