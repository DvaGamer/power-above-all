using UnityEngine;

namespace PowerAboveAll
{
    public sealed partial class CabinetHud
    {
        public bool ShowWorldSupply;
        private int depotIndex;
        private void SupplyDesk(GameApp app)
        {
            if(!ShowWorldSupply)return;
            var w=app.State.World;var army=w.Army(w.PlayerArmyId);
            var depots=w.Depots.FindAll(d=>d.FactionId==army.FactionId);
            Fill(new Rect(1080,185,338,440),paper);Fill(new Rect(1080,185,338,3),brass);
            Text(new Rect(1095,201,275,30),T("supply.title"),heading);
            if(Press(new Rect(1380,197,25,27),"×")){ShowWorldSupply=false;return;}
            Text(new Rect(1095,237,309,45),T("supply.army_stock",WorldSupply.DaysLeft(army).ToString("0.0"),army.AmmunitionWagon),body);
            var convoy=w.Convoys.FindLast(c=>c.ArmyId==army.Id);
            if(depots.Count>0)
            {
                depotIndex%=depots.Count;var depot=depots[depotIndex];var site=w.Sites.Find(s=>s.Id==depot.SiteId);
                string location=T("region."+site.RegionId);
                if(Press(new Rect(1095,289,309,29),T("supply.depot_at",location)))
                    app.StrategyCamera.Focus(WorldMapEntities.Position(site.Position),10);
                Text(new Rect(1095,326,309,40),T("supply.depot_stock",depot.Food,depot.Ammunition),small);
                if(Press(new Rect(1095,372,309,32),T("supply.send"),depot.Food>=WorldSupply.FoodLoad&&depot.Ammunition>=WorldSupply.AmmunitionLoad&&!(convoy!=null&&WorldSupply.Active(convoy))))
                    app.SupplyAction(false,depot.Id);
                if(Press(new Rect(1095,411,309,32),T("supply.restock")))app.SupplyAction(true,depot.Id);
                if(depots.Count>1&&Press(new Rect(1095,451,309,26),T("supply.other_depot")))depotIndex=(depotIndex+1)%depots.Count;
            }
            else Text(new Rect(1095,289,309,60),T("supply.no_depot"),body);
            if(convoy!=null)
            {
                Text(new Rect(1095,486,309,55),T("supply.status."+convoy.Status.ToString().ToLowerInvariant())+"\n"+T("supply.cargo",convoy.Food,convoy.Ammunition),small);
                if(convoy.Status==ConvoyStatus.Travelling)
                    Text(new Rect(1095,544,309,22),T("supply.eta",(WorldRouting.Remaining(convoy.Route,convoy.Position)/WorldConvoy.MetresPerSecond/3600).ToString("0.0")),small);
                if(Press(new Rect(1095,580,309,30),T("supply.locate")))
                    app.StrategyCamera.Focus(WorldMapEntities.Position(convoy.Position),4);
            }
        }
    }
}
