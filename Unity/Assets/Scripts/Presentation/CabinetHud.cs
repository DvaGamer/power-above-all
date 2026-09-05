using System;
using System.Globalization;
using UnityEngine;

namespace PowerAboveAll
{
    /// <summary>Ministerial marginalia around a permanently visible atlas. Drawn by GameApp after its 1440×900 GUI transform.</summary>
    public sealed class CabinetHud : MonoBehaviour
    {
        private readonly Color forest = C("#20362E"), deep = C("#192A24"), paper = C("#F0EADB"), pale = C("#E5E1CC"), ink = C("#314333"), muted = C("#7D856E"), brass = C("#CAB781"), red = C("#A36451"), rule = C("#CBCBB2");
        private GUIStyle body, small, tiny, title, heading, numeral, button, quietButton, lightBody, lightTiny, mapLabel, cityLabel, tabStyle;
        private Texture2D disk;
        private Font serifFont, sansFont;
        private bool ready, showHelp, confirmNew;
        private Vector2 provinceScroll, documentScroll;
        private string document = "council";
        private string cachedLanguage;
        private readonly float[] displayedStocks = new float[6], targetStocks = new float[6];
        private readonly bool[] stockInitialized = new bool[6];
        private LogEntry latestEntry;
        private float latestEntryTime;
        private float provinceContentHeight = 1100;
        private string shownProvince;
        public bool BlocksMapInput { get { return showHelp || confirmNew; } }
        private static readonly string[] ModeNames = { "control", "unrest", "food", "tax", "army", "influence" };

        public void Draw(GameApp app)
        {
            if (app == null || app.State == null || app.BattleActive) return;
            EnsureStyles();
            if (cachedLanguage != L.Language) { cachedLanguage = L.Language; provinceScroll = documentScroll = Vector2.zero; }
            CampaignState state = app.State; EconomyForecast forecast = CampaignCore.Forecast(state);
            if (state.Journal.Count > 0 && !ReferenceEquals(latestEntry, state.Journal[0])) { latestEntry = state.Journal[0]; latestEntryTime = Time.unscaledTime; }
            bool previousEnabled = GUI.enabled;
            if (BlocksMapInput) GUI.enabled = false;
            Top(app, forecast);
            Atlas(app);
            Province(app);
            Cabinet(app, forecast);
            Bottom(app);
            GUI.enabled = previousEnabled;
            if (showHelp) Help();
            if (confirmNew) Confirm(app);
        }

