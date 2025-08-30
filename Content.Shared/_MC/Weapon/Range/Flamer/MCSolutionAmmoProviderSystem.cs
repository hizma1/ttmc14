using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Interaction.Events;
using Robust.Shared.Map;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Random;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using System;

namespace Content.Shared._MC.Weapon.Range.Flamer
{
    public sealed class MCSolutionAmmoProviderSystem : EntitySystem
    {
        [Dependency] private readonly SharedSolutionContainerSystem _solution = default!;
        [Dependency] private readonly SharedAudioSystem _audio = default!;
        [Dependency] private readonly INetManager _net = default!;

        public override void Initialize()
        {
            SubscribeLocalEvent<MCSolutionAmmoProviderComponent, TakeAmmoEvent>(OnTakeAmmo);
            SubscribeLocalEvent<MCSolutionAmmoProviderComponent, GetAmmoCountEvent>(OnGetAmmoCount);
        }

        private void OnTakeAmmo(EntityUid uid, MCSolutionAmmoProviderComponent component, TakeAmmoEvent args)
        {
            if (component.CanProvideAmmo(args.Shots))
            {
                args.Ammo.Add((uid, component));
                component.ShotsProvided += args.Shots;
            }
        }

        private void OnGetAmmoCount(EntityUid uid, MCSolutionAmmoProviderComponent component, ref GetAmmoCountEvent args)
        {
            Entity<SolutionComponent>? solEnt;
            Solution? solution;

            Entity<SolutionContainerManagerComponent?> containerUid = uid;
            if (!_solution.TryGetSolution(containerUid, component.SolutionId, out solEnt, out solution))
            {
                if (EntityManager.TryGetComponent(uid, out SolutionContainerManagerComponent? mgr))
                {
                    Entity<SolutionContainerManagerComponent?> container = (uid, mgr);
                    if (!_solution.TryGetSolution(container, component.SolutionId, out solEnt, out solution))
                        return;
                }
                else
                {
                    return;
                }
            }
            if (solution == null)
                return;

            args.Count = solution.Volume.Int();
            args.Capacity = solution.MaxVolume.Int();
        }

        public void ShootFlamer(EntityUid providerUid, EntityUid gunUid, EntityUid? userUid, EntityCoordinates from, EntityCoordinates to)
        {
            if (!EntityManager.TryGetComponent(providerUid, out MCSolutionAmmoProviderComponent? comp))
                return;

            Entity<SolutionComponent>? solEnt;
            Solution? solution;

            Entity<SolutionContainerManagerComponent?> providerContainer = providerUid;
            if (!_solution.TryGetSolution(providerContainer, comp.SolutionId, out solEnt, out solution))
            {
                if (EntityManager.TryGetComponent(providerUid, out SolutionContainerManagerComponent? mgr))
                {
                    Entity<SolutionContainerManagerComponent?> container = (providerUid, mgr);
                    if (!_solution.TryGetSolution(container, comp.SolutionId, out solEnt, out solution))
                        return;
                }
                else
                {
                    return;
                }
            }
            if (solution == null)
                return;

            if (solution.Volume < comp.CostPer)
                return;

            if (solution.Contents.Count > 0)
            {
                var reagent = solution.Contents[0].Reagent;
                solution.RemoveReagent(reagent, (int)comp.CostPer);
            }
            else
            {
                solution.RemoveSolution(comp.CostPer);
            }

            if (solEnt == null)
                return;
            _solution.UpdateChemicals(solEnt.Value);

            if (TryComp<GunComponent>(gunUid, out var gun))
            {
                _audio.PlayPredicted(gun.SoundGunshotModified, gunUid, userUid);
            }

            if (_net.IsClient)
                return;

            if (!string.IsNullOrWhiteSpace(comp.FirePrototype))
            {
                var fire = EntityManager.SpawnEntity(comp.FirePrototype, to);
                var fireComp = EntityManager.GetComponentOrNull<MetaDataComponent>(fire);
                // TODO: параметризовать интенсивность/длительность по reagent
            }
        }

        private void OnShoot(EntityUid uid, GunComponent component, ref GunShotEvent args)
        {
            if (!TryGetAmmoProvider(uid, out var provider))
                return;

            ShootFlamer(provider.Uid, uid, args.User, args.FromCoordinates, args.ToCoordinates);
        }

        private bool TryGetAmmoProvider(EntityUid gunUid, out (EntityUid Uid, MCSolutionAmmoProviderComponent Comp) provider)
        {
            if (EntityManager.TryGetComponent<MCSolutionAmmoProviderComponent>(gunUid, out var comp))
            {
                provider = (gunUid, comp);
                return true;
            }

            provider = default;
            return false;
        }

        private bool HasEnoughAmmo(MCSolutionAmmoProviderComponent comp, int shots)
        {
            Entity<SolutionComponent>? solEnt;
            Solution? solution;

            Entity<SolutionContainerManagerComponent?> ownerContainer = comp.Owner;
            if (!_solution.TryGetSolution(ownerContainer, comp.SolutionId, out solEnt, out solution))
            {
                if (EntityManager.TryGetComponent(comp.Owner, out SolutionContainerManagerComponent? mgr))
                {
                    Entity<SolutionContainerManagerComponent?> container = (comp.Owner, mgr);
                    if (!_solution.TryGetSolution(container, comp.SolutionId, out solEnt, out solution))
                        return false;
                }
                else
                {
                    return false;
                }
            }

            if (solution == null)
                return false;

            return solution.Volume >= comp.CostPer * shots;
        }
    }
}
