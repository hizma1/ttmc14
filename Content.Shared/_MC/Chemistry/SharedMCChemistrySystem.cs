using System.Linq;
using Content.Shared._RMC14.Chemistry.Reagent;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameStates;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._MC.Chemistry;

public abstract class SharedMCChemistrySystem : EntitySystem
{
    [Dependency] private readonly SharedContainerSystem _container = default!;
    [Dependency] private readonly EntityWhitelistSystem _entityWhitelist = default!;
    [Dependency] private readonly ItemSlotsSystem _itemSlots = default!;
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly IPrototypeManager _prototypes = default!;
    [Dependency] private readonly SharedSolutionContainerSystem _solution = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    private readonly List<Entity<MCChemicalDispenserComponent>> _dispensers = new();

    public override void Initialize()
    {
        SubscribeLocalEvent<SolutionComponent, ComponentGetState>(OnSolutionGetState);
        SubscribeLocalEvent<SolutionComponent, ComponentHandleState>(OnSolutionHandleState);

        SubscribeLocalEvent<MCDetailedExaminableSolutionComponent, ExaminedEvent>(OnDetailedSolutionExamined);

        SubscribeLocalEvent<MCChemicalDispenserComponent, MapInitEvent>(OnDispenserMapInit);

        SubscribeLocalEvent<MCToggleableSolutionTransferComponent, MapInitEvent>(OnToggleableSolutionTransferMapInit);
        SubscribeLocalEvent<MCToggleableSolutionTransferComponent, GetVerbsEvent<AlternativeVerb>>(OnToggleableSolutionTransferVerbs);

        SubscribeLocalEvent<MCSolutionTransferWhitelistComponent, SolutionTransferAttemptEvent>(OnTransferWhitelistAttempt);

        SubscribeLocalEvent<MCNoMixingReagentsComponent, ExaminedEvent>(OnNoMixingReagentsExamined);
        SubscribeLocalEvent<MCNoMixingReagentsComponent, SolutionTransferAttemptEvent>(OnNoMixingReagentsTransferAttempt);

        SubscribeLocalEvent<MCEmptySolutionComponent, GetVerbsEvent<AlternativeVerb>>(OnEmptySolutionGetVerbs);

        Subs.BuiEvents<MCChemicalDispenserComponent>(MCChemicalDispenserUi.Key,
            subs =>
            {
                subs.Event<MCChemicalDispenserDispenseSettingBuiMsg>(OnChemicalDispenserSettingMsg);
                subs.Event<MCChemicalDispenserBeakerBuiMsg>(OnChemicalDispenserBeakerSettingMsg);
                subs.Event<MCChemicalDispenserEjectBeakerBuiMsg>(OnChemicalDispenserEjectBeakerMsg);
                subs.Event<MCChemicalDispenserDispenseBuiMsg>(OnChemicalDispenserDispenseMsg);
            });
    }

    private void OnSolutionGetState(Entity<SolutionComponent> ent, ref ComponentGetState args)
    {
        var s = new Solution(ent.Comp.Solution, _prototypes);
        args.State = new SolutionComponentState(s);
    }

    private void OnSolutionHandleState(Entity<SolutionComponent> ent, ref ComponentHandleState args)
    {
        if (args.Current is not SolutionComponentState s)
            return;

        ent.Comp.Solution = new Solution(s.Solution, _prototypes);
    }

    private void OnDetailedSolutionExamined(Entity<MCDetailedExaminableSolutionComponent> ent, ref ExaminedEvent args)
    {
        using (args.PushGroup(nameof(MCDetailedExaminableSolutionComponent)))
        {
            args.PushText("It contains:");
            if (!_solution.TryGetSolution(ent.Owner, ent.Comp.Solution, out _, out var solution) ||
                solution.Volume <= FixedPoint2.Zero)
            {
                args.PushText("Nothing.");
            }
            else
            {
                foreach (var reagent in solution.Contents)
                {
                    var name = reagent.Reagent.Prototype;
                    if (_prototypes.TryIndexReagent(reagent.Reagent.Prototype, out ReagentPrototype? reagentProto))
                        name = reagentProto.LocalizedName;

                    args.PushText($"{reagent.Quantity.Float():F2} units of {name}");
                }

                args.PushText($"Total volume: {solution.Volume} / {solution.MaxVolume}.");
            }
            var transferComp = EntityManager.GetComponent<MCToggleableSolutionTransferComponent>(ent.Owner);
            var directionText = transferComp.Direction switch
            {
                SolutionTransferDirection.Input => "Transfer mode: Drawing",
                SolutionTransferDirection.Output => "Transfer mode: Dispensing",
                _ => string.Empty,
            };
            if (!string.IsNullOrEmpty(directionText))
                args.PushText(directionText);
        }
    }

