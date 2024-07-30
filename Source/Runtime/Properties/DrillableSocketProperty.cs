using System.Collections.Generic;
using System.Linq;
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

        [SerializeField]
        private float enterTolerance = 0.02f;

        [SerializeField]
        private float endTolerance = 0.04f;

        [SerializeField]
        private float widthTolerance = 0.01f;

        [SerializeField]
        private bool placeEnterPointOnDrillableObjectSurface = true;

        public float Width => width;

        public Vector3 EnterPoint
        {
            get
            {
                if (placeEnterPointOnDrillableObjectSurface)
                {
                    return CalculateEnterPointOnObjectSurface();
                }
                else
                {
                    return transform.position;
                }
            }
        }

        private Vector3 CalculateEnterPointOnObjectSurface()
        {
            Ray ray = new Ray(transform.position, (EndPoint - transform.position).normalized);
            RaycastHit[] hits = Physics.RaycastAll(ray, Vector3.Distance(transform.position, EndPoint));

            IEnumerable<Vector3> contactPoints = hits.Where(hit => hit.collider.GetComponent<IDrillableProperty>() != null).Select(hit => hit.point);

            if (contactPoints.Any())
            {
                return contactPoints.OrderBy(point => Vector3.Distance(transform.position, point)).First();
            }

            return transform.position;
        }

        public Vector3 EndPoint
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

        public float EnterTolerance => enterTolerance;

        public float EndTolerance => endTolerance;

        public float WidthTolerance => widthTolerance;

        private void Awake()
        {
            AssignEndPoint();
        }

        private void AssignEndPoint()
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
            DebugUtils.DrawCylinderGizmo(transform.position, EndPoint, Width, Color.cyan);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(EnterPoint, EnterTolerance);
            Gizmos.DrawWireSphere(EndPoint, EndTolerance);
        }
    }
}