using UnityEngine;
using VRBuilder.VIRTOSHA.Properties;

namespace VRBuilder.VIRTOSHA
{
    public class DrillTip : MonoBehaviour
    {
        private DrillBit parentDrillBit;

        public void Init(DrillBit parent)
        {
            parentDrillBit = parent;
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