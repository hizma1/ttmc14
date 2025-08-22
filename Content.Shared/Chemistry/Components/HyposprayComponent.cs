using Content.Shared.FixedPoint;
using Robust.Shared.GameStates;
using Robust.Shared.Audio;

namespace Content.Shared.Chemistry.Components;

/// <summary>
///     Component that allows an entity instantly transfer liquids by interacting with objects that have solutions.
/// </summary>
[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState]
public sealed partial class HyposprayComponent : Component
{
    /// <summary>
    ///     Solution that will be used by hypospray for injections.
    /// </summary>
    [DataField]
    public string SolutionName = "hypospray";

    /// <summary>
    ///     Возможные объёмы для инъекции.
    /// </summary>
    [DataField]
    public FixedPoint2[] TransferAmounts = new[]
    {
        FixedPoint2.New(1),
        FixedPoint2.New(3),
        FixedPoint2.New(5),
        FixedPoint2.New(10),
        FixedPoint2.New(15),
        FixedPoint2.New(20),
        FixedPoint2.New(30),
        FixedPoint2.New(60),
        FixedPoint2.New(120)
    };

    /// <summary>
    ///     Текущий объём для инъекции.
    /// </summary>
    [AutoNetworkedField]
    [DataField]
    public FixedPoint2 TransferAmount = FixedPoint2.New(10);

    /// <summary>
    ///     Sound that will be played when injecting.
    /// </summary>
    [DataField]
    public SoundSpecifier InjectSound = new SoundPathSpecifier("/Audio/Items/hypospray.ogg");

    /// <summary>
    /// Decides whether you can inject everything or just mobs.
    /// </summary>
    [AutoNetworkedField]
    [DataField(required: true)]
    public bool OnlyAffectsMobs = false;

    /// <summary>
    /// If this can draw from containers in mob-only mode.
    /// </summary>
    [AutoNetworkedField]
    [DataField]
    public bool CanContainerDraw = true;

    /// <summary>
    /// Whether or not the hypospray is able to draw from containers or if it's a single use
    /// device that can only inject.
    /// </summary>
    [DataField]
    public bool InjectOnly = false;
}
