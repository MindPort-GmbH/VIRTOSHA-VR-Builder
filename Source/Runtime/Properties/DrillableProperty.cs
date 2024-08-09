using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using VRBuilder.Core.Properties;
using VRBuilder.Core.Utils;
using VRBuilder.VIRTOSHA.Structs;

namespace VRBuilder.VIRTOSHA.Properties
{
    /// <summary>
    /// Property that stores holes that can be dynamically drilled on its game object.
    /// </summary>
    public class DrillableProperty : LockableProperty, IDrillableProperty
    {
        private List<Hole> holes = new List<Hole>();

        [SerializeField]
        [Tooltip("List of colliders that are managed by this property. They should be children of this game object.")]
        private List<Collider> colliders = new List<Collider>();

        [SerializeField]
        [Tooltip("If selected, holes in the object will be displayed as gizmos in the editor window.")]
        private bool debugDisplayHoles;

        [Header("Events")]
        [SerializeField]
        private UnityEvent<DrillablePropertyEventArgs> holeCreated = new UnityEvent<DrillablePropertyEventArgs>();

        /// <inheritdoc/>       
        public UnityEvent<DrillablePropertyEventArgs> HoleCreated => holeCreated;

        /// <inheritdoc/>       
        public IEnumerable<Collider> Colliders
        {
            get
            {
                if (colliders.Count == 0)
                {
                    colliders.AddRange(GetComponentsInChildren<Collider>());
                }

                if (colliders.Count == 0)
                {
                    Debug.LogError($"{typeof(DrillableProperty).Name} on '{gameObject.name}' is missing a collider. Add some colliders to the game object hierarchy or to the list.");
                }

                return colliders;
            }
        }

        /// <inheritdoc/>       
        public void CreateHole(Vector3 enterPosition, Vector3 endPosition, float width)
        {
            holes.Add(new Hole(
                transform.InverseTransformPoint(enterPosition),
                transform.InverseTransformPoint(endPosition),
            width));

            holeCreated?.Invoke(new DrillablePropertyEventArgs(new Hole(enterPosition, endPosition, width)));
        }

        /// <inheritdoc/>       
        public void CreateHole(Hole hole)
        {
            CreateHole(hole.EnterPoint, hole.EndPoint, hole.Width);
        }

        /// <inheritdoc/>       
        public bool HasHole(Vector3 enterPosition, Vector3 endPosition, float width, float startTolerance, float endTolerance, float widthTolerance)
        {
            return holes.Any(hole =>
            Vector3.Distance(transform.TransformPoint(hole.EnterPoint), enterPosition) <= startTolerance &&
            Vector3.Distance(transform.TransformPoint(hole.EndPoint), endPosition) <= endTolerance &&
            hole.Width - width <= widthTolerance
            );
        }

        /// <inheritdoc/>       
        public bool HasHole(Hole hole, float enterTolerance, float endTolerance, float widthTolerance)
        {
            return HasHole(hole.EnterPoint, hole.EndPoint, hole.Width, enterTolerance, endTolerance, widthTolerance);
        }

        protected override void InternalSetLocked(bool lockState)
        {
        }

        private void OnDrawGizmos()
        {
            if (debugDisplayHoles == false)
            {
                return;
            }

            foreach (Hole hole in holes)
            {
                DebugUtils.DrawWireCylinderGizmo(transform.TransformPoint(hole.EnterPoint), transform.TransformPoint(hole.EndPoint), hole.Width, Color.green);
            }
        }
    }
}