        private void EnsureStyles()
        {
            if (ready) return; ready = true;
            serifFont = Font.CreateDynamicFontFromOSFont(new[] { "Georgia", "Times New Roman", "Liberation Serif" }, 20);
            sansFont = Font.CreateDynamicFontFromOSFont(new[] { "Arial", "Segoe UI", "DejaVu Sans" }, 16);
            body = Style(14, ink); small = Style(13, muted); tiny = Style(11, muted); title = Style(29, ink, true); heading = Style(21, ink, true); numeral = Style(28, paper, true);
            lightBody = Style(14, paper); lightTiny = Style(11, new Color(.69f,.73f,.64f));
            mapLabel = Style(15, C("#3D5038"), true); mapLabel.alignment = TextAnchor.MiddleCenter;
            cityLabel = Style(12, C("#667253"), true); cityLabel.fontStyle = FontStyle.Italic; cityLabel.alignment = TextAnchor.MiddleCenter;
            button = Style(14, paper); button.alignment = TextAnchor.MiddleCenter;
            quietButton = Style(13, ink); quietButton.alignment = TextAnchor.MiddleCenter;
            tabStyle = Style(12, muted); tabStyle.alignment = TextAnchor.MiddleCenter;
            disk = new Texture2D(64,64,TextureFormat.RGBA32,false) { name = "Engraved seal", filterMode = FilterMode.Bilinear };
            Color[] pixels = new Color[4096];
            for(int y=0;y<64;y++)for(int x=0;x<64;x++){float d=Vector2.Distance(new Vector2(x+.5f,y+.5f),new Vector2(32,32));pixels[y*64+x]=new Color(1,1,1,Mathf.Clamp01(32-d));}
            disk.SetPixels(pixels); disk.Apply();
        }
        private GUIStyle Style(int size, Color color, bool serif = false)
        {
            var style = new GUIStyle(GUI.skin.label) { font=serif?serifFont:sansFont,fontSize=size,wordWrap=true,richText=false,padding=new RectOffset(0,0,0,0),margin=new RectOffset(0,0,0,0) };
            style.normal.textColor=color;return style;
        }
        private static Color C(string html){ColorUtility.TryParseHtmlString(html,out var color);return color;}
        private static string T(string key,params object[] args){return L.Text(key,args);}
        private static string Number(float value){return Mathf.RoundToInt(value).ToString("N0",CultureInfo.GetCultureInfo(L.Language=="tr"?"tr-TR":"ru-RU"));}
        private static string Signed(int value){return (value>=0?"+":"−")+Number(Math.Abs(value));}
        private string Animated(int index,float actual)
        {
            targetStocks[index]=actual;
            if(!stockInitialized[index]){stockInitialized[index]=true;displayedStocks[index]=actual;}
            return Number(displayedStocks[index]);
        }
        private void Update()
        {
            float blend=1f-Mathf.Exp(-Time.unscaledDeltaTime*9f);
            for(int i=0;i<displayedStocks.Length;i++)
            {
                displayedStocks[i]=Mathf.Lerp(displayedStocks[i],targetStocks[i],blend);
                if(Mathf.Abs(displayedStocks[i]-targetStocks[i])<.08f)displayedStocks[i]=targetStocks[i];
            }
        }
        private static void Fill(Rect rect,Color color){Color before=GUI.color;GUI.color=color;GUI.DrawTexture(rect,Texture2D.whiteTexture);GUI.color=before;}
        private static void Border(Rect rect,Color color,float width=1){Fill(new Rect(rect.x,rect.y,rect.width,width),color);Fill(new Rect(rect.x,rect.yMax-width,rect.width,width),color);Fill(new Rect(rect.x,rect.y,width,rect.height),color);Fill(new Rect(rect.xMax-width,rect.y,width,rect.height),color);}
        private void Text(Rect rect,string text,GUIStyle style){GUI.Label(rect,text,style);}
        private void Rule(float x,float y,float width){Fill(new Rect(x,y,width,1),rule);}
        private bool Press(Rect rect,string text,bool enabled=true,bool primary=false)
        {
            bool over=rect.Contains(Event.current.mousePosition);
            Color fill=primary?forest:pale;
            if(over&&enabled)fill=primary?C("#405B40"):C("#D9DFC5");
            if(!enabled)fill=Color.Lerp(fill,paper,.5f);
            Fill(rect,fill);Border(rect,primary?forest:C("#BCC4A8"));
            bool previous=GUI.enabled;GUI.enabled=previous&&enabled;
            var style=primary?button:quietButton;
            Color old=GUI.color;if(!enabled)GUI.color=new Color(old.r,old.g,old.b,.47f);
            bool hit=GUI.Button(rect,text,style);GUI.color=old;GUI.enabled=previous;return hit;
        }
        private void Seal(Rect rect,Color color,int variant=0)
        {
            Color old=GUI.color;GUI.color=color;GUI.DrawTexture(rect,disk);GUI.color=old;
            // Profile miniature: collar, shoulders, curled wig, nose and a dark cravat.
            float unit=rect.width/64f;
            Rect R(float x,float y,float w,float h){return new Rect(rect.x+x*unit,rect.y+y*unit,w*unit,h*unit);}
            GUI.color=C("#E0D6B6");GUI.DrawTexture(R(17,12,28,31),disk);GUI.DrawTexture(R(20,27,25,20),disk);GUI.color=old;
            Fill(R(36,28,12,6),C("#E0D6B6"));Fill(R(18,43,29,12),C("#C6BC9C"));Fill(R(29,40,8,10),color);
            for(int i=0;i<4;i++){GUI.color=C("#B9B49D");GUI.DrawTexture(R(13+(variant%2)*2,17+i*5,11,10),disk);}GUI.color=old;
            Fill(R(36,26,2,2),color);Fill(R(43,36,4,1),color);Fill(R(20,52,28,1),color);
        }

