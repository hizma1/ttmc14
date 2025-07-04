using System.Numerics;
using Content.Client.UserInterface.Controls;
using Content.Shared._MC.Xeno.Construction;
using JetBrains.Annotations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Prototypes;

namespace Content.Client._MC.Xeno.Construction;

[UsedImplicitly]
public sealed class MCXenoChooseWeedsBui : BoundUserInterface
{
    [Dependency] private readonly IClyde _displayManager = default!;
    [Dependency] private readonly IInputManager _inputManager = default!;
    [Dependency] private readonly IPlayerManager _player = default!;
    [Dependency] private readonly IEyeManager _eye = default!;

    private readonly SpriteSystem _sprite;
    private readonly TransformSystem _transform;

    [ViewVariables]
    private MCXenoChooseWeedsMenu? _radialMenu;

    public MCXenoChooseWeedsBui(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
        IoCManager.InjectDependencies(this);

        _sprite = EntMan.System<SpriteSystem>();
        _transform = EntMan.System<TransformSystem>();
    }

    protected override void Open()
    {
        base.Open();

        _radialMenu = this.CreateWindow<MCXenoChooseWeedsMenu>();
        var parent = _radialMenu.FindControl<RadialContainer>("Main");

        if (EntMan.TryGetComponent<MCXenoPlantingWeedsComponent>(Owner, out var component))
        {
            foreach (var (id, data) in component.Weeds)
            {
                AddButton(id, data, parent);
            }

            AddButtonAuto(component, parent);
        }

        var vpSize = _displayManager.ScreenSize;
        var pos = _inputManager.MouseScreenPosition.Position / vpSize;

        if (EntMan.TryGetComponent<EyeComponent>(Owner, out var eyeComp) && eyeComp.Target is not null)
            pos = _eye.WorldToScreen(_transform.GetMapCoordinates((EntityUid)eyeComp.Target).Position) / vpSize;
        else if (_player.LocalEntity is { } ent)
            pos = _eye.WorldToScreen(_transform.GetMapCoordinates(ent).Position) / vpSize;

        _radialMenu.OpenCenteredAt(pos);
    }

    private void AddButton(EntProtoId id, MCXenoPlantingWeedsComponent.WeedEntry data, RadialContainer parent)
    {
        var texture = new TextureRect
        {
            VerticalAlignment = Control.VAlignment.Center,
            HorizontalAlignment = Control.HAlignment.Center,
            Texture = _sprite.Frame0(data.Sprite),
            TextureScale = new Vector2(2f, 2f),
        };

        var button = new RadialMenuTextureButton
        {
            StyleClasses = { "RadialMenuButton" },
            SetSize = new Vector2(64, 64),
            ToolTip = data.Name,
        };

        button.OnButtonDown += _ => SendPredictedMessage(new MCXenoChooseWeedsBuiMsg(id));

        button.AddChild(texture);
        parent.AddChild(button);
    }

    private void AddButtonAuto(MCXenoPlantingWeedsComponent component, RadialContainer parent)
    {
        var texture = new TextureRect
        {
            VerticalAlignment = Control.VAlignment.Center,
            HorizontalAlignment = Control.HAlignment.Center,
            Texture = _sprite.Frame0(component.AutoSprite),
            TextureScale = new Vector2(2f, 2f),
        };

        var button = new RadialMenuTextureButton
        {
            StyleClasses = { "RadialMenuButton" },
            SetSize = new Vector2(64, 64),
        };

        button.OnButtonDown += _ => SendPredictedMessage(new MCXenoChooseAutoWeedsBuiMsg(!component.Auto));

        button.AddChild(texture);
        parent.AddChild(button);
    }
}
