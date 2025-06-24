using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared._RMC14.Xenonids.Weeds;
using Content.Shared.Actions;
using Content.Shared.Administration.Logs;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.Database;
using Content.Shared.Popups;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Shared._MC.Xeno.Construction;

public sealed class MCXenoPlantingWeedsSystem : EntitySystem
{
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly IMapManager _map = default!;
    [Dependency] private readonly ISharedAdminLogManager _adminLogs = default!;

    [Dependency] private readonly SharedActionsSystem _actions = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly SharedMapSystem _mapSystem = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedUserInterfaceSystem _ui = default!;
    [Dependency] private readonly SharedXenoWeedsSystem _xenoWeeds = default!;
    [Dependency] private readonly SharedXenoHiveSystem _xenoHive = default!;
    [Dependency] private readonly XenoPlasmaSystem _xenoPlasma = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MCXenoPlantingWeedsComponent, MCXenoChooseWeedsBuiMsg>(OnChooseMessage);

        SubscribeLocalEvent<MCXenoPlantingWeedsComponent, MCXenoPlaceWeedsActionEvent>(OnPlaceEvent);
        SubscribeLocalEvent<MCXenoPlantingWeedsComponent, MCXenoChooseWeedsActionEvent>(OnChooseEvent);

        SubscribeLocalEvent<MCXenoChooseWeedsActionComponent, MCXenoWeedsChosenEvent>(OnActionWeedsChosen);
    }

    private void OnChooseMessage(Entity<MCXenoPlantingWeedsComponent> entity, ref MCXenoChooseWeedsBuiMsg args)
    {
        Select(entity, args.Id);
        _ui.CloseUi(entity.Owner, MCXenoPlantingWeedsUI.Key, entity);
    }

    private void OnChooseEvent(Entity<MCXenoPlantingWeedsComponent> entity, ref MCXenoChooseWeedsActionEvent args)
    {
        args.Handled = true;
        _ui.TryOpenUi(entity.Owner, MCXenoPlantingWeedsUI.Key, entity);
    }

    private void OnPlaceEvent(Entity<MCXenoPlantingWeedsComponent> entity, ref MCXenoPlaceWeedsActionEvent args)
    {
        if (entity.Comp.Selected is not { } weedsSelected)
            return;

        var weedsData = entity.Comp.Weeds[weedsSelected];

        var coordinates = _transform.GetMoverCoordinates(entity).SnapToGrid(EntityManager, _map);
        if (_transform.GetGrid(coordinates) is not { } gridUid || !TryComp(gridUid, out MapGridComponent? gridComp))
            return;

        var grid = new Entity<MapGridComponent>(gridUid, gridComp);
        if (_xenoWeeds.IsOnWeeds(grid, coordinates, true))
        {
            _popup.PopupClient(Loc.GetString("cm-xeno-weeds-source-already-here"), entity.Owner, entity.Owner);
            return;
        }

        var tile = _mapSystem.CoordinatesToTile(gridUid, gridComp, coordinates);
        if (!_xenoWeeds.CanSpreadWeedsPopup(grid, tile, entity, weedsData.SemiWeedable, true))
            return;

        if (!_xenoWeeds.CanPlaceWeedsPopup(entity, grid, coordinates, false))
            return;

        if (!_xenoPlasma.TryRemovePlasmaPopup(entity.Owner, weedsData.Cost))
            return;

        args.Handled = true;
        if (_net.IsServer)
        {
            var weeds = Spawn(weedsSelected, coordinates);
            _adminLogs.Add(LogType.RMCXenoPlantWeeds, $"Xeno {ToPrettyString(entity):xeno} planted weeds {ToPrettyString(weeds):weeds} at {coordinates}");
            _xenoHive.SetSameHive(entity.Owner, weeds);
        }

        _audio.PlayPredicted(weedsData.PlaceSound, coordinates, entity);
    }

    private void OnActionWeedsChosen(Entity<MCXenoChooseWeedsActionComponent> entity, ref MCXenoWeedsChosenEvent args)
    {
        if (!_actions.TryGetActionData(entity, out var action))
            return;

        action.Icon = args.Data.Sprite;
        Dirty(entity, action);
    }

    private void Select(Entity<MCXenoPlantingWeedsComponent> entity, EntProtoId id)
    {
        if (!entity.Comp.Weeds.ContainsKey(id))
            return;

        entity.Comp.Selected = id;
        DirtyField(entity, entity.Comp, nameof(MCXenoPlantingWeedsComponent.Selected));

        var ev = new MCXenoWeedsChosenEvent(id, entity.Comp.Weeds[id]);
        foreach (var (entityUid, _) in _actions.GetActions(entity))
        {
            RaiseLocalEvent(entityUid, ref ev);
        }
    }
}
