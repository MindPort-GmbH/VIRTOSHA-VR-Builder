using VRBuilder.Core.Conditions;
using VRBuilder.Editor.UI.StepInspector.Menu;
using VRBuilder.VIRTOSHA.Conditions;

namespace VRBuilder.Editor.VIRTOSHA.UI.Conditions
{
    /// <inheritdoc />
    public class DrillHolesConditionMenuItem : MenuItem<ICondition>
    {
        /// <inheritdoc />
        public override string DisplayedName { get; } = "VIRTOSHA/Drill holes";

        /// <inheritdoc />
        public override ICondition GetNewItem()
        {
            return new DrillHolesCondition();
        }
    }
}
