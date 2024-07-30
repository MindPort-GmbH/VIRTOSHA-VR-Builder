using UnityEngine;
using VRBuilder.Core.Properties;
using VRBuilder.Core.SceneObjects;
using VRBuilder.VIRTOSHA.Structs;

namespace VRBuilder.VIRTOSHA.Properties
{
    /// <summary>
    /// Property for an object that can be drilled holes into.
    /// </summary>
    public interface IDrillableProperty : ISceneObjectProperty, ILockable
    {
        /// <summary>
        /// Creates a hole with the specified characteristic in the object.
        /// </summary>
        void CreateHole(Vector3 enterPosition, Vector3 endPosition, float width);

        /// <summary>
        /// Creates a hole with the specified characteristic in the object.
        /// </summary>
        void CreateHole(Hole hole);

        /// <summary>
        /// Returns true if the object has at least a hole that satisfies the given requisites.
        /// </summary>
        bool HasHole(Vector3 enterPosition, Vector3 endPosition, float width, float startTolerance, float endTolerance, float widthTolerance);

        /// <summary>
        /// Creates a hole with the specified characteristic in the object.
        /// </summary>
        bool HasHole(Hole hole, float enterTolerance, float endTolerance, float widthTolerance);
    }
}