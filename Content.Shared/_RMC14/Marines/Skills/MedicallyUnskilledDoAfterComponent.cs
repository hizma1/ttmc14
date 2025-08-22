using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Marines.Skills;

// TODO RMC14 make this more general
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(SkillsSystem))]
public sealed partial class MedicallyUnskilledDoAfterComponent : Component
{
    // TODO RMC14 use Skills struct and IncludeDataField
    [DataField, AutoNetworkedField]
    public int Min = 1;

    /// <summary>
    /// Задержка для пользователя с навыком
    /// </summary>
    [DataField, AutoNetworkedField]
    public TimeSpan SkilledDelay = TimeSpan.FromSeconds(0);

    /// <summary>
    /// Задержка для пользователя без навыка (для совместимости с прототипами)
    /// </summary>
    [DataField, AutoNetworkedField]
    public TimeSpan DoAfter = TimeSpan.FromSeconds(3);

    [DataField, AutoNetworkedField]
    public EntProtoId<SkillDefinitionComponent> Skill = "RMCSkillMedical";
}
