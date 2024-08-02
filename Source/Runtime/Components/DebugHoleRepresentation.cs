using UnityEngine;
using VRBuilder.VIRTOSHA.Properties;
using VRBuilder.VIRTOSHA.Structs;

namespace VRBuilder.VIRTOSHA.Components
{
    /// <summary>
    /// Visualizes the holes on a drillable property by spawning a sphere on the entrance point
    /// and showing the depth in mm.
    /// </summary>
    [RequireComponent(typeof(IDrillableProperty))]
    public class DebugHoleRepresentation : MonoBehaviour
    {
        private IDrillableProperty drillableProperty;

        [SerializeField]
        private DepthDisplay depthDisplayPrefab;

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
            holeObject.transform.localScale = new Vector3(hole.Width, hole.Width, hole.Width);
            holeObject.transform.parent = transform;

            holeObject.GetComponent<MeshRenderer>().material.color = Color.black;

            DepthDisplay depthDisplay = GameObject.Instantiate<DepthDisplay>(depthDisplayPrefab);
            depthDisplay.transform.parent = holeObject.transform;
            depthDisplay.transform.localPosition = Vector3.zero;
            depthDisplay.transform.localRotation = Quaternion.identity;
            depthDisplay.SetDepth(Vector3.Distance(hole.EnterPoint, hole.EndPoint));
        }
    }
}