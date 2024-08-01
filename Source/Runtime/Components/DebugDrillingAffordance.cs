using UnityEngine;

namespace VRBuilder.VIRTOSHA.Components
{
    [RequireComponent(typeof(IDrill))]
    public class DebugDrillingAffordance : MonoBehaviour
    {
        private IDrill drill;
        private DebugDrillingAffordanceObject affordanceObject;

        [SerializeField]
        private DebugDrillingAffordanceObject affordancePrefab;

        [SerializeField]
        private DepthDisplay depthDisplayPrefab;

        private void OnEnable()
        {
            if (drill == null)
            {
                drill = GetComponent<IDrill>();
            }

            drill.DrillingStarted += OnStartDrilling;
            drill.DrillingStopped += OnStopDrilling;
        }

        private void OnDisable()
        {
            drill.DrillingStarted -= OnStartDrilling;
            drill.DrillingStopped -= OnStopDrilling;
        }

        private void OnStartDrilling(object sender, DrillEventArgs e)
        {
            affordanceObject = GameObject.Instantiate<DebugDrillingAffordanceObject>(affordancePrefab);
            affordanceObject.transform.position = e.EnterPoint;
            affordanceObject.transform.rotation = Quaternion.LookRotation(e.Direction);
            affordanceObject.SetWidth(e.HoleWidth);

            DepthDisplay depthDisplay = GameObject.Instantiate(depthDisplayPrefab);
            depthDisplay.transform.parent = affordanceObject.transform;
            depthDisplay.transform.localPosition = Vector3.zero;
            depthDisplay.transform.localRotation = Quaternion.identity;
            depthDisplay.SetDepth(drill.CurrentDrillDepth);
        }

        private void OnStopDrilling(object sender, DrillEventArgs e)
        {
            affordanceObject?.Destroy();
        }

        private void Update()
        {
            if (drill.IsDrilling)
            {
                affordanceObject.SetDepth(drill.CurrentDrillDepth);
            }
        }
    }
}