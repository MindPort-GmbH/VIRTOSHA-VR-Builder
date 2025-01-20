/*
<ai_context>
  Menu item to add the FollowPathCondition through the VR Builder step inspector.
</ai_context>
*/

using VRBuilder.Core.Conditions;
using VRBuilder.Core.Editor.UI.StepInspector.Menu;
using VRBuilder.VIRTOSHA.Conditions;

namespace VRBuilder.Editor.VIRTOSHA.UI.Conditions
{
    /// <summary>
    /// Menu item for creating a FollowPathCondition.
    /// </summary>
    public class FollowPathConditionMenuItem : MenuItem<ICondition>
    {
        public override string DisplayedName { get; } = "VIRTOSHA/Follow Path";

        public override ICondition GetNewItem()
        {
            return new FollowPathCondition();
        }
    }
}