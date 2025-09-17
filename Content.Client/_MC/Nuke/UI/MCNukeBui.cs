using Content.Shared._MC.Nuke.Components;
using Content.Shared._MC.Nuke.UI;
using JetBrains.Annotations;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Containers;

namespace Content.Client._MC.Nuke.UI;

[UsedImplicitly]
public sealed class MCNukeBui : BoundUserInterface
{
    [Dependency] private readonly IEntityManager _entities = default!;

    [ViewVariables]
    private MCNukeWindow? _window;

    public MCNukeBui(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindow<MCNukeWindow>();

        _window.DiskRedButton.OnPressed += _ => SendMessage(new MCNukeSlotBuiMessage(MCNukeComponent.SlotRedId));
        _window.DiskBlueButton.OnPressed += _ => SendMessage(new MCNukeSlotBuiMessage(MCNukeComponent.SlotBlueId));
        _window.DiskGreenButton.OnPressed += _ => SendMessage(new MCNukeSlotBuiMessage(MCNukeComponent.SlotGreenId));

        RefreshButtons();
    }

    private void RefreshButtons()
    {
        if (_window is null)
            return;

        if (!_entities.TryGetComponent<ContainerManagerComponent>(Owner, out var component))
            return;

        RefreshButton(_window.DiskRedButton, MCNukeComponent.SlotRedId, component);
        RefreshButton(_window.DiskBlueButton, MCNukeComponent.SlotBlueId, component);
        RefreshButton(_window.DiskGreenButton, MCNukeComponent.SlotGreenId, component);
    }

    private void RefreshButton(Button button, string id, ContainerManagerComponent component)
    {
        button.Text = component.Containers[id].Count == 0 ? "Inject" : "Eject";
    }
}