        private void Top(GameApp app,EconomyForecast forecast)
        {
            Fill(new Rect(0,0,1440,94),deep);Fill(new Rect(0,93,1440,1),C("#5B6249"));
            Border(new Rect(24,20,43,52),C("#8A855A"));Text(new Rect(28,25,35,39),T("ui.seal"),numeral);
            var brand=new GUIStyle(title);brand.fontSize=24;brand.normal.textColor=paper;
            Text(new Rect(82,23,252,31),T("ui.title"),brand);Text(new Rect(84,60,236,16),T("ui.tagline"),lightTiny);
            Resource(337,T("ui.gold"),Animated(0,app.State.Gold),T("ui.weekly",Signed(forecast.NetGold)),brass);
            Resource(490,T("ui.food"),Animated(1,app.State.Food),T("ui.weekly",Signed(forecast.NetFood)),brass);
            Resource(643,T("ui.supplies"),Animated(2,app.State.MilitarySupplies),T("ui.stock"),brass);
            Resource(796,T("ui.troops"),Animated(3,app.State.Troops),T("ui.reserve",Number(app.State.Manpower)),brass);
            Resource(949,T("ui.power"),Animated(4,app.State.Power),T("ui.personal"),brass);
            Resource(1102,T("ui.unrest"),Animated(5,CampaignCore.AverageUnrest(app.State)),T("ui.country"),C("#C58B70"));
            var language=new Rect(1275,24,72,32);if(Press(language,T("ui.language")))app.SetLanguage(L.Language=="ru"?"tr":"ru");
            if(Press(new Rect(1359,24,54,32),T("ui.help.short")))showHelp=!showHelp;
        }
        private void Resource(float x,string label,string value,string detail,Color accent)
        {
            Fill(new Rect(x-16,24,1,48),C("#405344"));Text(new Rect(x,16,140,19),label,lightTiny);
            var style=new GUIStyle(numeral);style.normal.textColor=accent;Text(new Rect(x,34,142,35),value,style);Text(new Rect(x,71,142,17),detail,lightTiny);
        }
        private void Atlas(GameApp app)
        {
            Fill(new Rect(245,94,895,43),C("#DFE3CE"));Rule(245,136,895);
            float modeWidth=895f/ModeNames.Length;
            for(int i=0;i<ModeNames.Length;i++)
            {
                Rect rect=new Rect(245+i*modeWidth,94,modeWidth,43);bool active=app.Mode==ModeNames[i];
                if(active){Fill(rect,C("#EFF0DE"));Fill(new Rect(rect.x+12,rect.yMax-3,rect.width-24,3),C("#7D9067"));}
                var style=new GUIStyle(tabStyle);if(active)style.normal.textColor=ink;
                if(GUI.Button(rect,T("ui.mode."+ModeNames[i]),style))app.SetMode(ModeNames[i]);
            }
            var mapTitle=new GUIStyle(heading);mapTitle.alignment=TextAnchor.MiddleCenter;mapTitle.normal.textColor=C("#49604D");
            Text(new Rect(400,149,585,32),T("ui.atlas.title"),mapTitle);
            var caption=new GUIStyle(tiny);caption.alignment=TextAnchor.MiddleCenter;
            Text(new Rect(420,181,545,18),T("ui.atlas.subtitle"),caption);
            foreach(var definition in CampaignCore.Regions)
            {
                Vector3 screen=app.Camera.WorldToScreenPoint(app.Map.RegionWorld(definition.Id));
                float x=screen.x/Screen.width*1440f,y=(1f-screen.y/Screen.height)*900f;
                if(x<265||x>1118||y<206||y>749)continue;
                var style=new GUIStyle(mapLabel);if(definition.Id==app.State.SelectedRegionId){style.fontStyle=FontStyle.Bold;style.normal.textColor=C("#2C422C");}
                Text(new Rect(x-83,y-34,166,24),T("region."+definition.Id),style);
                Text(new Rect(x-65,y+14,130,19),T("city."+definition.Id),cityLabel);
            }
            var geography=new GUIStyle(cityLabel);geography.fontSize=18;geography.normal.textColor=C("#6F8A7B");
            Text(new Rect(271,448,132,60),T("ui.atlas.atlantic"),geography);
            Text(new Rect(699,714,300,28),T("ui.atlas.mediterranean"),geography);
            Text(new Rect(306,224,180,24),T("ui.atlas.channel"),geography);
            // Brass compass engraved in the sea, not an interactive control.
            var compass=new GUIStyle(heading);compass.alignment=TextAnchor.MiddleCenter;compass.normal.textColor=C("#6E826A");
            Text(new Rect(280,654,65,25),T("ui.atlas.north"),tiny);Text(new Rect(280,678,65,45),T("ui.compass"),compass);
            Fill(new Rect(259,769,868,24),new Color(.90f,.92f,.83f,.86f));
            Text(new Rect(272,773,665,18),T("ui.legend."+app.Mode),tiny);Text(new Rect(956,773,165,18),T("ui.atlas.scale"),tiny);
        }

