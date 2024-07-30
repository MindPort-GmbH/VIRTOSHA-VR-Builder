using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using VRBuilder.Core.Conditions;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Core.Tests.Conditions;
using VRBuilder.VIRTOSHA.Conditions;
using VRBuilder.VIRTOSHA.Properties;
using VRBuilder.VIRTOSHA.Structs;


namespace VRBuilder.VIRTOSHA.Tests.Conditions
{
    public class DrillHolesConditionTests : ConditionTests
    {
        private static TestCaseData[] completeConditionTestCases = new TestCaseData[]
        {
            new TestCaseData(new Hole(new Vector3(10.5f, 10, 10), new Vector3(10, 10.2f, 10), 0.2f), new Hole(new Vector3(10.5f, 10, 10), new Vector3(10, 10.2f, 10), 0.2f)).Returns(null),
            new TestCaseData(new Hole(new Vector3(10, 12, 10), new Vector3(10, 10.2f, 10), 0.2f), new Hole(new Vector3(10, 10.5f, 10), new Vector3(10, 10.2f, 10), 0.2f)).Returns(null),
            new TestCaseData(new Hole(new Vector3(10.5f, 10, 10), new Vector3(10, 10.2f, 10), 0.2f), new Hole(new Vector3(10.54f, 10, 10), new Vector3(10, 10.16f, 10), 0.2f)).Returns(null),
        };

        private static TestCaseData[] failedConditionTestCases = new TestCaseData[]
        {
            new TestCaseData(new Hole(new Vector3(10.5f, 10, 10), new Vector3(10, 10, 10), 0.2f), new Hole(new Vector3(10.5f, 10, 10), new Vector3(9.8f, 10, 10), 0.2f)).Returns(null),
            new TestCaseData(new Hole(new Vector3(10.5f, 10, 10), new Vector3(10, 10.2f, 10), 0.2f), new Hole(new Vector3(10.5f, 10, 10), new Vector3(10, 10.2f, 10), 0.3f)).Returns(null),
            new TestCaseData(new Hole(new Vector3(10.5f, 10, 10), new Vector3(10, 10.2f, 10), 0.2f), new Hole(new Vector3(10.6f, 10, 10), new Vector3(10, 10.1f, 10), 0.2f)).Returns(null),
        };

        private IDrillableProperty CreateDrillableObject()
        {
            GameObject drillableGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            drillableGameObject.transform.position = new Vector3(10, 10, 10);
            IDrillableProperty drillableProperty = drillableGameObject.AddComponent<DrillableProperty>();
            return drillableProperty;
        }

        private IDrillableSocketProperty CreateDrillableSocket()
        {
            GameObject drillableSocketObject = new GameObject("Drillable Socket");
            IDrillableSocketProperty drillableSocketProperty = drillableSocketObject.AddComponent<DrillableSocketProperty>();
            return drillableSocketProperty;
        }

        protected override ICondition CreateDefaultCondition()
        {
            IDrillableProperty drillableProperty = CreateDrillableObject();
            IDrillableSocketProperty drillableSocket = CreateDrillableSocket();
            drillableSocket.Configure(new Hole(), 0.05f, 0.05f);
            DrillHolesCondition condition = new DrillHolesCondition();
            condition.Data.DrillableSockets = new MultipleScenePropertyReference<IDrillableSocketProperty>(drillableSocket.SceneObject.Guid);
            condition.Data.DrillableObject = new SingleScenePropertyReference<IDrillableProperty>(drillableProperty.SceneObject.Guid);
            return condition;
        }

        [UnityTest]
        [TestCaseSource("completeConditionTestCases")]
        public IEnumerator ConditionIsFulfilled(Hole expected, Hole actual)
        {
            // Given a drill holes condition,
            IDrillableProperty drillableProperty = CreateDrillableObject();
            IDrillableSocketProperty drillableSocket = CreateDrillableSocket();
            drillableSocket.Configure(expected, 0.05f, 0.05f);
            DrillHolesCondition condition = new DrillHolesCondition();
            condition.Data.DrillableSockets = new MultipleScenePropertyReference<IDrillableSocketProperty>(drillableSocket.SceneObject.Guid);
            condition.Data.DrillableObject = new SingleScenePropertyReference<IDrillableProperty>(drillableProperty.SceneObject.Guid);

            // When the specified holes are drilled,
            condition.LifeCycle.Activate();
            while (condition.LifeCycle.Stage != Core.Stage.Active)
            {
                yield return null;
                condition.Update();
            }

            drillableProperty.CreateHole(actual);

            yield return null;
            condition.Update();

            // Then it is fulfilled as intended.
            Assert.IsTrue(condition.IsCompleted);

            // Cleanup
            GameObject.DestroyImmediate(drillableProperty.SceneObject.GameObject);
            GameObject.DestroyImmediate(drillableSocket.SceneObject.GameObject);
        }

        [UnityTest]
        [TestCaseSource("failedConditionTestCases")]
        public IEnumerator ConditionIsNotFulfilledWhenWrongHolesDrilled(Hole expected, Hole actual)
        {
            // Given a drill holes condition,
            IDrillableProperty drillableProperty = CreateDrillableObject();
            IDrillableSocketProperty drillableSocket = CreateDrillableSocket();
            drillableSocket.Configure(expected, 0.05f, 0.05f);
            DrillHolesCondition condition = new DrillHolesCondition();
            condition.Data.DrillableSockets = new MultipleScenePropertyReference<IDrillableSocketProperty>(drillableSocket.SceneObject.Guid);
            condition.Data.DrillableObject = new SingleScenePropertyReference<IDrillableProperty>(drillableProperty.SceneObject.Guid);

            // When the specified holes are drilled,
            condition.LifeCycle.Activate();
            while (condition.LifeCycle.Stage != Core.Stage.Active)
            {
                yield return null;
                condition.Update();
            }

            drillableProperty.CreateHole(actual);

            yield return null;
            condition.Update();

            // Then it is fulfilled as intended.
            Assert.IsFalse(condition.IsCompleted);

            // Cleanup
            GameObject.DestroyImmediate(drillableProperty.SceneObject.GameObject);
            GameObject.DestroyImmediate(drillableSocket.SceneObject.GameObject);
        }

        [UnityTest]
        [TestCaseSource("completeConditionTestCases")]
        public IEnumerator HolesCreatedOnAutocomplete(Hole expected, Hole actual)
        {
            // Given a drill holes condition,
            IDrillableProperty drillableProperty = CreateDrillableObject();
            IDrillableSocketProperty drillableSocket = CreateDrillableSocket();
            drillableSocket.Configure(expected, placeEnterPointOnSurface: false);
            DrillHolesCondition condition = new DrillHolesCondition();
            condition.Data.DrillableSockets = new MultipleScenePropertyReference<IDrillableSocketProperty>(drillableSocket.SceneObject.Guid);
            condition.Data.DrillableObject = new SingleScenePropertyReference<IDrillableProperty>(drillableProperty.SceneObject.Guid);

            // When it is fastforwarded,
            condition.LifeCycle.Activate();
            while (condition.LifeCycle.Stage != Core.Stage.Active)
            {
                yield return null;
                condition.Update();
            }

            condition.Autocomplete();

            // Then the specified holes are created.
            Assert.IsTrue(drillableProperty.HasHole(expected, 0.001f, 0.001f, 0.001f));

            // Cleanup
            GameObject.DestroyImmediate(drillableProperty.SceneObject.GameObject);
            GameObject.DestroyImmediate(drillableSocket.SceneObject.GameObject);
        }
    }
}