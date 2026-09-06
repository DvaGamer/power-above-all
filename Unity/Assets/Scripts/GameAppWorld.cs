using System;
using System.Collections.Generic;
using UnityEngine;

namespace PowerAboveAll
{
    public sealed partial class GameApp
    {
        public WorldSimulation Simulation { get; private set; }
        public bool ShowPendingDocument;
        public bool WorldHeadquartersSelected;
        public void SupplyAction(bool stock,string depotId)=>Report(stock?WorldSupply.Restock(State,depotId):WorldSupply.Dispatch(State,depotId,State.World.PlayerArmyId));
        public void OpenWorldSupply()=>hud.ShowWorldSupply=true;
        public void ReportWorldOrder(string id,WorldPoint point,WorldIntent intent,WorldFormation formation)
        {var result=Simulation.OrderUnit(id,point,intent,formation);Report(result);if(result.Ok)Feedback("order");}
        public WorldMapEntities WorldEntities { get; private set; }
        private float nextWorldRefresh;
        private string lastWorldNotice;
        private void InitializeContinuousWorld()
        {
            if(State.World==null)
            {
                var sites=new List<WorldSite>();var roads=new List<WorldRoad>();
                // Oynanış merkezleri önce gelir; ek yerleşimler aynı grafiğin düğümleridir.
                foreach(var region in Map.WorldData.regions)
                {
                    var town=Array.Find(Map.WorldData.settlements,s=>s.id==region.seatId);
                    sites.Add(new WorldSite{Id=town.id,RegionId=region.id,Position=WorldPoint.FromGeographic(town.longitude,town.latitude)});
                }
                foreach(var town in Map.WorldData.settlements)
                    if(!sites.Exists(s=>s.Id==town.id))sites.Add(new WorldSite{Id=town.id,RegionId=town.regionId,Position=WorldPoint.FromGeographic(town.longitude,town.latitude)});
                foreach(var data in Map.WorldData.roads)
                {
                    if(!sites.Exists(s=>s.Id==data.from)||!sites.Exists(s=>s.Id==data.to))continue;
                    var road=new WorldRoad{Id=data.id,From=data.from,To=data.to};
                    for(int i=0;i<data.points.Length;i+=2)road.Points.Add(WorldPoint.FromGeographic(data.points[i],data.points[i+1]));
                    road.Points[0]=sites.Find(s=>s.Id==road.From).Position;road.Points[road.Points.Count-1]=sites.Find(s=>s.Id==road.To).Position;roads.Add(road);
                }
                Simulation=WorldSimulation.Create(State,sites,roads);
                CreateWorldTerrain();
            }
            else Simulation=new WorldSimulation(State);
            if(WorldEntities==null){WorldEntities=new GameObject("World armies and formations").AddComponent<WorldMapEntities>();WorldEntities.Initialize(this);}
            nextWorldRefresh=0;ShowPendingDocument=false;lastWorldNotice="";
        }
        public void SetWorldSpeed(int speed)
        {if(ChoosingRole||speed<0||speed>3)return;Simulation.SetSpeed((WorldSpeed)speed);}
        public void FocusWorldArmy()
        {
            var army=State.World.Army(State.World.SelectedArmyId);
            var sight=State.World.Sightings.Find(s=>s.ArmyId==army.Id);
            if(army.Id!=State.World.PlayerArmyId&&sight==null)return;
            StrategyCamera.Focus(WorldMapEntities.Position(army.Id==State.World.PlayerArmyId?army.Position:sight.Position),army.Activity==ArmyActivity.Fighting?.15f:12);
        }
        private void UpdateContinuousWorld()
        {
            if(Simulation==null||ChoosingRole)return;
            Simulation.Advance(Math.Min(Time.unscaledDeltaTime,.25f));
            if(State.World.LastNoticeKey!=lastWorldNotice)
            {
                lastWorldNotice=State.World.LastNoticeKey;messageKey=lastWorldNotice;messageArgs=new object[0];
                if(lastWorldNotice=="world.contact")Feedback("order");
                if(lastWorldNotice=="world.victory"||lastWorldNotice=="world.defeat")Feedback(lastWorldNotice=="world.victory"?"victory":"defeat");
            }
            if(Time.unscaledTime>=nextWorldRefresh){nextWorldRefresh=Time.unscaledTime+.35f;Refresh();}
            if(!State.PendingPetition&&!CampaignCore.MandateDue(State))ShowPendingDocument=false;
            if(Input.GetKeyDown(KeyCode.Tab))hud.PanelsHidden=!hud.PanelsHidden;
            if(!ShowPendingDocument&&Input.GetMouseButtonDown(0)&&Input.GetKey(KeyCode.LeftShift)&&!hud.IsPointerOverInterface(ViewLayout.ToCanvas(Input.mousePosition)))
            {
                var unit=State.World.Army(State.World.PlayerArmyId).Units.Find(u=>u.Id==State.World.SelectedUnitId);
                if((unit!=null||WorldHeadquartersSelected)&&new Plane(Vector3.up,new Vector3(0,WorldMapEntities.Ground,0)).Raycast(Camera.ScreenPointToRay(Input.mousePosition),out float hit))
                {
                    var point=WorldMapEntities.Point(Camera.ScreenPointToRay(Input.mousePosition).GetPoint(hit));
                    Report(WorldHeadquartersSelected?Simulation.MoveHeadquarters(point):Simulation.OrderUnit(unit.Id,point,WorldIntent.Hold,unit.Formation));
                }
            }
        }
        private bool SelectWorldEntity(Vector3 point)
        {
            if(Input.GetKey(KeyCode.LeftShift))return true;
            string unit=WorldEntities.PickUnit(point);
            if(unit!=null){State.World.SelectedArmyId=State.World.PlayerArmyId;State.World.SelectedUnitId=unit;Feedback("paper");return true;}
            string army=WorldEntities.PickArmy(point);
            if(army==null)return false;
            State.World.SelectedArmyId=army;State.World.SelectedUnitId="";
            if(lastMapClickRegion==army&&Time.unscaledTime-lastMapClick<.32f)FocusWorldArmy();
            lastMapClickRegion=army;lastMapClick=Time.unscaledTime;return true;
        }
        private void HandleWorldTimeEvent(Event e)
        {
            if(ChoosingRole||State?.World==null||e.type!=EventType.KeyDown)return;
            if(e.keyCode==KeyCode.Space){SetWorldSpeed(State.World.Clock.Speed==WorldSpeed.Pause?1:0);e.Use();}
            else if(e.keyCode>=KeyCode.Alpha1&&e.keyCode<=KeyCode.Alpha3){SetWorldSpeed((int)e.keyCode-(int)KeyCode.Alpha0);e.Use();}
            else if(e.keyCode==KeyCode.Escape&&ShowPendingDocument){ShowPendingDocument=false;e.Use();}
        }
    }
}