        private void Province(GameApp app)
        {
            CampaignState state=app.State;RegionState region=CampaignCore.Region(state,state.SelectedRegionId);
            RegionDefinition definition=Array.Find(CampaignCore.Regions,r=>r.Id==state.SelectedRegionId);
            if(region==null||definition==null)return;
            if(shownProvince!=region.Id){shownProvince=region.Id;provinceScroll=Vector2.zero;}
            Fill(new Rect(0,94,245,706),paper);Fill(new Rect(244,94,1,706),C("#AEBBA0"));
            Text(new Rect(18,110,213,22),T("ui.province.dispatch"),tiny);Rule(18,141,208);
            Text(new Rect(18,155,212,60),T("region."+region.Id),title);
            Text(new Rect(19,214,209,24),T("city."+region.Id),small);
            provinceScroll=GUI.BeginScrollView(new Rect(12,246,226,540),provinceScroll,new Rect(0,0,205,Mathf.Max(540,provinceContentHeight)),false,false);
            float y=0;Text(new Rect(4,y,195,25),T("ui.orders"),tiny);y+=29;
            Order(app,ref y,"bread",T("ui.order.bread"),T("ui.order.bread.detail"),!region.BreadUsed&&state.Food>=40,region.BreadUsed?"ui.reason.used":"ui.reason.food");
            Order(app,ref y,"tax",T("ui.order.tax"),T("ui.order.tax.detail"),!region.TaxUsed,"ui.reason.used");
            Order(app,ref y,"recruit",T("ui.order.recruit"),T("ui.order.recruit.detail"),!region.RecruitUsed&&state.ArmyRegionId==region.Id&&state.Gold>=120&&state.Food>=20&&state.MilitarySupplies>=15&&state.Manpower>=200,region.RecruitUsed?"ui.reason.used":state.ArmyRegionId!=region.Id?"ui.reason.army":"ui.reason.recruit");
            var march=CampaignCore.CanMarch(state,region.Id);bool here=state.ArmyRegionId==region.Id;
            if(Press(new Rect(4,y,195,45),T(here?"ui.army.here":march.RequiresBattle?"ui.army.battle":"ui.army.march"),!here&&march.Ok,true))app.March();y+=53;
            string marchDetail=here?T("ui.army.here.detail"):L.Text(march.Key,march.Args);
            float marchHeight=small.CalcHeight(new GUIContent(marchDetail),195);Text(new Rect(4,y,195,marchHeight),marchDetail,small);y+=marchHeight+19;
            if(region.Id=="ile")Order(app,ref y,"subsidy",T(state.SubsidyParis?"ui.order.subsidy.stop":"ui.order.subsidy"),T(state.SubsidyParis?"ui.order.subsidy.stop.detail":"ui.order.subsidy.detail"),true);
            Rule(4,y,195);y+=20;
            Meter(4,y,196,T("ui.control"),region.Control,C("#698260"));y+=48;Meter(4,y,196,T("ui.unrest"),region.Unrest,red);y+=48;Meter(4,y,196,T("ui.elite"),region.EliteLoyalty,C("#AF9964"));y+=52;
            Pair(4,y,195,T("ui.population"),T("ui.million",(definition.Population/1000000f).ToString("0.0",CultureInfo.GetCultureInfo(L.Language=="tr"?"tr-TR":"ru-RU"))));y+=30;
            Pair(4,y,195,T("ui.tax.base"),Number(definition.BaseTax));y+=30;Pair(4,y,195,T("ui.food.base"),Number(definition.BaseFood));y+=39;
            Rule(4,y,195);y+=17;Text(new Rect(4,y,195,22),T("ui.army.dispatch"),tiny);y+=28;
            Text(new Rect(4,y,195,29),T("city."+state.ArmyRegionId),heading);y+=36;
            Pair(4,y,195,T("ui.troops"),Number(state.Troops));y+=29;Pair(4,y,195,T("ui.moves"),Number(state.Moves));y+=33;
            Meter(4,y,195,T("ui.morale"),state.Morale,C("#708563"));y+=49;Meter(4,y,195,T("ui.supply"),state.Supply,C("#9B9E66"));y+=49;Meter(4,y,195,T("ui.fatigue"),state.Fatigue,red);y+=55;
            provinceContentHeight=y+10;
            GUI.EndScrollView();
        }
        private void Order(GameApp app,ref float y,string action,string name,string detail,bool enabled,string reasonKey=null)
        {
            if(Press(new Rect(4,y,195,38),name,enabled))app.Act(action);y+=44;
            if(!enabled&&!string.IsNullOrEmpty(reasonKey))
            {
                string reason=T(reasonKey);var reasonStyle=new GUIStyle(small);reasonStyle.normal.textColor=red;
                float reasonHeight=reasonStyle.CalcHeight(new GUIContent(reason),188);
                Text(new Rect(7,y,188,reasonHeight),reason,reasonStyle);y+=reasonHeight+6;
            }
            float detailHeight=small.CalcHeight(new GUIContent(detail),188);
            Text(new Rect(7,y,188,detailHeight),detail,small);y+=detailHeight+16;
        }
        private void Pair(float x,float y,float width,string key,string value)
        {
            Text(new Rect(x,y,width*.65f,28),key,small);var right=new GUIStyle(body);right.alignment=TextAnchor.UpperRight;Text(new Rect(x+width*.61f,y,width*.39f,28),value,right);
        }
        private void Meter(float x,float y,float width,string key,float value,Color color)
        {
            Pair(x,y,width,key,Number(value));Fill(new Rect(x,y+27,width,4),C("#D5D6BF"));Fill(new Rect(x,y+27,width*Mathf.Clamp01(value/100f),4),color);
        }

