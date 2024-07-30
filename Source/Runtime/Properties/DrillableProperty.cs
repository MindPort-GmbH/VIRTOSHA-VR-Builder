using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRBuilder.Core.Properties;
using VRBuilder.Core.Utils;
using VRBuilder.VIRTOSHA.Structs;

namespace VRBuilder.VIRTOSHA.Properties
{
    public class DrillableProperty : LockableProperty, IDrillableProperty
    {
        private List<Hole> holes = new List<Hole>();

        [SerializeField]
        [Tooltip("If selected, holes in the object will be displayed as gizmos in the editor window.")]
        private bool debugDisplayHoles;

        /// <inheritdoc/>       
        public void CreateHole(Vector3 enterPosition, Vector3 endPosition, float width)
        {
            // TODO should probably not create holes if locked.
            holes.Add(new Hole(
                transform.InverseTransformPoint(enterPosition),
                transform.InverseTransformPoint(endPosition),
                width));
        }

        public void CreateHole(Hole hole)
        {
            CreateHole(hole.EnterPoint, hole.EndPoint, hole.Width);
        }

        /// <inheritdoc/>       
        public bool HasHole(Vector3 startPosition, Vector3 endPosition, float width, float startTolerance, float endTolerance, float widthTolerance)
        {
            return holes.Any(hole =>
            Vector3.Distance(transform.TransformPoint(hole.EnterPoint), startPosition) <= startTolerance &&
            Vector3.Distance(transform.TransformPoint(hole.EndPoint), endPosition) <= endTolerance &&
            hole.Width - width <= widthTolerance
            );
        }

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
                DebugUtils.DrawCylinderGizmo(transform.TransformPoint(hole.EnterPoint), transform.TransformPoint(hole.EndPoint), hole.Width, Color.green);
            }
        }
    }
}