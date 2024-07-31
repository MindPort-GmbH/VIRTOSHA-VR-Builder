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

            if (GetComponent<Collider>() == null)
            {
                Debug.LogError($"No collider is set on {typeof(DrillTip).Name} on '{gameObject.name}'. A collider is needed for the component to work as intended. Please add one.");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (parentDrillBit.InUse == false)
            {
                return;
            }

            IDrillableProperty drillableProperty = other.GetComponent<IDrillableProperty>();

            if (drillableProperty != null)
            {
                parentDrillBit.EmitTouchedDrillableObject(drillableProperty, other);
            }
        }
    }
}