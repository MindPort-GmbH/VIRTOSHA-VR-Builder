/*
<ai_context>
  Condition that checks if a path has been followed via FollowPathProperty and FollowPathObjectProperty.
</ai_context>
*/

using Newtonsoft.Json;
using System.Runtime.Serialization;
using UnityEngine.Scripting;
using VRBuilder.Core;
using VRBuilder.Core.Attributes;
using VRBuilder.Core.Conditions;
using VRBuilder.Core.SceneObjects;
using VRBuilder.VIRTOSHA.Properties;

namespace VRBuilder.VIRTOSHA.Conditions
{
    /// <summary>
    /// Condition which becomes completed when the specified path is fully followed by the given object.
    /// </summary>
    [DataContract(IsReference = true)]
    public class FollowPathCondition : Condition<FollowPathCondition.EntityData>
    {
        /// <summary>
        /// Entity data for <see cref="FollowPathCondition"/>.
        /// </summary>
        public class EntityData : IConditionData
        {
            /// <summary>
            /// The object that follows the path (provides the tip transform).
            /// </summary>
            [DataMember]
            [DisplayName("Follow Path Object")]
            public SingleScenePropertyReference<IFollowPathObjectProperty> FollowPathObject { get; set; }

            /// <summary>
            /// The path property to follow.
            /// </summary>
            [DataMember]
            [DisplayName("Follow Path Property")]
            public SingleScenePropertyReference<IFollowPathProperty> FollowPath { get; set; }

            /// <inheritdoc/>
            public bool IsCompleted { get; set; }

            [IgnoreDataMember]
            [HideInProcessInspector]
            public string Name
            {
                get
                {
                    return $"Follow Spline Path";
                }
            }

            public Metadata Metadata { get; set; }
        }

        private class ActiveProcess : BaseActiveProcessOverCompletable<EntityData>
        {
            public ActiveProcess(EntityData data) : base(data)
            {
            }

            protected override bool CheckIfCompleted()
            {
                // Check if path is fully followed.
                IFollowPathProperty pathProperty = Data.FollowPath.Value;
                return pathProperty != null && pathProperty.IsPathCompleted;
            }
        }

        private class EntityAutocompleter : Autocompleter<EntityData>
        {
            public EntityAutocompleter(EntityData data) : base(data)
            {
            }

            public override void Complete()
            {
                // Mark path as completed
                if (Data.FollowPath.Value != null)
                {
                    Data.FollowPath.Value.CompletePath();
                }
            }
        }

        [JsonConstructor, Preserve]
        public FollowPathCondition()
        {
            Data.FollowPathObject = new SingleScenePropertyReference<IFollowPathObjectProperty>();
            Data.FollowPath = new SingleScenePropertyReference<IFollowPathProperty>();
        }

        public override IStageProcess GetActiveProcess()
        {
            return new ActiveProcess(Data);
        }

        protected override IAutocompleter GetAutocompleter()
        {
            return new EntityAutocompleter(Data);
        }
    }
}