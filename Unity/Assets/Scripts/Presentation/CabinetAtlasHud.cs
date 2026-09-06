using System.Collections.Generic;
using UnityEngine;

namespace PowerAboveAll
{
    public sealed partial class CabinetHud
    {
        public bool PanelsHidden { get; set; }
        private bool showProvince, showDocument;
        private float panelOpenedAt=-10;
        private readonly List<Rect> labelBounds=new List<Rect>();

        private void Top(GameApp app,EconomyForecast forecast)
        {
            Fill(new Rect(0,0,1440,78),deep);Fill(new Rect(0,77,1440,1),brass);
            var mark=new GUIStyle(heading);mark.normal.textColor=paper;mark.fontSize=18;
            Text(new Rect(18,13,205,25),"POWER ABOVE ALL",mark);
            Text(new Rect(18,44,220,22),Date(app.ViewState.Week),lightBody);
            string[] labels={"ui.gold","ui.food","ui.supplies","ui.troops","ui.power"};
            string[] values={Animated(0,app.ViewState.Gold),Animated(1,app.ViewState.Food),Animated(2,app.ViewState.MilitarySupplies),Animated(3,app.ViewState.Troops),Animated(4,app.ViewState.Power)};
            string[] details={Signed(forecast.NetGold),Signed(forecast.NetFood),T("ui.stock"),T("ui.reserve",Number(app.ViewState.Manpower)),T("ui.personal")};
            for(int i=0;i<5;i++)
            {
                float x=257+i*174;Fill(new Rect(x-14,17,1,45),C("#536E5D"));
                Text(new Rect(x,10,161,18),T(labels[i]),lightTiny);
                var amount=new GUIStyle(numeral);amount.fontSize=24;amount.normal.textColor=i==0?brass:paper;
                Text(new Rect(x,29,153,28),values[i],amount);
                Text(new Rect(x,58,153,16),details[i],lightTiny);
            }
            if(Press(new Rect(1141,17,106,31),T("ui.language")))app.SetLanguage(L.Language=="ru"?"tr":"ru");
            if(Press(new Rect(1257,17,69,31),T("ui.help.short")))showHelp=true;
            if(Press(new Rect(1336,17,86,31),T("ui.world.hide")))PanelsHidden=true;
            if(Press(new Rect(1142,53,280,22),T(CampaignCore.Desk(app.State)==null?"dispatch.open":"dispatch.at_desk")))app.OpenBordeauxDesk();
        }

        private void AtlasNavigation(GameApp app)
        {
            Fill(new Rect(258,90,922,38),paper);Border(new Rect(258,90,922,38),rule);
            if(Press(new Rect(266,95,86,27),T("ui.world.world")))app.StrategyCamera.SetView(Vector3.zero,3000,0,85);
            if(Press(new Rect(359,95,86,27),T("ui.world.europe")))app.StrategyCamera.SetView(AtlasProjection.Project(12,49),570,0,72);
            if(Press(new Rect(452,95,86,27),T("ui.world.france")))app.StrategyCamera.SetView(AtlasProjection.Project(2.3f,46.6f),150,0,65);
            if(Press(new Rect(545,95,104,27),T("ui.troops")))app.StrategyCamera.Focus(app.Map.RegionWorld(app.ViewState.ArmyRegionId));
            string[] names={"council","economy","journal","mandate"};
            for(int i=0;i<4;i++)
                if(Press(new Rect(672+i*124,95,117,27),T(names[i]=="mandate"?"ui.mandate.tab":"ui.tab."+names[i])))
                {if(showDocument&&document==names[i])showDocument=false;else OpenDocument(names[i]);}
            if(app.StrategyCamera.Distance<420)
            {
                float width=104;
                for(int i=0;i<ModeNames.Length;i++)
                {
                    Rect r=new Rect(300+i*(width+4),802,width,28);
                    if(Press(r,T("ui.mode."+ModeNames[i]),true,app.Mode==ModeNames[i]))app.SetMode(ModeNames[i]);
                }
                Fill(new Rect(972,802,165,28),paper);
                Text(new Rect(982,807,145,18),T("ui.world."+app.StrategyCamera.ZoomLevel.ToString().ToLowerInvariant()),tiny);
            }
        }

