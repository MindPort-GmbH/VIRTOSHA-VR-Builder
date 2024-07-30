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
    /// Condition which becomes completed when a hole is drilled at the required position.
    /// </summary>
    [DataContract(IsReference = true)]
    [HelpLink("https://www.mindport.co/vr-builder/manual/default-conditions/use-object")]
    public class HasDrilledHolesCondition : Condition<HasDrilledHolesCondition.EntityData>
    {
        public class EntityData : IConditionData
        {
            [DataMember]
            [DisplayName("Drillable object")]
            public SingleScenePropertyReference<IDrillableProperty> DrillableObject { get; set; }

            [DataMember]
            [DisplayName("Drillable sockets")]
            public MultipleScenePropertyReference<IDrillableSocketProperty> DrillableSockets { get; set; }

            public bool IsCompleted { get; set; }

            [IgnoreDataMember]
            [HideInProcessInspector]
            public string Name => $"Drill {DrillableObject}";

            public Metadata Metadata { get; set; }
        }

        private class ActiveProcess : BaseActiveProcessOverCompletable<EntityData>
        {
            public ActiveProcess(EntityData data) : base(data)
            {
            }

            protected override bool CheckIfCompleted()
            {
                bool isCompleted = true;

                foreach (IDrillableSocketProperty drillableSocket in Data.DrillableSockets.Values)
                {
                    isCompleted &= Data.DrillableObject.Value.HasHole(drillableSocket.EnterPoint, drillableSocket.EndPoint, drillableSocket.Width, drillableSocket.EnterTolerance, drillableSocket.EndTolerance, drillableSocket.WidthTolerance);
                }

                return isCompleted;
            }
        }

        private class EntityAutocompleter : Autocompleter<EntityData>
        {
            public EntityAutocompleter(EntityData data) : base(data)
            {
            }

            public override void Complete()
            {
                foreach (IDrillableSocketProperty drillableSocket in Data.DrillableSockets.Values)
                {
                    Data.DrillableObject.Value.CreateHole(Data.DrillableObject.Value.SceneObject.GameObject.transform.InverseTransformPoint(drillableSocket.EnterPoint),
                        Data.DrillableObject.Value.SceneObject.GameObject.transform.InverseTransformPoint(drillableSocket.EndPoint),
                        drillableSocket.Width);
                }
            }
        }

        [JsonConstructor, Preserve]
        public HasDrilledHolesCondition()
        {
            Data.DrillableObject = new SingleScenePropertyReference<IDrillableProperty>();
            Data.DrillableSockets = new MultipleScenePropertyReference<IDrillableSocketProperty>();
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