using System.Linq;
using UnityEngine;
using VRBuilder.VIRTOSHA.Properties;

namespace VRBuilder.VIRTOSHA.Components
{
    /// <summary>
    /// Component representing the tip of the drill, used to register collisions when starting to drill holes.
    /// </summary>
    public class DrillTip : MonoBehaviour
    {
        private DrillBit parentDrillBit;

        public void Init(DrillBit parent)
        {
            parentDrillBit = parent;

            Collider collider = GetComponent<Collider>();

            if (collider == null)
            {
                Debug.LogError($"No collider is found on {typeof(DrillTip).Name} or its children on '{gameObject.name}'. A collider is needed for the component to work as intended. Please add one.");
            }
            else if (collider.isTrigger == false)
            {
                Debug.LogError($"The collider on {typeof(DrillTip).Name} or its children on '{gameObject.name}' is not set as a trigger. Please set 'Is Trigger' to true in the Inspector for the component to work as intended.");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (parentDrillBit.InUse == false)
            {
                return;
            }

            IDrillableProperty drillableProperty = other.GetComponentInParent<IDrillableProperty>();

            if (drillableProperty != null && drillableProperty.Colliders.Contains(other) && drillableProperty.IsLocked == false)
            {
                parentDrillBit.EmitTouchedDrillableObject(drillableProperty, other);
            }
        }
    }
}