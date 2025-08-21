using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Weapons.Ranged;
using Content.Shared.Hands;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;

namespace Content.Shared._MC.Weapon.Range;

public sealed class MCGunSkilledSystem : EntitySystem
{
    [Dependency] private readonly SkillsSystem _rmcSkills = default!;
    [Dependency] private readonly CMGunSystem _rmcGun = default!;
    [Dependency] private readonly SharedGunSystem _gun = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MCGunSkilledComponent, GotEquippedHandEvent>(TryRefreshGunModifiers);
        SubscribeLocalEvent<MCGunSkilledComponent, GotUnequippedHandEvent>(TryRefreshGunModifiers);
        SubscribeLocalEvent<MCGunSkilledComponent, GunRefreshModifiersEvent>(OnGunRefreshModifiers);
    }

    private void TryRefreshGunModifiers<TComp, TEvent>(Entity<TComp> ent, ref TEvent args) where TComp : IComponent?
    {
        if (TryComp<GunComponent>(ent, out var gun))
            _gun.RefreshModifiers((ent, gun));
    }

    private void OnGunRefreshModifiers(Entity<MCGunSkilledComponent> entity, ref GunRefreshModifiersEvent args)
    {
        if (!TryGetUserSkills(entity, out var user))
            return;

        var skill = _rmcSkills.GetSkill((user, user), entity.Comp.Skill);
        args.MinAngle -= skill * 2;
        args.MaxAngle -= skill * 2;
        args.CameraRecoilScalar = Math.Max(0, args.CameraRecoilScalar - skill * 2);
    }

    private bool TryGetUserSkills(EntityUid gun, out Entity<SkillsComponent> user)
    {
        user = default;
        if (!_rmcGun.TryGetGunUser(gun, out var gunUser) || !TryComp<SkillsComponent>(gunUser, out var skills))
            return false;

        user = (gunUser, skills);
        return true;
    }
}
