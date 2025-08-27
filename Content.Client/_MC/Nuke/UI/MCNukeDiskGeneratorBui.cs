using Content.Shared._MC.Nuke.Components;
using Content.Shared._MC.Nuke.UI;
using Content.Shared.FixedPoint;
using JetBrains.Annotations;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;

namespace Content.Client._MC.Nuke.UI;

[UsedImplicitly]
public sealed class MCNukeDiskGeneratorBui : BoundUserInterface
{
    [Dependency] private readonly IEntityManager _entities = default!;

    [ViewVariables]
    private MCNukeDiskGeneratorWindow? _window;

    public MCNukeDiskGeneratorBui(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindow<MCNukeDiskGeneratorWindow>();
        _window.RunButton.OnPressed += _ => SendMessage(new MCNukeDiskGeneratorRunBuiMessage());

        if (!_entities.TryGetComponent<MCNukeDiskGeneratorComponent>(Owner, out var component))
            return;

        RefreshOverall(component.OverallProgress, component.Color);
    }

    private void RefreshOverall(FixedPoint2 value, Color color)
    {
        if (_window is null)
            return;

        _window.OverallProgressBar.Value = value.Float() * 100;
        _window.OverallProgressBar.ForegroundStyleBoxOverride = new StyleBoxFlat(color);
        _window.OverallProgressLabel.Text = $"{(value * 100).Int()}%";
    }
}
