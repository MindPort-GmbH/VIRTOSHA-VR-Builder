using UnityEngine;

namespace VRBuilder.VIRTOSHA.Components
{
    /// <summary>
    /// Debug drilling affordance implementation, displaying a particle system and the depth in mm.
    /// </summary>
    public class DebugDrillingAffordance : DrillingAffordance
    {
        [SerializeField]
        private DepthDisplay depthDisplayPrefab;

        private DepthDisplay depthDisplay;

        private void Awake()
        {
            depthDisplay = GameObject.Instantiate<DepthDisplay>(depthDisplayPrefab);
            depthDisplay.transform.parent = transform;
            depthDisplay.transform.localPosition = Vector3.zero;
            depthDisplay.transform.localRotation = Quaternion.identity;
        }

        public override void SetWidth(float width)
        {
            ParticleSystem particleSystem = GetComponentInChildren<ParticleSystem>();

            if (particleSystem == null)
            {
                return;
            }

            particleSystem.transform.localScale = new Vector3(width, width, width);
        }

        public override void SetDepth(float depth)
        {
            depthDisplay.SetDepth(depth);
        }

        public override void Remove()
        {
            Destroy(gameObject);
        }
    }
}