using System.Collections.Generic;
using NUnit.Framework;

namespace PowerAboveAll.Tests
{
    public sealed class WorldCombatBalanceTests
    {
        [TestCase(1789u)] [TestCase(731u)] [TestCase(9821u)]
        public void PreparationCanReverseTwoToOneDisadvantageThroughVisibleCauses(uint seed)
        {
            foreach(bool prepared in new[]{false,true})
            {
                var home=new WorldSite{Id="paris",RegionId="ile",Position=new WorldPoint(0,0)};
                var enemy=new WorldSite{Id="reims",RegionId="champagne",Position=new WorldPoint(780,0)};
                var s=WorldSimulation.Create(CampaignCore.Create(),new[]{home,enemy},new[]{new WorldRoad{Id="road",From=home.Id,To=enemy.Id,Points=new List<WorldPoint>{home.Position,enemy.Position}}});
                var a=s.State.Army("royal");var b=s.State.Army("resistance");a.Posture=WorldPosture.Defend;b.Posture=WorldPosture.Advance;
                int assigned=0,original=b.Men;
                for(int i=0;i<b.Units.Count;i++){int men=i==5?2400-assigned:(int)(b.Units[i].Original*2400d/original);b.Units[i].Men=b.Units[i].Original=men;assigned+=men;}
                if(prepared)
                {
                    b.Fatigue=65;b.AmmunitionWagon=0;b.WagonIntegrity=0;foreach(var unit in b.Units)unit.Ammo=4;
                    s.State.Terrain.Add(new WorldTerrainFeature{Id="ridge",Kind=WorldTerrainKind.Hill,Centre=new WorldPoint(-200,0),Radius=700,Height=30,Source="Synthetic balance fixture",Confidence="Not real French terrain"});
                    s.State.Commanders.Find(c=>c.Id==a.CommanderId).Competence=85;
                }
                s.SetSpeed(WorldSpeed.Normal);s.Advance(.1);s.State.Battles[0].RandomState=seed;
                for(int i=0;i<240&&s.State.HasCombat;i++)
                {
                    s.Advance(10);while(s.State.Clock.PendingMilliseconds>=100)s.Drain();
                    var reserve=a.Units.Find(u=>u.Role==WorldRole.Reserve);
                    var gap=a.Units.Find(u=>(u.Role==WorldRole.Left||u.Role==WorldRole.Centre||u.Role==WorldRole.Right)&&u.Withdrawal!=WorldWithdrawal.None);
                    if(prepared&&gap!=null&&!reserve.ManualOrder&&reserve.Orders.Count==0)s.OrderUnit(reserve.Id,WorldCommand.Slot(a,gap),WorldIntent.Hold,WorldFormation.Line);
                }
                Assert.That(s.State.HasCombat,Is.False);Assert.That(s.State.Battles[0].WinnerId,Is.EqualTo(prepared?a.Id:b.Id));
                Assert.That(a.Men,Is.GreaterThan(600));Assert.That(b.Men,Is.GreaterThan(1200),"Organisation should break before annihilation.");
                WorldValidation.Validate(s.Campaign);
            }
        }
    }
}