        private void Atlas(GameApp app)
        {
            labelBounds.Clear();float distance=app.StrategyCamera.Distance;
            if(distance>900)
            {
                AtlasLabel(app,AtlasProjection.Project(-103,46),T("ui.world.north_america"),mapLabel,220);
                AtlasLabel(app,AtlasProjection.Project(-61,-15),T("ui.world.south_america"),mapLabel,220);
                AtlasLabel(app,AtlasProjection.Project(20,6),T("ui.world.africa"),mapLabel,160);
                AtlasLabel(app,AtlasProjection.Project(83,46),T("ui.world.asia"),mapLabel,160);
                AtlasLabel(app,AtlasProjection.Project(133,-26),T("ui.world.australia"),mapLabel,170);
            }
            else if(distance>280)
                AtlasLabel(app,AtlasProjection.Project(2.3f,46.6f),T("ui.world.france"),heading,180);
            if(distance<280)
            {
                var selected=app.ViewState.SelectedRegionId;
                foreach(var s in app.Map.WorldData.settlements)
                    if(s.regionId==selected&&s.rank==0)SettlementLabel(app,s,distance,true);
                foreach(var s in app.Map.WorldData.settlements)
                    if(!(s.regionId==selected&&s.rank==0)&&(s.rank==0||distance<85))SettlementLabel(app,s,distance,false);
            }
            var sea=new GUIStyle(cityLabel);sea.fontSize=distance>280?16:13;sea.normal.textColor=C("#416F76");
            if(distance<900)
            {
                AtlasLabel(app,AtlasProjection.Project(-12,43),T("ui.atlas.atlantic"),sea,190);
                AtlasLabel(app,AtlasProjection.Project(7,39),T("ui.atlas.mediterranean"),sea,230);
                if(distance<280)AtlasLabel(app,AtlasProjection.Project(-2,50),T("ui.atlas.channel"),sea,190);
            }
        }
        private void SettlementLabel(GameApp app,AtlasSettlement place,float distance,bool selected)
        {
            Vector3 p=AtlasProjection.Project(place.longitude,place.latitude,.2f);
            string name=place.rank==0?T(distance<80?"city."+place.regionId:"region."+place.regionId):place.name;
            var style=new GUIStyle(distance<80?cityLabel:mapLabel);style.fontSize=selected?16:14;style.fontStyle=selected?FontStyle.Bold:FontStyle.Normal;
            AtlasLabel(app,p,name,style,165,selected);
        }
        private void AtlasLabel(GameApp app,Vector3 world,string text,GUIStyle style,float width,bool selected=false)
        {
            Vector3 projected=app.Camera.WorldToScreenPoint(world);if(projected.z<=0)return;
            Vector2 p=ViewLayout.ToCanvas(projected);Rect r=new Rect(p.x-width*.5f,p.y-30,width,24);
            if(r.x<10||r.xMax>1430||r.y<140||r.yMax>788)return;
            if(!PanelsHidden&&((showProvince&&r.x<250)||(showDocument&&r.xMax>1140)))return;
            foreach(Rect previous in labelBounds)if(previous.Overlaps(r))return;
            labelBounds.Add(r);
            var centered=new GUIStyle(style);centered.alignment=TextAnchor.MiddleCenter;centered.wordWrap=false;
            if(selected){Fill(new Rect(r.x+8,r.y,width-16,24),paper);Fill(new Rect(r.x+8,r.y+23,width-16,1),brass);}
            var halo=new GUIStyle(centered);halo.normal.textColor=new Color(paper.r,paper.g,paper.b,.80f);
            Text(new Rect(r.x+1,r.y+1,r.width,r.height),text,halo);Text(r,text,centered);
        }
        private void Bottom(GameApp app)
        {
            Fill(new Rect(0,844,1440,56),deep);Fill(new Rect(0,844,1440,1),brass);
            if(Press(new Rect(15,857,64,28),T("ui.save")))app.Save();
            if(Press(new Rect(86,857,72,28),T("ui.load")))app.Load();
            if(Press(new Rect(165,857,58,28),T("ui.new")))confirmNew=true;
            Text(new Rect(244,852,870,22),string.IsNullOrEmpty(app.Message)?T("ui.welcome"):app.Message,lightBody);
            Text(new Rect(244,879,890,17),T(CampaignCore.Desk(app.State)!=null?"dispatch.forecast_notice":"ui.world.controls"),lightTiny);
            if(Press(new Rect(1160,855,262,34),T("ui.next"),true,true))app.NextWeek();
        }
    }
}
