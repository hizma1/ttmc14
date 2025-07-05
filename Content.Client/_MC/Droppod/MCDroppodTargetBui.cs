using Content.Shared._MC.Droppod.Components;
using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.TacticalMap;
using JetBrains.Annotations;
using Robust.Client.Player;
using Robust.Client.UserInterface;

namespace Content.Client._MC.Droppod;

[UsedImplicitly]
public sealed class MCDroppodTargetBui : BoundUserInterface
{
    [Dependency] private readonly IPlayerManager _player = default!;

    private MCDroppodTargetWindow? _window;

    public MCDroppodTargetBui(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindow<MCDroppodTargetWindow>();

        _window.Control.MouseFilter = Control.MouseFilterMode.Stop;
        _window.Control.OnClickedInIndices = OnClicked;
        _window.Control.DrawAreaLabels = false;

        UpdateTacticalMapDisplay();
    }

    private void UpdateTacticalMapDisplay()
    {
        if (_window is null)
            return;

        if (_player.LocalEntity is not { } player)
            return;

        if (!EntMan.TryGetComponent<TacticalMapUserComponent>(player, out var user) || !EntMan.TryGetComponent<AreaGridComponent>(user.Map, out var areaGrid))
            return;

        _window.Control.UpdateTexture((user.Map.Value, areaGrid));
    }

    private void OnClicked(Vector2i tile)
    {
        SendMessage(new MCDroppodTagetBuiMsg(tile));
    }
}
