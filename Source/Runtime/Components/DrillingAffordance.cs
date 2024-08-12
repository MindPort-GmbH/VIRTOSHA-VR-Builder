using UnityEngine;

namespace VRBuilder.VIRTOSHA.Components
{
    /// <summary>
    /// Provides some feedback while a <see cref="Drill"/> is drilling.
    /// </summary>
    public abstract class DrillingAffordance : MonoBehaviour
    {
        /// <summary>
        /// Sets the width of the visualized hole.
        /// </summary>        
        public abstract void SetWidth(float width);

        /// <summary>
        /// Sets the depth of the visualized hole.
        /// </summary>        
        public abstract void SetDepth(float depth);

        /// <summary>
        /// Removes this visualization.
        /// </summary>
        public abstract void Remove();
    }
}