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
    public class DrillableSocketProperty : LockableProperty, IDrillableSocketProperty
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

        [SerializeField]
        [Tooltip("If true, a highlight object will be shown when this property is unlocked.")]
        private bool showHighlightObject = true;

        [SerializeField]
        [Tooltip("Material to use for the highlight object.")]
        private Material highlightMaterial;

        private GameObject highlightObject;

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
        /// <param name="hole">The Hole object to configure the property with.</param>
        /// <param name="enterTolerance">The tolerance value for entering the hole.</param>
        /// <param name="endTolerance">The tolerance value for reaching the end of the hole.</param>
        /// <param name="widthTolerance">The tolerance value for the width of the hole.</param>
        /// <param name="placeEnterPointOnSurface">Determines whether to place the enter point on the surface of the drillable object.</param>
        public void Configure(Hole hole, float enterTolerance = 0.01f, float endTolerance = 0.01f, float widthTolerance = 0.001f, bool placeEnterPointOnSurface = true)
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

            IEnumerable<Vector3> contactPoints = hits.Where(IsDrillablePropertyHit).Select(hit => hit.point);

            if (contactPoints.Any())
            {
                return contactPoints.OrderBy(point => Vector3.Distance(transform.position, point)).First();
            }

            return transform.position;
        }

        private bool IsDrillablePropertyHit(RaycastHit hit)
        {
            IDrillableProperty drillableProperty = hit.collider.GetComponentInParent<IDrillableProperty>();
            return drillableProperty != null && drillableProperty.Colliders.Contains(hit.collider);
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
                endPoint.transform.localPosition = new Vector3(0, -0.1f, 0);
                endPoint.transform.localRotation = Quaternion.identity;
            }
        }

        private void OnDrawGizmos()
        {
            DebugUtils.DrawWireCylinderGizmo(transform.position, EndPoint, Width, Color.cyan);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(EnterPoint, EnterTolerance);
            Gizmos.DrawWireSphere(EndPoint, EndTolerance);
        }

        private void CreateHighlightObject()
        {
            highlightObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            GameObject.Destroy(highlightObject.GetComponent<Collider>());

            Vector3 midpoint = (transform.position + EndPoint) / 2.0f;
            highlightObject.transform.position = midpoint;

            float distance = Vector3.Distance(transform.position, EndPoint);
            highlightObject.transform.localScale = new Vector3(Width, distance / 2.0f, Width);

            Vector3 direction = EndPoint - transform.position;
            highlightObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);

            highlightObject.transform.parent = transform;

            if (highlightMaterial != null)
            {
                MeshRenderer meshRenderer = highlightObject.gameObject.GetComponent<MeshRenderer>();
                meshRenderer.material = highlightMaterial;
            }
        }

        protected override void InternalSetLocked(bool lockState)
        {
            if (lockState == false)
            {
                if (highlightObject == null)
                {
                    CreateHighlightObject();
                }

                highlightObject.SetActive(true);
            }
            else
            {
                highlightObject?.SetActive(false);
            }
        }
    }
}