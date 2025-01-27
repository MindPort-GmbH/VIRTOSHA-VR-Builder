using TMPro;
using UnityEngine;

namespace VRBuilder.VIRTOSHA.Components
{
    /// <summary>
    /// Displays a value in mm on a text mesh.
    /// </summary>
    public class DepthDisplay : MonoBehaviour
    {
        TextMeshPro textMesh;

        /// <summary>
        /// Set the displayed depth to the specified value.
        /// </summary>        
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