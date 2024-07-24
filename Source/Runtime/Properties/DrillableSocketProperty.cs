using UnityEngine;
using VRBuilder.Core.Properties;
using VRBuilder.Core.Utils;

namespace VRBuilder.VIRTOSHA.Properties
{
    [ExecuteInEditMode]
    public class DrillableSocketProperty : ProcessSceneObjectProperty, IDrillableSocketProperty
    {
        private DrillableSocketEndPoint endPoint;

        [SerializeField]
        private float width = 0.01f;

        public float Width => width;

        public Vector3 Start => transform.position;

        public Vector3 End
        {
            get
            {
                if (endPoint == null)
                {
                    AssignEndPoint();
                }

                return endPoint.transform.position;
            }
        }

        private void Awake()
        {
            AssignEndPoint();
        }

        void AssignEndPoint()
        {
            endPoint = GetComponentInChildren<DrillableSocketEndPoint>();

            if (endPoint == null)
            {
                endPoint = new GameObject("End Point").AddComponent<DrillableSocketEndPoint>();
                endPoint.transform.SetParent(transform);
                endPoint.transform.position = transform.position + new Vector3(0, -0.1f, 0);
                endPoint.transform.rotation = Quaternion.identity;
            }
        }

        private void OnDrawGizmos()
        {
            DebugUtils.DrawCylinderGizmo(Start, End, Width, Color.cyan);
        }
    }
}