using Content.Shared._MC.Nuke.UI;
using Content.Shared.FixedPoint;
using JetBrains.Annotations;
using Robust.Client.UserInterface;

namespace Content.Client._MC.Nuke.UI;

[UsedImplicitly]
public sealed class MCNukeDiskGeneratorBui : BoundUserInterface
{
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

        RefreshOverall(0);
    }

    private void RefreshOverall(FixedPoint2 value)
    {
        if (_window is null)
            return;

        _window.OverallProgressBar.Value = value.Float();
        _window.OverallProgressLabel.Text = $"{value.Float()}%";
    }
}
