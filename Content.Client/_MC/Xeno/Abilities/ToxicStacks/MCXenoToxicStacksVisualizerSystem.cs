using Content.Shared._MC.Xeno.Abilities.Evasion;
using Content.Shared._MC.Xeno.Abilities.ToxicStacks;
using Content.Shared._RMC14.Xenonids;
using Robust.Client.GameObjects;
using Robust.Client.Player;

namespace Content.Client._MC.Xeno.Abilities.ToxicStacks;

public sealed class MCXenoEvasionVisualizerSystem : VisualizerSystem<MCXenoToxicStacksComponent>
{
    [Dependency] private readonly IPlayerManager _player = default!;

    protected override void OnAppearanceChange(EntityUid uid, MCXenoToxicStacksComponent component, ref AppearanceChangeEvent args)
    {
        base.OnAppearanceChange(uid, component, ref args);

        if (_player.LocalEntity is null || !HasComp<XenoComponent>(_player.LocalEntity))
            return;

        if (args.Sprite is null)
            return;

        if (!AppearanceSystem.TryGetData<int>(uid, MCXenoToxicStacksVisuals.Visuals, out var value, args.Component))
            return;

        if (!args.Sprite.LayerMapTryGet(MCXenoToxicStacksLayer.Base, out var layer) ||
            !args.Sprite.LayerMapTryGet(MCXenoToxicStacksLayer.Icon, out var iconLayer))
            return;

        var visible = value > 0;
        args.Sprite.LayerSetVisible(layer, visible);
        args.Sprite.LayerSetVisible(iconLayer, visible);

        args.Sprite.LayerSetState(layer, $"intoxicated_amount{value}");
    }
}
