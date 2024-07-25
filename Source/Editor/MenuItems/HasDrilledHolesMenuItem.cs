using VRBuilder.Core.Conditions;
using VRBuilder.Editor.UI.StepInspector.Menu;
using VRBuilder.VIRTOSHA.Conditions;

namespace VRBuilder.Editor.VIRTOSHA.UI.Conditions
{
    /// <inheritdoc />
    public class HasDrilledHolesMenuItem : MenuItem<ICondition>
    {
        /// <inheritdoc />
        public override string DisplayedName { get; } = "VIRTOSHA/Has drilled holes";

        /// <inheritdoc />
        public override ICondition GetNewItem()
        {
            return new HasDrilledHolesCondition();
        }
    }
}