        private void Cabinet(GameApp app,EconomyForecast forecast)
        {
            Fill(new Rect(1140,94,300,706),paper);Fill(new Rect(1140,94,1,706),C("#AAB79C"));
            Text(new Rect(1161,109,258,20),T("ui.cabinet"),tiny);Rule(1161,141,257);
            string[] names={"council","economy","journal"};
            for(int i=0;i<names.Length;i++)
            {
                Rect rect=new Rect(1156+i*89,151,87,36);bool selected=document==names[i];if(selected){Fill(rect,pale);Fill(new Rect(rect.x,rect.yMax-2,rect.width,2),C("#839371"));}
                if(GUI.Button(rect,T("ui.tab."+names[i]),tabStyle)){document=names[i];documentScroll=Vector2.zero;}
            }
            float height=document=="council"?1700:document=="economy"?1230:Mathf.Max(575,app.State.Journal.Count*145+100);
            documentScroll=GUI.BeginScrollView(new Rect(1156,201,278,584),documentScroll,new Rect(0,0,251,height),false,false);
            if(document=="council")Council(app);else if(document=="economy")Economy(app,forecast);else Journal(app);
            GUI.EndScrollView();
        }
        private void Council(GameApp app)
        {
            float y=0;Text(new Rect(4,y,244,38),T("ui.council.title"),heading);y+=43;
            Text(new Rect(4,y,243,64),T("ui.council.intro"),small);y+=77;
            foreach(var faction in app.State.Factions)
            {
                var person=app.State.Characters.Find(p=>p.Id==faction.LeaderId);
                Rule(4,y,242);y+=17;
                Text(new Rect(5,y,240,29),T("faction."+faction.Id),heading);y+=38;
                Seal(new Rect(6,y+2,52,60),faction.Id=="urban"?red:C("#69765B"),faction.Id=="assembly"?1:0);
                if(person!=null){Text(new Rect(71,y,172,44),T(person.NameKey),body);Text(new Rect(71,y+45,172,44),T(person.PositionKey),small);}y+=94;
                Meter(5,y,236,T("ui.approval"),faction.Approval,C("#819168"));y+=46;
                Pair(5,y,236,T("ui.influence"),Number(faction.Influence));y+=29;Pair(5,y,236,T("ui.radicalism"),Number(faction.Radicalism));y+=36;
                Text(new Rect(5,y,236,65),T(faction.DemandKey),small);y+=74;
                if(person!=null){Text(new Rect(5,y,236,43),T("ui.character.traits",Number(person.Ambition),Number(person.Competence),Number(person.Relationship)),tiny);y+=52;}
            }
        }
        private void Economy(GameApp app,EconomyForecast forecast)
        {
            float y=0;Text(new Rect(4,y,244,36),T("ui.economy.title"),heading);y+=44;
            Text(new Rect(4,y,243,68),T("ui.economy.intro"),small);y+=83;
            Text(new Rect(4,y,243,25),T("ui.economy.treasury"),tiny);y+=35;
            LedgerLine(ref y,T("ui.economy.tax"),forecast.TaxIncome);LedgerLine(ref y,T("ui.economy.army"),-forecast.ArmyCost);y+=8;Rule(4,y,238);y+=17;LedgerLine(ref y,T("ui.economy.balance"),forecast.NetGold,true);y+=24;
            Text(new Rect(4,y,243,25),T("ui.economy.grain"),tiny);y+=35;
            LedgerLine(ref y,T("ui.economy.production"),forecast.Production);LedgerLine(ref y,T("ui.economy.population"),-forecast.CivilianConsumption);LedgerLine(ref y,T("ui.economy.rations"),-forecast.ArmyConsumption);LedgerLine(ref y,T("ui.economy.subsidy"),-forecast.SubsidyConsumption);y+=8;Rule(4,y,238);y+=17;LedgerLine(ref y,T("ui.economy.balance"),forecast.NetFood,true);y+=31;
            Text(new Rect(4,y,243,30),T("ui.economy.chain"),heading);y+=40;
            Text(new Rect(4,y,238,135),T("ui.economy.chain.detail"),body);y+=148;
            Fill(new Rect(4,y,238,126),pale);Fill(new Rect(4,y,3,126),brass);Text(new Rect(16,y+12,214,102),T("ui.economy.supply.detail"),small);y+=146;
            Text(new Rect(4,y,238,86),T("ui.economy.subsidy.note"),small);y+=92;
            if(Press(new Rect(4,y,238,41),T("ui.economy.paris")))app.SelectRegion("ile");
        }
        private void LedgerLine(ref float y,string label,int value,bool total=false)
        {
            Text(new Rect(5,y,163,45),label,total?body:small);var number=new GUIStyle(total?heading:body);number.alignment=TextAnchor.UpperRight;number.normal.textColor=value<0?red:ink;Text(new Rect(174,y,68,38),Signed(value),number);y+=49;
        }
        private void Journal(GameApp app)
        {
            Text(new Rect(4,0,243,35),T("ui.journal.title"),heading);float y=52;bool first=true;
            foreach(var entry in app.State.Journal)
            {
                if(first)
                {
                    float emphasis=Mathf.Clamp01(1f-(Time.unscaledTime-latestEntryTime)/5f);
                    Fill(new Rect(0,y-7,245,132),Color.Lerp(paper,C("#E2DEC5"),.25f+emphasis*.6f));
                    Fill(new Rect(0,y-7,2,132),brass);
                    var stamp=new GUIStyle(tiny);stamp.alignment=TextAnchor.UpperRight;stamp.normal.textColor=red;
                    Text(new Rect(141,y,99,22),T("ui.journal.latest"),stamp);first=false;
                }
                Text(new Rect(5,y,236,22),Date(entry.Week),tiny);y+=28;
                Text(new Rect(5,y,234,81),L.Text(entry.Key,entry.Args),body);y+=95;Rule(5,y,236);y+=17;
            }
        }

