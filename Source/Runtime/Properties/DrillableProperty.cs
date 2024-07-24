using System.Collections.Generic;
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

        protected override void InternalSetLocked(bool lockState)
        {
            throw new System.NotImplementedException();
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