using Content.Client.CombatMode;
using Content.Client.Gameplay;
using Content.Shared._MC.Xeno.Spit;
using Content.Shared.ActionBlocker;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.State;
using Robust.Shared.Input;
using Robust.Shared.Map;
using Robust.Shared.Timing;

namespace Content.Client._MC.Xeno.Spit;

public sealed class MCXenoSpitSystem : MCSharedXenoSpitSystem
{
    [Dependency] private readonly IStateManager _state = default!;
    [Dependency] private readonly IEyeManager _eye = default!;
    [Dependency] private readonly IInputManager _inputManager = default!;
    [Dependency] private readonly IMapManager _mapManager  = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IPlayerManager _player = default!;

    [Dependency] private readonly CombatModeSystem _combatMode = default!;
    [Dependency] private readonly InputSystem _input = default!;
    [Dependency] private readonly TransformSystem _transform = default!;
    [Dependency] private readonly MapSystem _map = default!;

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if (!_timing.IsFirstTimePredicted)
            return;

        if (_player.LocalEntity is not { } entityUid)
            return;

        if (!_combatMode.IsInCombatMode(entityUid))
            return;

        var keyState = _input.CmdStates.GetState(EngineKeyFunctions.UseSecondary);
        if (keyState != BoundKeyState.Down)
            return;

        var mousePosition = _eye.PixelToMap(_inputManager.MouseScreenPosition);
        if (mousePosition.MapId == MapId.Nullspace)
            return;

        var coordinates = _mapManager.TryFindGridAt(mousePosition, out var gridUid, out _)
            ? _transform.ToCoordinates(gridUid, mousePosition)
            : _transform.ToCoordinates(_map.GetMap(mousePosition.MapId), mousePosition);

        var target = _state.CurrentState is GameplayStateBase screen
            ? GetNetEntity(screen.GetClickedEntity(mousePosition))
            : null;

        RaisePredictiveEvent(new MCXenoSpitEvent(
            target,
            GetNetEntity(entityUid),
            GetNetCoordinates(coordinates)
        ));
    }
}
