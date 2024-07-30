using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRBuilder.Core.Properties;
using VRBuilder.Core.Utils;

namespace VRBuilder.VIRTOSHA.Properties
{
    public class DrillableProperty : LockableProperty, IDrillableProperty
    {
        private List<Hole> holes = new List<Hole>();

        [SerializeField]
        [Tooltip("If selected, holes in the object will be displayed as gizmos in the editor window.")]
        private bool debugDisplayHoles;

        /// <inheritdoc/>       
        public void CreateHole(Vector3 startPosition, Vector3 endPosition, float width)
        {
            // TODO should probably not create holes if locked.
            holes.Add(new Hole(startPosition, endPosition, width));
        }

        /// <inheritdoc/>       
        public bool HasHole(Vector3 startPosition, Vector3 endPosition, float width, float startTolerance, float endTolerance, float widthTolerance)
        {
            return holes.Any(hole =>
            Vector3.Distance(transform.TransformPoint(hole.Start), startPosition) <= startTolerance &&
            Vector3.Distance(transform.TransformPoint(hole.End), endPosition) <= endTolerance &&
            hole.Width - width <= widthTolerance
            );
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
                DebugUtils.DrawCylinderGizmo(transform.TransformPoint(hole.Start), transform.TransformPoint(hole.End), hole.Width, Color.green);
            }
        }
    }
}