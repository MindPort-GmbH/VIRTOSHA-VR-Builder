using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRBuilder.Core.Properties;
using VRBuilder.Core.Utils;
using VRBuilder.VIRTOSHA.Structs;

namespace VRBuilder.VIRTOSHA.Properties
{
    /// <summary>
    /// A game object defining a potential hole to be drilled.
    /// </summary>
    [ExecuteInEditMode]
    public class DrillableSocketProperty : ProcessSceneObjectProperty, IDrillableSocketProperty
    {
        private DrillableSocketEndPoint endPoint;

        [SerializeField]
        [Tooltip("Width of the hole.")]
        private float width = 0.01f;

        [SerializeField]
        [Tooltip("Maximum acceptable distance from the enter point.")]
        private float enterTolerance = 0.02f;

        [SerializeField]
        [Tooltip("Maximum acceptable distance from the end point.")]
        private float endTolerance = 0.04f;

        [SerializeField]
        [Tooltip("Maximum acceptable difference in width.")]
        private float widthTolerance = 0.01f;

        [SerializeField]
        [Tooltip("If true, the enter point will automatically move to the surface of the most suitable drillable object.")]
        private bool placeEnterPointOnDrillableObjectSurface = true;

        /// <summary>
        /// Width of the hole.
        /// </summary>
        public float Width => width;

        /// <summary>
        /// Beginning point of the hole. Can either correspond to transform or 
        /// be placed on the surface of an intersecting drillable object.
        /// </summary>
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

        /// <summary>
        /// End point of the hole. Corresponds to the position of the end point game object
        /// parented to this drillable socket.
        /// </summary>
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

        /// <summary>
        /// Maximum acceptable distance from the enter point.
        /// </summary>
        public float EnterTolerance => enterTolerance;

        /// <summary>
        /// Maximum acceptable distance from the end point.
        /// </summary>
        public float EndTolerance => endTolerance;

        /// <summary>
        /// Maximum acceptable difference in width.
        /// </summary>
        public float WidthTolerance => widthTolerance;

        /// <summary>
        /// Configure the drillable socket with the provided parameters.
        /// </summary>
        public void Configure(Hole hole, float enterTolerance = 0.01F, float endTolerance = 0.01F, float widthTolerance = 0.001F, bool placeEnterPointOnSurface = true)
        {
            if (endPoint == null)
            {
                AssignEndPoint();
            }

            transform.position = hole.EnterPoint;
            endPoint.transform.position = hole.EndPoint;
            width = hole.Width;
            this.enterTolerance = enterTolerance;
            this.endTolerance = endTolerance;
            this.widthTolerance = widthTolerance;
            placeEnterPointOnDrillableObjectSurface = placeEnterPointOnSurface;
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
            DebugUtils.DrawWireCylinderGizmo(transform.position, EndPoint, Width, Color.cyan);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(EnterPoint, EnterTolerance);
            Gizmos.DrawWireSphere(EndPoint, EndTolerance);
        }
    }
}