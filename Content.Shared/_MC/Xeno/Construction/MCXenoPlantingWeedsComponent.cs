using Content.Shared.FixedPoint;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared._MC.Xeno.Construction;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(fieldDeltas: true)]
public sealed partial class MCXenoPlantingWeedsComponent : Component
{
    [DataField, AutoNetworkedField]
    public EntProtoId? Selected;

    [DataField, AutoNetworkedField]
    public bool AutoPlace;

    [DataField, AutoNetworkedField]
    public Dictionary<EntProtoId, WeedEntry> Weeds = new();

    [DataDefinition, Serializable, NetSerializable]
    public sealed partial class WeedEntry
    {
        [DataField]
        public LocId Name;

        [DataField]
        public FixedPoint2 Cost;

        [DataField]
        public SpriteSpecifier.Rsi Sprite;

        [DataField]
        public bool SemiWeedable;

        [DataField]
        public SoundSpecifier PlaceSound = new SoundCollectionSpecifier("RMCResinBuild")
        {
            Params = AudioParams.Default.WithVolume(-10f),
        };
    }
}

[Serializable, NetSerializable]
public enum MCXenoPlantingWeedsUI
{
    Key,
}

[Serializable, NetSerializable]
public sealed class MCXenoChooseWeedsBuiMsg : BoundUserInterfaceMessage
{
    public readonly EntProtoId Id;

    public MCXenoChooseWeedsBuiMsg(EntProtoId id)
    {
        Id = id;
    }
}
