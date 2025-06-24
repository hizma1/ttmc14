using Content.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;

namespace Content.Client._MC.Xeno.Construction;

public sealed partial class MCXenoChooseWeedsMenu : RadialMenu
{
    public MCXenoChooseWeedsMenu()
    {
        RobustXamlLoader.Load(this);
    }
}
