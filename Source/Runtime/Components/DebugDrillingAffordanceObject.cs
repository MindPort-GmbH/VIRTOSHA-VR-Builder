using UnityEngine;

namespace VRBuilder.VIRTOSHA.Components
{
    public class DebugDrillingAffordanceObject : MonoBehaviour
    {
        private DepthDisplay depthDisplay;

        public void SetWidth(float width)
        {
            ParticleSystem particleSystem = GetComponentInChildren<ParticleSystem>();

            if (particleSystem == null)
            {
                return;
            }

            particleSystem.transform.localScale = new Vector3(width, width, width);
            //ParticleSystem.ShapeModule shapeModule = particleSystem.shape;
            //shapeModule.radius = width;
        }

        public void SetDepth(float depth)
        {
            if (depthDisplay == null)
            {
                depthDisplay = GetComponentInChildren<DepthDisplay>();
            }

            if (depthDisplay != null)
            {
                depthDisplay.SetDepth(depth);
            }
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}