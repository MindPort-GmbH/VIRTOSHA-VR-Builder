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
        private bool debugDisplayHoles;

        public void CreateHole(Vector3 startPosition, Vector3 endPosition, float width)
        {
            holes.Add(new Hole(startPosition, endPosition, width));
        }

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

        private void Awake()
        {
            holes.Add(new Hole(new Vector3(0, .5f, 0), new Vector3(0, 0, 0), 0.1f));
        }

        private void OnDrawGizmos()
        {
            if (debugDisplayHoles == false)
            {
                return;
            }

            foreach (Hole hole in holes)
            {
                DebugUtils.DrawCylinderGizmo(transform.TransformPoint(hole.Start), transform.TransformPoint(hole.End), hole.Width, Color.red);
            }
        }
    }
}