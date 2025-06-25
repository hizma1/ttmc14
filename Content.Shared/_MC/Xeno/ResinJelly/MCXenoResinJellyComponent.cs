using Content.Shared.Chat.Prototypes;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._MC.Xeno.ResinJelly;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(fieldDeltas: true)]
public sealed partial class MCXenoResinJellyComponent : Component
{
    [DataField, AutoNetworkedField]
    public TimeSpan Duration = TimeSpan.FromSeconds(30);

    [DataField, AutoNetworkedField]
    public TimeSpan ApplyOtherDuration = TimeSpan.FromSeconds(1);

    [DataField, AutoNetworkedField]
    public TimeSpan ApplySelfDuration = TimeSpan.FromSeconds(2);

    [DataField, AutoNetworkedField]
    public Color AuraColor = Color.FromHex("#fe7b00");

    [DataField, AutoNetworkedField]
    public ProtoId<EmotePrototype> Emote = "XenoRoar";
}
