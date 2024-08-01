using TMPro;
using UnityEngine;

namespace VRBuilder.VIRTOSHA.Components
{
    public class DepthDisplay : MonoBehaviour
    {
        TextMeshPro textMesh;
        public void SetDepth(float depth)
        {
            if (textMesh == null)
            {
                textMesh = GetComponentInChildren<TextMeshPro>();
            }

            if (textMesh == null)
            {
                Debug.LogError($"No TextMeshPro component found in children of {gameObject.name}. The depth will not be displayed.");
                return;
            }

            textMesh.text = $"{Mathf.Round(depth * 1000)} mm";
        }
    }
}