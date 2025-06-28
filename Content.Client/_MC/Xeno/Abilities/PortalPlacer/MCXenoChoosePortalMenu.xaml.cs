using Content.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;

namespace Content.Client._MC.Xeno.Abilities.PortalPlacer;

public sealed partial class MCXenoChoosePortalMenu : RadialMenu
{
    public MCXenoChoosePortalMenu()
    {
        RobustXamlLoader.Load(this);
    }
}
