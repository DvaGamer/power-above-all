#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;

namespace PowerAboveAll.Tests
{
    public sealed class StrategicCameraTests
    {
        GameObject host;Camera view;StrategicCamera atlas;
        [SetUp] public void Begin(){host=new GameObject("Atlas input test");view=host.AddComponent<Camera>();atlas=host.AddComponent<StrategicCamera>();atlas.Initialize(view);}
        [TearDown] public void End(){Object.DestroyImmediate(host);}
        [Test] public void ResumeRestoresAtlasPoseAfterTacticalCameraUse()
        {
            atlas.SetView(new Vector3(800,0,-120),750,70,40,true);
            var position=view.transform.position;var rotation=view.transform.rotation;
            atlas.Suspend();view.transform.SetPositionAndRotation(Vector3.zero,Quaternion.identity);view.orthographic=true;
            atlas.Resume();Assert.AreEqual(position,view.transform.position);Assert.AreEqual(rotation,view.transform.rotation);
            Assert.IsFalse(view.orthographic);Assert.AreEqual(750,atlas.Distance);
        }
        [Test] public void WorldBoundsKeepAtlasNearCentreWhileCloseViewCanExploreEdges()
        {
            var edge=new Vector3(5000,-50,5000);
            Assert.AreEqual(new Vector3(1220,0,870),StrategicCamera.ClampFocus(edge,60));
            Assert.AreEqual(new Vector3(120,0,100),StrategicCamera.ClampFocus(edge,StrategicCamera.MaximumDistance));
        }
        [Test] public void SuspendedBattleCameraDoesNotConsumeAtlasEvents()
        {
            atlas.Suspend();var input=new Event{type=EventType.ScrollWheel,delta=new Vector2(0,-15)};
            atlas.HandleMapEvent(input,Vector3.zero);Assert.AreEqual(EventType.ScrollWheel,input.type);
        }
    }
}
#endif
