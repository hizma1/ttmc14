using Content.Shared.FixedPoint;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Content.Shared.Weapons.Ranged;

namespace Content.Shared._MC.Weapon.Range.Flamer;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(fieldDeltas: true), Access(typeof(SharedGunSystem), typeof(MCSolutionAmmoProviderSystem))]
public sealed partial class MCSolutionAmmoProviderComponent : Component, IShootable
{
    [DataField("solutionId"), AutoNetworkedField]
    public string SolutionId = string.Empty;

    [DataField, AutoNetworkedField]
    public int MaxIntensity = 20;

    [DataField, AutoNetworkedField]
    public int MaxDuration = 24;

    [DataField, AutoNetworkedField]
    public TimeSpan DelayPer = TimeSpan.FromSeconds(0.05);

    [DataField, AutoNetworkedField]
    public FixedPoint2 CostPer = FixedPoint2.New(1);

    [DataField("proto")]
    public EntProtoId Prototype;

    [DataField("fireProto")]
    public string FirePrototype = "MCFlamerFire";

    public bool Caseless => false;

    public EntityUid? GetEntity()
    {
        return Owner;
    }

    public (EntityUid Entity, IShootable Shootable)? TakeShot()
    {
        return (Owner, this);
    }

    [DataField("shotsProvided"), AutoNetworkedField]
    public int ShotsProvided = 0;

    public bool CanProvideAmmo(int requestedShots)
    {
        return true;
    }

}
