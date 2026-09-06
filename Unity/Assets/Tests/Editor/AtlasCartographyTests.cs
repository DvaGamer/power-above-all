using NUnit.Framework;
using UnityEngine;

namespace PowerAboveAll.Tests
{
    public sealed class AtlasCartographyTests
    {
        [Test] public void DisplayGeneralizationPreservesSourceAndEndpoints()
        {
            var path=new GeoPath{points=new[]{0f,0f,1f,.002f,2f,-.002f,3f,0f}};
            var original=(float[])path.points.Clone();
            var ink=AtlasCartography.Simplify(path,.04f);
            CollectionAssert.AreEqual(original,path.points);
            CollectionAssert.AreEqual(new[]{0f,0f,3f,0f},ink.points);
        }
        [Test] public void AGeographicBendBeyondToleranceIsNotFlattened()
        {
            var path=new GeoPath{points=new[]{0f,0f,1f,0f,1.5f,1f,2f,0f,3f,0f}};
            var ink=AtlasCartography.Simplify(path,.1f);
            bool bend=false;for(int i=0;i<ink.points.Length;i+=2)if(ink.points[i]==1.5f&&ink.points[i+1]==1f)bend=true;
            Assert.That(bend,Is.True);
        }
        [Test] public void ClosedBoundaryKeepsAClosedUsableRing()
        {
            var path=new GeoPath{points=new[]{0f,0f,.5f,0f,1f,0f,1f,1f,0f,1f,0f,0f}};
            var ink=AtlasCartography.Simplify(path,.1f);
            Assert.That(ink.points.Length,Is.GreaterThanOrEqualTo(8));
            Assert.That(ink.points[0],Is.EqualTo(ink.points[ink.points.Length-2]));
            Assert.That(ink.points[1],Is.EqualTo(ink.points[ink.points.Length-1]));
        }
        [Test] public void SchematicDashesStayOnSourceRouteWithGaps()
        {
            var dashes=AtlasCartography.Dashes(new[]{0f,0f,2f,0f},.4f,.3f);
            Assert.That(dashes.Length,Is.GreaterThan(5));
            float last=-1;
            foreach(var dash in dashes)
            {
                Assert.That(dash.points[1],Is.Zero);Assert.That(dash.points[3],Is.Zero);
                Assert.That(dash.points[0],Is.GreaterThan(last));
                Assert.That(dash.points[2],Is.LessThanOrEqualTo(2f));last=dash.points[2];
            }
        }
    }
}
