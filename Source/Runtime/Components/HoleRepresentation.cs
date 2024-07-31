using UnityEngine;

namespace VRBuilder.VIRTOSHA.Components
{
    public class HoleRepresentation : MonoBehaviour
    {
        public void VisualizeHole(Vector3 enterPoint, Vector3 direction, Transform parent, float width, float depth)
        {
            GameObject hole = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            GameObject.Destroy(hole.GetComponent<Collider>());
            hole.transform.position = enterPoint;
            hole.transform.rotation = Quaternion.LookRotation(direction);
            hole.transform.localScale = new Vector3(width / 2, width / 2, width / 5);
            hole.transform.parent = parent;
        }
    }
}