        private void Bottom(GameApp app)
        {
            Fill(new Rect(0,800,1440,100),deep);Fill(new Rect(0,800,1440,1),C("#5B6249"));
            Text(new Rect(20,814,225,34),Date(app.State.Week),numeral);
            if(Press(new Rect(20,858,77,27),T("ui.save")))app.Save();if(Press(new Rect(104,858,77,27),T("ui.load")))app.Load();if(Press(new Rect(188,858,51,27),T("ui.new")))confirmNew=true;
            Text(new Rect(265,813,838,42),string.IsNullOrEmpty(app.Message)?T("ui.welcome"):app.Message,lightBody);
            Text(new Rect(265,866,826,21),T("ui.shortcuts"),lightTiny);
            var nextRect=new Rect(1171,820,247,59);Fill(nextRect,brass);Border(nextRect,C("#D8C692"));var next=new GUIStyle(heading);next.alignment=TextAnchor.MiddleCenter;
            if(GUI.Button(nextRect,T("ui.next"),next))app.NextWeek();
            Text(new Rect(1174,883,244,16),T("ui.week",app.State.Week+1),lightTiny);
        }
        private static string Date(int week)
        {
            // Core permits very long simulation runs; cycle the calendar display safely beyond DateTime's range.
            long total=(long)week*7;DateTime start=new DateTime(1789,5,5);long max=(DateTime.MaxValue.Date-start).Days;
            DateTime date=start.AddDays(Math.Min(total,max));return T("ui.date",date.Day,T("ui.month."+date.Month),date.Year);
        }
        private void Help()
        {
            Fill(new Rect(0,0,1440,900),new Color(.07f,.13f,.10f,.74f));Rect sheet=new Rect(395,130,650,640);Fill(sheet,paper);Border(sheet,brass,2);
            Text(new Rect(424,153,590,26),T("ui.help.kicker"),tiny);Text(new Rect(424,191,590,51),T("ui.help.title"),title);
            Text(new Rect(424,259,590,420),T("ui.help.body"),body);
            if(Press(new Rect(424,704,590,45),T("ui.help.close"),true,true))showHelp=false;
        }
        private void Confirm(GameApp app)
        {
            Fill(new Rect(0,0,1440,900),new Color(.07f,.13f,.10f,.74f));Fill(new Rect(445,300,550,290),paper);Border(new Rect(445,300,550,290),brass);
            Text(new Rect(476,328,488,54),T("ui.restart.title"),title);Text(new Rect(476,397,488,76),T("ui.restart.body"),body);
            if(Press(new Rect(476,511,231,43),T("ui.cancel")))confirmNew=false;
            if(Press(new Rect(722,511,241,43),T("ui.restart.confirm"),true,true)){confirmNew=false;app.NewCampaign();}
        }
        private void OnDestroy(){if(disk)Destroy(disk);if(serifFont)Destroy(serifFont);if(sansFont)Destroy(sansFont);}
    }
}
