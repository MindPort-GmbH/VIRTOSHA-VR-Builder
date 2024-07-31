using UnityEngine;
using VRBuilder.VIRTOSHA.Properties;
using VRBuilder.VIRTOSHA.Structs;

namespace VRBuilder.VIRTOSHA.Components
{
    [RequireComponent(typeof(IDrillableProperty))]
    public class DrilledHoleRepresentation : MonoBehaviour
    {
        private IDrillableProperty drillableProperty;

        private void OnEnable()
        {
            if (drillableProperty == null)
            {
                drillableProperty = GetComponent<IDrillableProperty>();
            }

            drillableProperty.HoleCreated.AddListener(OnHoleCreated);
        }

        private void OnDisable()
        {
            drillableProperty.HoleCreated.RemoveListener(OnHoleCreated);
        }

        private void OnHoleCreated(DrillablePropertyEventArgs arg0)
        {
            VisualizeHole(arg0.Hole);
        }

        private void VisualizeHole(Hole hole)
        {
            GameObject holeObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            GameObject.Destroy(holeObject.GetComponent<Collider>());
            holeObject.transform.position = hole.EnterPoint;
            holeObject.transform.rotation = Quaternion.LookRotation(hole.EndPoint - hole.EnterPoint);
            holeObject.transform.localScale = new Vector3(hole.Width, hole.Width, hole.Width / 5);
            holeObject.transform.parent = transform;

            holeObject.GetComponent<MeshRenderer>().material.color = Color.black;
        }
    }
}