    private void OnDispenserMapInit(Entity<MCChemicalDispenserComponent> ent, ref MapInitEvent args)
    {
        _container.EnsureContainer<ContainerSlot>(ent, ent.Comp.ContainerSlotId);
    }

    private void OnToggleableSolutionTransferMapInit(Entity<MCToggleableSolutionTransferComponent> ent, ref MapInitEvent args)
    {
        ent.Comp.Direction = SolutionTransferDirection.Input;
        RemCompDeferred<DrainableSolutionComponent>(ent);
        var refillable = EnsureComp<RefillableSolutionComponent>(ent);
        refillable.Solution = ent.Comp.Solution;
        Dirty(ent, refillable);
    }

    private void OnToggleableSolutionTransferVerbs(Entity<MCToggleableSolutionTransferComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
    {
        if (!args.CanAccess || !args.CanInteract)
            return;

        var user = args.User;
        var dispensing = HasComp<DrainableSolutionComponent>(ent);
        args.Verbs.Add(new AlternativeVerb
        {
            Text = dispensing ? "Enable drawing" : "Enable dispensing",
            Act = () =>
            {
                dispensing = HasComp<DrainableSolutionComponent>(ent);
                if (dispensing)
                {
                    RemCompDeferred<DrainableSolutionComponent>(ent);
                    var refillable = EnsureComp<RefillableSolutionComponent>(ent);
                    refillable.Solution = ent.Comp.Solution;
                    ent.Comp.Direction = SolutionTransferDirection.Input;
                    Dirty(ent, refillable);
                    _popup.PopupClient("Now drawing", ent, user, PopupType.Medium);
                }
                else
                {
                    RemCompDeferred<RefillableSolutionComponent>(ent);
                    var drainable = EnsureComp<DrainableSolutionComponent>(ent);
                    drainable.Solution = ent.Comp.Solution;
                    ent.Comp.Direction = SolutionTransferDirection.Output;
                    Dirty(ent, drainable);
                    _popup.PopupClient("Now dispensing", ent, user, PopupType.Medium);
                }
            },
        });
    }

    private void OnTransferWhitelistAttempt(Entity<MCSolutionTransferWhitelistComponent> ent, ref SolutionTransferAttemptEvent args)
    {
        if (ent.Owner == args.From)
        {
            if (_entityWhitelist.IsWhitelistFail(ent.Comp.SourceWhitelist, args.To))
                args.Cancel(Loc.GetString(ent.Comp.Popup));
        }
        else
        {
            if (_entityWhitelist.IsWhitelistFail(ent.Comp.TargetWhitelist, args.From))
                args.Cancel(Loc.GetString(ent.Comp.Popup));
        }
    }

    private void OnNoMixingReagentsExamined(Entity<MCNoMixingReagentsComponent> ent, ref ExaminedEvent args)
    {
        using (args.PushGroup(nameof(MCNoMixingReagentsComponent)))
        {
            args.PushMarkup(Loc.GetString("rmc-fuel-examine-cant-mix"));
        }
    }

    private void OnNoMixingReagentsTransferAttempt(Entity<MCNoMixingReagentsComponent> ent, ref SolutionTransferAttemptEvent args)
    {
        var tankSolution = args.FromSolution.Comp.Solution;
        var targetSolution = args.ToSolution.Comp.Solution;
        if (targetSolution.Contents.Count > 1)
        {
            args.Cancel(Loc.GetString("rmc-fuel-cant-mix"));
            return;
        }

        foreach (var content in targetSolution.Contents)
        {
            if (tankSolution.Volume > FixedPoint2.Zero &&
                !tankSolution.ContainsReagent(content.Reagent))
            {
                args.Cancel(Loc.GetString("rmc-fuel-cant-mix"));
                return;
            }
        }
    }

    private void OnEmptySolutionGetVerbs(Entity<MCEmptySolutionComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
    {
        if (!args.CanAccess || !args.CanComplexInteract)
            return;

        if (!_solution.TryGetSolution(ent.Owner, ent.Comp.Solution, out var solutionEnt, out _) ||
            solutionEnt.Value.Comp.Solution.Volume <= FixedPoint2.Zero)
        {
            return;
        }

        args.Verbs.Add(new AlternativeVerb
        {
            Text = Loc.GetString("rmc-empty-solution-verb"),
            Act = () =>
            {
                if (_solution.TryGetSolution(ent.Owner, ent.Comp.Solution, out solutionEnt, out _))
                    _solution.RemoveAllSolution(solutionEnt.Value);
            },
            Priority = 1,
        });
    }

    private void OnChemicalDispenserSettingMsg(Entity<MCChemicalDispenserComponent> ent, ref MCChemicalDispenserDispenseSettingBuiMsg args)
    {
        if (!ent.Comp.Settings.Contains(args.Amount))
            return;

        ent.Comp.DispenseSetting = args.Amount;
        Dirty(ent);
    }

    private void OnChemicalDispenserBeakerSettingMsg(Entity<MCChemicalDispenserComponent> ent, ref MCChemicalDispenserBeakerBuiMsg args)
    {
        if (!_itemSlots.TryGetSlot(ent, ent.Comp.ContainerSlotId, out var slot) ||
            slot.ContainerSlot?.ContainedEntity is not { } contained ||
            !_solution.TryGetMixableSolution(contained, out var solutionEnt, out _) ||
            !ent.Comp.Settings.Contains(args.Amount))
        {
            return;
        }

        _solution.SplitSolution(solutionEnt.Value, args.Amount);
        DispenserUpdated(ent);
    }

    private void OnChemicalDispenserEjectBeakerMsg(Entity<MCChemicalDispenserComponent> ent, ref MCChemicalDispenserEjectBeakerBuiMsg args)
    {
        if (!_itemSlots.TryGetSlot(ent, ent.Comp.ContainerSlotId, out var slot))
            return;

        _itemSlots.TryEjectToHands(ent, slot, args.Actor, true);
        Dirty(ent);
    }

    private void OnChemicalDispenserDispenseMsg(Entity<MCChemicalDispenserComponent> ent, ref MCChemicalDispenserDispenseBuiMsg args)
    {
        if (!_itemSlots.TryGetSlot(ent, ent.Comp.ContainerSlotId, out var slot) ||
            slot.ContainerSlot?.ContainedEntity is not { } contained ||
            !_solution.TryGetMixableSolution(contained, out var solutionEnt, out _) ||
            !ent.Comp.Reagents.Contains(args.Reagent) ||
            !TryGetStorage(ent.Comp.Network, out var storage))
        {
            return;
        }

        var dispense = ent.Comp.DispenseSetting;
        var available = solutionEnt.Value.Comp.Solution.AvailableVolume;
        if (dispense > available)
            dispense = available;

        if (dispense == FixedPoint2.Zero)
            return;

        var cost = ent.Comp.CostPerUnit * dispense;
        if (cost > storage.Comp.Energy)
            return;

        ChangeStorageEnergy(storage, storage.Comp.Energy - cost);
        _solution.TryAddReagent(solutionEnt.Value, args.Reagent, ent.Comp.DispenseSetting);
    }

    public bool TryGetStorage(EntProtoId network, out Entity<MCChemicalStorageComponent> storage)
    {
        var storages = EntityQueryEnumerator<MCChemicalStorageComponent>();
        while (storages.MoveNext(out var storageId, out var storageComp))
        {
            if (IsClientSide(storageId))
                continue;

            if (storageComp.Network == network)
            {
                storage = (storageId, storageComp);
                return true;
            }
        }

        storage = default;
        return false;
    }

    public void ChangeStorageEnergy(Entity<MCChemicalStorageComponent> storage, FixedPoint2 energy)
    {
        storage.Comp.Energy = FixedPoint2.Clamp(energy, FixedPoint2.Zero, storage.Comp.MaxEnergy);
        Dirty(storage);

        var dispensers = EntityQueryEnumerator<MCChemicalDispenserComponent>();
        while (dispensers.MoveNext(out var dispenserId, out var dispenserComp))
        {
            if (dispenserComp.Network != storage.Comp.Network)
                continue;

            dispenserComp.Energy = energy;
            Dirty(dispenserId, dispenserComp);
        }
    }

    protected virtual void DispenserUpdated(Entity<MCChemicalDispenserComponent> ent)
    {
    }

    public override void Update(float frameTime)
    {
        if (_net.IsClient)
            return;

        var time = _timing.CurTime;
        var storages = EntityQueryEnumerator<MCChemicalStorageComponent>();
        while (storages.MoveNext(out var storageId, out var storage))
        {
            if (time < storage.RechargeAt)
                continue;

            storage.RechargeAt = time + storage.RechargeEvery;
            Dirty(storageId, storage);

            _dispensers.Clear();
            var dispensers = EntityQueryEnumerator<MCChemicalDispenserComponent>();
            while (dispensers.MoveNext(out var dispenserId, out var dispenser))
            {
                if (dispenser.Network == storage.Network)
                    _dispensers.Add((dispenserId, dispenser));
            }

            storage.MaxEnergy = storage.BaseMax + storage.MaxPer * _dispensers.Count;
            storage.Recharge = storage.BaseRecharge + storage.RechargePer * _dispensers.Count;

            if (!storage.Updated)
            {
                storage.Updated = true;
                storage.Energy = storage.MaxEnergy;
            }
            else
            {
                storage.Energy = FixedPoint2.Min(storage.MaxEnergy, storage.Energy + storage.Recharge);
            }

            foreach (var dispenser in _dispensers)
            {
                dispenser.Comp.Energy = storage.Energy;
                dispenser.Comp.MaxEnergy = storage.MaxEnergy;
                Dirty(dispenser);
            }
        }
    }
}
