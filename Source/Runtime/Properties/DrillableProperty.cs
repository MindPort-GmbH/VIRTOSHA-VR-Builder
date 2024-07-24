using System.Collections.Generic;
using UnityEngine;
using VRBuilder.Core.Properties;

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
        }

        private void OnDrawGizmos()
        {
            if (debugDisplayHoles == false)
            {
                return;
            }

            foreach (Hole hole in holes)
            {
                DrawCylinderGizmo(transform.TransformPoint(hole.Start), transform.TransformPoint(hole.End), hole.Width);
            }
        }

        private void DrawCylinderGizmo(Vector3 startPoint, Vector3 endPoint, float width)
        {
            Gizmos.color = Color.red;

            // Calculate cylinder direction and length
            Vector3 direction = endPoint - startPoint;
            float length = direction.magnitude;
            Vector3 directionNormalized = direction.normalized;

            // Calculate rotation
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, directionNormalized);
            Vector3 midPoint = (startPoint + endPoint) / 2;

            // Scale the cylinder to match the specified length and radius
            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(midPoint, rotation, new Vector3(width, length / 2, width));

            // Draw the cylinder
            Gizmos.DrawWireMesh(GetCylinderMesh());

            // Reset the Gizmos matrix
            Gizmos.matrix = oldMatrix;
        }

        private Mesh GetCylinderMesh()
        {
            // Generate or get a cylinder mesh
            GameObject tempCylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            Mesh cylinderMesh = tempCylinder.GetComponent<MeshFilter>().sharedMesh;
            DestroyImmediate(tempCylinder);
            return cylinderMesh;
        }
    }
}