using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TestTools;
using VRBuilder.BasicInteraction.Properties;
using VRBuilder.Core;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Core.Tests.RuntimeUtils;
using VRBuilder.VIRTOSHA.Components;

namespace VRBuilder.VIRTOSHA.Tests
{
    public class DrillTests : RuntimeTests
    {
        private class UsablePropertyMock : MonoBehaviour, IUsableProperty
        {
            public UnityEvent<UsablePropertyEventArgs> UseStarted => new UnityEvent<UsablePropertyEventArgs>();

            public UnityEvent<UsablePropertyEventArgs> UseEnded => new UnityEvent<UsablePropertyEventArgs>();

            public bool IsBeingUsed => throw new NotImplementedException();

            public ISceneObject SceneObject => throw new NotImplementedException();

            public bool IsLocked => throw new NotImplementedException();

            public event EventHandler<LockStateChangedEventArgs> Locked;
            public event EventHandler<LockStateChangedEventArgs> Unlocked;

            public void FastForwardUse()
            {
                throw new NotImplementedException();
            }

            public void ForceSetUsed(bool isUsed)
            {
                throw new NotImplementedException();
            }

            public bool RemoveUnlocker(IStepData data)
            {
                throw new NotImplementedException();
            }

            public void RequestLocked(bool lockState, IStepData stepData = null)
            {
                throw new NotImplementedException();
            }

            public void SetLocked(bool lockState)
            {
                throw new NotImplementedException();
            }
        }

        [UnityTest]
        public IEnumerator DrillShowsErrorIfNoDrillBit()
        {
            // Given a game object with a drill,
            GameObject drillObject = new GameObject("Drill");
            drillObject.AddComponent<UsablePropertyMock>();
            Drill drill = drillObject.AddComponent<Drill>();

            // When no drill bit component is present amongst its children,

            // Then an error is logged.
            LogAssert.Expect(LogType.Error, $"The {typeof(Drill).Name} component on '{drillObject.name}' cannot work without a {typeof(DrillBit).Name} component on a child object. Please create one.");

            GameObject.DestroyImmediate(drillObject);
            yield return null;
        }
    }
}