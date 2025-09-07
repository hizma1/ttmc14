using Robust.Shared;
using Robust.Shared.Configuration;

namespace Content.Shared._MC;

[CVarDefs]
public sealed partial class MCCVars : CVars
{
    public static readonly CVarDef<float> MCRespawnActionCooldownMinutes =
        CVarDef.Create("mc.respawn_action_delay_minutes", 10f, CVar.SERVER | CVar.REPLICATED);
}
