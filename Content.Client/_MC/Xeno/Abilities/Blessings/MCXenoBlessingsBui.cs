using Content.Shared._MC.Xeno.Blessings;
using Content.Shared._MC.Xeno.Blessings.Prototypes;
using JetBrains.Annotations;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Prototypes;

namespace Content.Client._MC.Xeno.Abilities.Blessings;

[UsedImplicitly]
public sealed class MCXenoBlessingsBui : BoundUserInterface
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    private readonly Dictionary<ProtoId<MCXenoBlessingsGroupPrototype>, int> _groupMapping = new();
    private int _groupId;

    [ViewVariables]
    private MCXenoBlessingsWindow? _window;

    public MCXenoBlessingsBui(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindow<MCXenoBlessingsWindow>();

        if (!EntMan.TryGetComponent<MCXenoBlessingsComponent>(Owner, out var component))
            return;

        foreach (var entry in component.Entries)
        {
            AddEntry(entry);
        }
    }

    private void AddEntry(ProtoId<MCXenoBlessingsEntryPrototype> id)
    {
        if (_window is null)
            return;

        if (!_prototype.TryIndex(id, out var prototype))
            return;

        if (!_prototype.TryIndex(prototype.Group, out var group))
            return;

        if (!_groupMapping.TryGetValue(prototype.Group, out var groupId))
        {
            groupId = _groupId++;

            var scroll = new ScrollContainer();
            _window.GroupContainer.AddChild(scroll);

            _groupMapping.Add(prototype.Group, groupId);
            _window.GroupContainer.SetTabTitle(groupId, Loc.GetString(group.Title));
        }
    }
}
