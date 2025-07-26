namespace Content.Shared._MC.Xeno.Blessings;

public sealed class MCXenoBlessingsSystem : EntitySystem
{
    [Dependency] private readonly SharedUserInterfaceSystem _ui = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MCXenoBlessingsComponent, MCXenoBlessingsActionEvent>(OnAction);
    }

    private void OnAction(Entity<MCXenoBlessingsComponent> entity, ref MCXenoBlessingsActionEvent args)
    {
        args.Handled = true;
        _ui.TryOpenUi(entity.Owner, MCXenoBlessingsUiKey.Key, entity);
    }
}
