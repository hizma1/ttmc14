using Robust.Shared.GameStates;

namespace Content.Shared._MC.Armor;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class MCArmorComponent : Component
{
    [DataField, AutoNetworkedField]
    public int Melee;

    [DataField, AutoNetworkedField]
    public int Bullet;

    [DataField, AutoNetworkedField]
    public int Laser;

    [DataField, AutoNetworkedField]
    public int Energy;

    [DataField, AutoNetworkedField]
    public int Bomb;

    [DataField, AutoNetworkedField]
    public int Bio;

    [DataField, AutoNetworkedField]
    public int Fire;

    [DataField, AutoNetworkedField]
    public int Acid;
}
