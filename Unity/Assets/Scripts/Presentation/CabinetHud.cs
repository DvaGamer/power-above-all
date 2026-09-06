using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace PowerAboveAll
{
    /// <summary>Ministerial marginalia around a permanently visible atlas. Drawn by GameApp after its 1440×900 GUI transform.</summary>
    public sealed partial class CabinetHud : MonoBehaviour
    {
        private readonly Color forest = C("#304F43"), deep = C("#243B37"), paper = C("#F3E7CA"), pale = C("#E9DCB7"), ink = C("#243B37"), muted = C("#53604D"), brass = C("#CAB36F"), red = C("#864B45"), rule = C("#C9C29E");
        private GUIStyle body, small, tiny, title, heading, numeral, button, quietButton, lightBody, lightTiny, mapLabel, cityLabel, tabStyle;
        private readonly Texture2D[] medallions = new Texture2D[4];
        private Texture2D portraitSheet;
        private float scrollGrabOffset;
        private Font serifFont, sansFont;
        private bool ready, showHelp, confirmNew, pendingMandateTerms;
        private Vector2 provinceScroll, documentScroll;
        private string document = "council";
        private string cachedLanguage;
        private readonly float[] displayedStocks = new float[6], targetStocks = new float[6];
        private readonly float[] stockFrom = new float[6], stockChangedAt = new float[6];
        private readonly int[] stockDirection = new int[6];
        private readonly bool[] stockInitialized = new bool[6];
        private LogEntry latestEntry;
        private float latestEntryTime;
        private float provinceContentHeight = 1100;
        private float documentContentHeight = 584;
        private string shownProvince;
        private CampaignState previewSource, nextState;
        private LogEntry previewEntry;
        private string previewRegion;
        private ActionResult weekCheck;
        private int additionalRecruitPayroll, additionalRecruitFood;
        private readonly Dictionary<string, ActionResult> orderChecks = new Dictionary<string, ActionResult>();
        private bool weeklyChange;
        private int observedWeek;
        public bool BlocksMapInput { get { return showHelp || confirmNew; } }
        private static readonly string[] ModeNames = { "control", "unrest", "food", "tax", "army", "influence" };

        public void OpenDocument(string name)
        {
            if(name!="council"&&name!="economy"&&name!="journal"&&name!="mandate"&&name!="accord")return;
            document=name;documentScroll=Vector2.zero;documentContentHeight=584;pendingMandateTerms=false;
        }

        public void ShowMandateTerms()
        {
            OpenDocument("mandate");pendingMandateTerms=true;
        }

        public void Draw(GameApp app)
        {
            if (app == null || app.State == null || app.BattleActive) return;
            EnsureStyles();
            if (cachedLanguage != L.Language) { cachedLanguage = L.Language; provinceScroll = documentScroll = Vector2.zero; }
            CampaignState state = app.State; EconomyForecast forecast = CampaignCore.Forecast(state);
            Observe(state);
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
            body = Style(14, ink); small = Style(13, muted); tiny = Style(12, muted); title = Style(29, ink, true); heading = Style(21, ink, true); numeral = Style(28, paper);
            lightBody = Style(14, paper); lightTiny = Style(12, C("#D0D4B9"));
            mapLabel = Style(15, ink, true); mapLabel.alignment = TextAnchor.MiddleCenter;
            cityLabel = Style(12, C("#435C4B"), true); cityLabel.fontStyle = FontStyle.Italic; cityLabel.alignment = TextAnchor.MiddleCenter;
            button = Style(14, paper); button.alignment = TextAnchor.MiddleCenter;
            quietButton = Style(13, ink); quietButton.alignment = TextAnchor.MiddleCenter;
            tabStyle = Style(12, muted); tabStyle.alignment = TextAnchor.MiddleCenter;
            portraitSheet=Resources.Load<Texture2D>("Art/PoliticalPortraits-v1");
            for(int i=0;i<medallions.Length;i++)medallions[i]=EngravedMedallion(i);
        }
        private GUIStyle Style(int size, Color color, bool serif = false)
        {
            var style = new GUIStyle(GUI.skin.label) { font=serif?serifFont:sansFont,fontSize=size,wordWrap=true,richText=false,padding=new RectOffset(0,0,0,0),margin=new RectOffset(0,0,0,0) };
            style.normal.textColor=style.hover.textColor=style.active.textColor=style.focused.textColor=color;return style;
        }
        private static Color C(string html){ColorUtility.TryParseHtmlString(html,out var color);return color;}
        private static string T(string key,params object[] args){return L.Text(key,args);}
        private static string Number(float value){return Mathf.RoundToInt(value).ToString("N0",CultureInfo.GetCultureInfo(L.Language=="tr"?"tr-TR":"ru-RU"));}
        private static string Number(long value){return value.ToString("N0",CultureInfo.GetCultureInfo(L.Language=="tr"?"tr-TR":"ru-RU"));}
        private static string Signed(int value){return (value>=0?"+":"−")+Number(Math.Abs(value));}
        private static string Change(float value){return (value>=0?"+":"−")+Math.Abs(value).ToString("0.#",CultureInfo.GetCultureInfo(L.Language=="tr"?"tr-TR":"ru-RU"));}
        private void Observe(CampaignState state)
        {
            LogEntry current=state.Journal.Count>0?state.Journal[0]:null;
            bool replaced=!ReferenceEquals(previewSource,state);
            if(!replaced&&ReferenceEquals(previewEntry,current)&&previewRegion==state.SelectedRegionId)return;
            if(replaced)
            {
                Array.Clear(stockInitialized,0,stockInitialized.Length);
                Array.Clear(stockDirection,0,stockDirection.Length);
                documentScroll=provinceScroll=Vector2.zero;
            }
            weeklyChange=!replaced&&state.Week!=observedWeek;
            observedWeek=state.Week;
            if(!ReferenceEquals(latestEntry,current))
            {
                // Yeni kayıtlar üstte eklenirken okunmakta olan satır yerinde kalır.
                if(!replaced&&document=="journal"&&documentScroll.y>1f&&latestEntry!=null)
                {
                    int previous=state.Journal.IndexOf(latestEntry);
                    if(previous>0)for(int i=0;i<previous;i++)documentScroll.y+=JournalEntryHeight(state.Journal[i]);
                }
                latestEntry=current;latestEntryTime=Time.unscaledTime;
            }
            previewSource=state;previewEntry=current;previewRegion=state.SelectedRegionId;
            ObserveRegionalAccord(state);
            // Salt okunur sunum: gerçek çekirdek kuralları yalnızca derin kopyada hesaplanır.
            string snapshot=JsonUtility.ToJson(state);
            nextState=JsonUtility.FromJson<CampaignState>(snapshot);
            weekCheck=CampaignCore.NextWeek(nextState);
            var currentEconomy=CampaignCore.Forecast(state);
            additionalRecruitPayroll=additionalRecruitFood=0;
            foreach(string action in new[]{"bread","tax","recruit","subsidy"})
            {
                var proposed=JsonUtility.FromJson<CampaignState>(snapshot);
                orderChecks[action]=CampaignCore.Act(proposed,action,state.SelectedRegionId);
                if(action!="recruit"||!orderChecks[action].Ok)continue;
                var afterRecruit=CampaignCore.Forecast(proposed);
                additionalRecruitPayroll=afterRecruit.ArmyCost-currentEconomy.ArmyCost;
                additionalRecruitFood=afterRecruit.ArmyConsumption-currentEconomy.ArmyConsumption;
            }
        }
        private string Animated(int index,int actual)
        {
            string moving=Animated(index,(float)actual);
            return displayedStocks[index]==targetStocks[index]?Number((long)actual):moving;
        }
        private string Animated(int index,float actual)
        {
            if(!stockInitialized[index]){stockInitialized[index]=true;displayedStocks[index]=actual;}
            else if(targetStocks[index]!=actual)
            {
                stockFrom[index]=displayedStocks[index];stockDirection[index]=actual>targetStocks[index]?1:-1;
                stockChangedAt[index]=Time.unscaledTime+(weeklyChange?(index==0?0f:index==1?.13f:index==5?.26f:.13f):0f);
            }
            targetStocks[index]=actual;
            return Number(displayedStocks[index]);
        }
        private void Update()
        {
            for(int i=0;i<displayedStocks.Length;i++)
            {
                if(!stockInitialized[i]||displayedStocks[i]==targetStocks[i])continue;
                float progress=Mathf.Clamp01((Time.unscaledTime-stockChangedAt[i])/.18f);
                displayedStocks[i]=Mathf.Lerp(stockFrom[i],targetStocks[i],progress*progress*(3f-2f*progress));
            }
        }
        private static void Fill(Rect rect,Color color){Color before=GUI.color;GUI.color=color;GUI.DrawTexture(rect,Texture2D.whiteTexture);GUI.color=before;}
        private static void Border(Rect rect,Color color,float width=1){Fill(new Rect(rect.x,rect.y,rect.width,width),color);Fill(new Rect(rect.x,rect.yMax-width,rect.width,width),color);Fill(new Rect(rect.x,rect.y,width,rect.height),color);Fill(new Rect(rect.xMax-width,rect.y,width,rect.height),color);}
        private void Text(Rect rect,string text,GUIStyle style){GUI.Label(rect,text,style);}
        private void Rule(float x,float y,float width){Fill(new Rect(x,y,width,1),rule);}
        private bool Press(Rect rect,string text,bool enabled=true,bool primary=false)
        {
            enabled=enabled&&GUI.enabled;
            bool over=rect.Contains(Event.current.mousePosition),down=over&&Input.GetMouseButton(0)&&enabled;
            Color fill=primary?forest:pale;
            if(over&&enabled)fill=primary?C("#416553"):C("#DEE2BD");
            if(down)fill=Color.Lerp(fill,ink,.14f);
            if(!enabled)fill=Color.Lerp(pale,paper,.4f);
            Fill(rect,fill);Border(rect,enabled&&primary?forest:C("#B9BD99"));
            if(over&&enabled)Fill(new Rect(rect.x+1,rect.yMax-2,rect.width-2,1),brass);
            bool previous=GUI.enabled;GUI.enabled=enabled;
            bool hit=GUI.Button(rect,GUIContent.none,GUIStyle.none);
            var style=new GUIStyle(primary?button:quietButton);if(down)style.contentOffset=new Vector2(0,1);
            if(!enabled)style.normal.textColor=muted;
            // Etkileşim kapalı olsa da emrin adı okunur; Unity'nin ikinci soldurmasını uygulama.
            GUI.enabled=true;GUI.Label(rect,text,style);GUI.enabled=previous;return hit;
        }
        private Vector2 BeginMatteScroll(Rect viewport,Vector2 scroll,Rect content,int hint)
        {
            float maximum=Mathf.Max(0,content.height-viewport.height);
            scroll.x=0;scroll.y=Mathf.Clamp(scroll.y,0,maximum);
            Rect rail=new Rect(viewport.xMax-13,viewport.y,13,viewport.height);
            int control=GUIUtility.GetControlID(hint,FocusType.Passive,rail);
            float thumbHeight=Mathf.Min(viewport.height,Mathf.Max(28,viewport.height*viewport.height/content.height));
            float travel=Mathf.Max(0,viewport.height-thumbHeight);
            float thumbY=viewport.y+(maximum>0?scroll.y/maximum*travel:0);
            Event current=Event.current;
            if(maximum>0&&GUI.enabled)
            {
                if(current.type==EventType.ScrollWheel&&viewport.Contains(current.mousePosition))
                {scroll.y=Mathf.Clamp(scroll.y+current.delta.y*24,0,maximum);current.Use();}
                if(current.type==EventType.MouseDown&&current.button==0&&rail.Contains(current.mousePosition))
                {
                    GUIUtility.hotControl=control;
                    scrollGrabOffset=current.mousePosition.y>=thumbY&&current.mousePosition.y<=thumbY+thumbHeight?current.mousePosition.y-thumbY:thumbHeight*.5f;
                    scroll.y=travel>0?Mathf.Clamp01((current.mousePosition.y-viewport.y-scrollGrabOffset)/travel)*maximum:0;
                    current.Use();
                }
                else if(current.type==EventType.MouseDrag&&GUIUtility.hotControl==control)
                {scroll.y=travel>0?Mathf.Clamp01((current.mousePosition.y-viewport.y-scrollGrabOffset)/travel)*maximum:0;current.Use();}
            }
            if(current.type==EventType.MouseUp&&GUIUtility.hotControl==control)
            {GUIUtility.hotControl=0;current.Use();}
            if(maximum>0)
            {
                thumbY=viewport.y+scroll.y/maximum*travel;
                bool over=rail.Contains(current.mousePosition)||GUIUtility.hotControl==control;
                Fill(new Rect(rail.x+6,rail.y,1,rail.height),rule);
                Fill(new Rect(rail.x+4,thumbY,5,thumbHeight),over?ink:muted);
                Fill(new Rect(rail.x+5,thumbY+2,1,Mathf.Max(0,thumbHeight-4)),Color.Lerp(paper,brass,.6f));
            }
            // Unity yalnızca içerik kırpmasını yönetir; hazır parlak kaydırıcı çizilmez.
            Rect page=new Rect(viewport.x,viewport.y,viewport.width-15,viewport.height);
            return GUI.BeginScrollView(page,scroll,content,false,false,GUIStyle.none,GUIStyle.none);
        }
        private static float EllipseDistance(Vector2 point,Vector2 center,Vector2 radius)
        {
            Vector2 delta=point-center;
            return (1f-Mathf.Sqrt(delta.x*delta.x/(radius.x*radius.x)+delta.y*delta.y/(radius.y*radius.y)))*Mathf.Min(radius.x,radius.y);
        }
        private static float EdgeDistance(Vector2 point,Vector2 a,Vector2 b)
        {
            Vector2 edge=b-a;
            return (point-a-edge*Mathf.Clamp01(Vector2.Dot(point-a,edge)/edge.sqrMagnitude)).magnitude;
        }
        private static float ContourDistance(Vector2 point,Vector2[] points)
        {
            bool inside=false;float squared=10;
            for(int i=0,j=points.Length-1;i<points.Length;j=i++)
            {
                Vector2 a=points[j],b=points[i],edge=b-a;
                float length=edge.sqrMagnitude;
                if(length<=.00000001f)continue;
                Vector2 nearest=point-a-edge*Mathf.Clamp01(Vector2.Dot(point-a,edge)/length);
                squared=Mathf.Min(squared,nearest.sqrMagnitude);
                if((a.y>point.y)!=(b.y>point.y)&&point.x<(b.x-a.x)*(point.y-a.y)/(b.y-a.y)+a.x)inside=!inside;
            }
            return Mathf.Sqrt(squared)*(inside?1:-1);
        }
        private static Vector2[] SmoothContour(Vector2[] points)
        {
            var result=new Vector2[points.Length*4];
            for(int i=0;i<points.Length;i++)for(int step=0;step<4;step++)
            {
                Vector2 a=points[(i+points.Length-1)%points.Length],b=points[i],c=points[(i+1)%points.Length],d=points[(i+2)%points.Length];
                float t=step*.25f;
                result[i*4+step]=.5f*((2*b)+(-a+c)*t+(2*a-5*b+4*c-d)*t*t+(-a+3*b-3*c+d)*t*t*t);
            }
            return result;
        }
        private Texture2D EngravedMedallion(int variant)
        {
            // Kurgusal kişilerin özgün gravür eskizi; tarihsel portre kaynağı değildir.
            const int width=144,height=176;
            var texture=new Texture2D(width,height,TextureFormat.RGBA32,false){name="Council engraved miniature "+variant,filterMode=FilterMode.Bilinear,wrapMode=TextureWrapMode.Clamp};
            var face=SmoothContour(new[]{new Vector2(.40f,.79f),new Vector2(.54f,.80f),new Vector2(.63f,.75f),new Vector2(.64f,.68f),new Vector2(.64f,.64f),new Vector2(.68f,.60f),new Vector2(.73f+(variant==1?.015f:0),.57f),new Vector2(.70f,.55f),new Vector2(.675f,.55f),new Vector2(.69f,.524f),new Vector2(.678f,.51f),new Vector2(.65f,.478f),new Vector2(.59f,.456f),new Vector2(.60f,.37f),new Vector2(.44f,.34f),new Vector2(.445f,.47f),new Vector2(.37f,.56f),new Vector2(.35f,.70f)});
            var coat=SmoothContour(new[]{new Vector2(.20f,.18f),new Vector2(.27f,.29f),new Vector2(.42f,.34f),new Vector2(.47f,.38f),new Vector2(.60f,.35f),new Vector2(.65f,.30f),new Vector2(.78f,.26f),new Vector2(.84f,.18f)});
            Color ground=variant==2?C("#815F51"):C("#596B55"), skin=C("#D9CFAB"), shadow=C("#A5A384"), hair=variant>=2?C("#6F735C"):C("#C0BCA0");
            var pixels=new Color[width*height];
            for(int y=0;y<height;y++)for(int x=0;x<width;x++)
            {
                Vector2 point=new Vector2((x+.5f)/width,(y+.5f)/height);
                float edge=EllipseDistance(point,new Vector2(.5f,.5f),new Vector2(.465f,.478f));
                if(edge<-.006f){pixels[y*width+x]=Color.clear;continue;}
                Color color=Color.Lerp(ground,brass,.08f+point.y*.09f);
                float oval=Mathf.Sqrt(Mathf.Pow((point.x-.5f)/.465f,2)+Mathf.Pow((point.y-.5f)/.478f,2));
                color=Color.Lerp(color,brass,Mathf.Clamp01(1-Mathf.Abs(oval-.946f)*120)*.68f);
                float coatMask=Mathf.Clamp01(ContourDistance(point,coat)*height+.5f);
                color=Color.Lerp(color,Color.Lerp(deep,ground,.62f),coatMask);
                float hairMask=EllipseDistance(point,new Vector2(.425f,.674f),new Vector2(.187f,.212f));
                if(variant>=2&&point.y<.64f)hairMask=-1;
                color=Color.Lerp(color,hair,Mathf.Clamp01(hairMask*height+.5f));
                float faceDistance=ContourDistance(point,face),faceMask=Mathf.Clamp01(faceDistance*height+.5f);
                Color flesh=Color.Lerp(shadow,skin,Mathf.Clamp01((point.x-.35f)*2.8f+.22f));
                float hatching=Mathf.Pow(Mathf.Max(0,Mathf.Sin((point.x+point.y*.36f)*105)),14)*.12f;
                flesh=Color.Lerp(flesh,ground,hatching+Mathf.Clamp01(1-Mathf.Abs(faceDistance)*190)*.18f);
                color=Color.Lerp(color,flesh,faceMask);
                if(variant<2)
                {
                    for(int curl=0;curl<5;curl++)
                    {
                        float curlDistance=EllipseDistance(point,new Vector2(.307f+Mathf.Sin(curl*.8f)*.018f,.70f-curl*.063f),new Vector2(.061f,.043f));
                        float curlMask=Mathf.Clamp01(curlDistance*height+.5f);
                        Color curlColor=Color.Lerp(hair,shadow,Mathf.Clamp01(1-curlDistance*65)*.55f);
                        color=Color.Lerp(color,curlColor,curlMask);
                    }
                }
                float ear=EllipseDistance(point,new Vector2(.464f,.605f),new Vector2(.029f,.044f));
                color=Color.Lerp(color,shadow,Mathf.Clamp01(1-Mathf.Abs(ear)*240)*faceMask*.5f);
                float eye=EdgeDistance(point,new Vector2(.602f,.653f),new Vector2(.629f,.647f));
                float brow=EdgeDistance(point,new Vector2(.597f,.679f),new Vector2(.634f,.671f));
                float mouth=EdgeDistance(point,new Vector2(.657f,.528f),new Vector2(.685f,.525f));
                float lapel=Mathf.Min(EdgeDistance(point,new Vector2(.402f,.328f),new Vector2(.49f,.201f)),EdgeDistance(point,new Vector2(.605f,.322f),new Vector2(.55f,.203f)));
                color=Color.Lerp(color,ink,Mathf.Clamp01((.006f-Mathf.Min(eye,Mathf.Min(brow,mouth)))*height)*faceMask*.86f);
                color=Color.Lerp(color,brass,Mathf.Clamp01((.006f-lapel)*height)*coatMask*.68f);
                color.a=Mathf.Clamp01(edge*height+.5f);pixels[y*width+x]=color;
            }
            texture.SetPixels(pixels);texture.Apply(false,true);return texture;
        }
        private void Seal(Rect rect,int variant)
        {
            variant=Mathf.Clamp(variant,0,medallions.Length-1);
            if(portraitSheet)
            {
                var uv=new Rect((variant%2)*.5f,variant<2?.5f:0f,.5f,.5f);
                float aspect=(float)portraitSheet.width/portraitSheet.height;
                float width=Mathf.Min(rect.width,rect.height*aspect),height=width/aspect;
                rect=new Rect(rect.x+(rect.width-width)*.5f,rect.y+(rect.height-height)*.5f,width,height);
                GUI.DrawTextureWithTexCoords(rect,portraitSheet,uv,true);
            }
            else GUI.DrawTexture(rect,medallions[variant],ScaleMode.ScaleToFit,true);
        }

        private void Top(GameApp app,EconomyForecast forecast)
        {
            Fill(new Rect(0,0,1440,94),deep);Fill(new Rect(0,93,1440,1),C("#5B6249"));
            Border(new Rect(24,20,43,52),C("#8A855A"));Text(new Rect(28,25,35,39),T("ui.seal"),numeral);
            var brand=new GUIStyle(title);brand.fontSize=24;brand.normal.textColor=paper;
            Text(new Rect(82,23,252,31),T("ui.title"),brand);
            var tagline=new GUIStyle(lightTiny){wordWrap=false,clipping=TextClipping.Clip};
            Text(new Rect(84,60,236,18),T("ui.tagline"),tagline);
            Resource(0,337,T("ui.gold"),Animated(0,app.State.Gold),T("ui.weekly",Signed(forecast.NetGold)),brass);
            Resource(1,490,T("ui.food"),Animated(1,app.State.Food),T("ui.weekly",Signed(forecast.NetFood)),brass);
            Resource(2,643,T("ui.supplies"),Animated(2,app.State.MilitarySupplies),T("ui.stock"),brass);
            Resource(3,796,T("ui.troops"),Animated(3,app.State.Troops),T("ui.reserve",Number(app.State.Manpower)),brass);
            Resource(4,949,T("ui.power"),Animated(4,app.State.Power),T("ui.personal"),brass);
            Resource(5,1102,T("ui.unrest"),Animated(5,CampaignCore.AverageUnrest(app.State)),T("ui.country"),C("#DEAB94"));
            var language=new Rect(1275,24,72,32);if(Press(language,T("ui.language")))app.SetLanguage(L.Language=="ru"?"tr":"ru");
            if(Press(new Rect(1359,24,54,32),T("ui.help.short"))){showHelp=!showHelp;app.Feedback("paper");}
            CabinetAudio audio=app.GetComponent<CabinetAudio>();
            if(audio!=null&&Press(new Rect(1275,64,138,23),T(audio.Muted?"ui.sound.off":"ui.sound.on")))
            {
                audio.SetMuted(!audio.Muted);
                if(!L.IsReviewSession){PlayerPrefs.SetInt("muted",audio.Muted?1:0);PlayerPrefs.Save();}
                if(!audio.Muted)app.Feedback("paper");
            }
        }
        private void Resource(int index,float x,string label,string value,string detail,Color accent)
        {
            Fill(new Rect(x-16,24,1,48),C("#60735D"));Text(new Rect(x,16,140,19),label,lightTiny);
            float emphasis=stockDirection[index]==0?0f:Mathf.Clamp01(1f-(Time.unscaledTime-stockChangedAt[index])/.9f);
            bool benefit=index==5?stockDirection[index]<0:stockDirection[index]>0;
            var style=new GUIStyle(numeral);style.normal.textColor=Color.Lerp(accent,benefit?C("#B9CC9D"):C("#DBAA91"),emphasis);
            style.fontSize=value.Length>8?23:28;
            style.wordWrap=false;
            while(style.fontSize>18&&style.CalcSize(new GUIContent(value)).x>142)style.fontSize--;
            Text(new Rect(x,34,142,35),value,style);Text(new Rect(x,71,142,17),detail,lightTiny);
            if(emphasis>0f)Fill(new Rect(x,69,140,1),new Color(style.normal.textColor.r,style.normal.textColor.g,style.normal.textColor.b,emphasis*.6f));
        }
        private void Atlas(GameApp app)
        {
            Fill(new Rect(245,94,895,43),C("#DCE1BC"));Rule(245,136,895);
            float modeWidth=895f/ModeNames.Length;
            for(int i=0;i<ModeNames.Length;i++)
            {
                Rect rect=new Rect(245+i*modeWidth,94,modeWidth,43);bool active=app.Mode==ModeNames[i];
                if(active){Fill(rect,paper);Fill(new Rect(rect.x+12,rect.yMax-3,rect.width-24,3),C("#4F7361"));}
                var style=new GUIStyle(tabStyle);if(active)style.normal.textColor=ink;
                if(GUI.Button(rect,T("ui.mode."+ModeNames[i]),style)){app.SetMode(ModeNames[i]);app.Feedback("paper");}
            }
            var mapTitle=new GUIStyle(heading);mapTitle.alignment=TextAnchor.MiddleCenter;mapTitle.normal.textColor=ink;
            Text(new Rect(400,149,585,32),T("ui.atlas.title"),mapTitle);
            var caption=new GUIStyle(tiny);caption.alignment=TextAnchor.MiddleCenter;
            Text(new Rect(420,181,545,18),T("ui.atlas.subtitle"),caption);
            foreach(var definition in CampaignCore.Regions)
            {
                Vector3 screen=app.Camera.WorldToScreenPoint(app.Map.RegionWorld(definition.Id));
                Vector2 point=ViewLayout.ToCanvas(screen);float x=point.x,y=point.y;
                if(x<265||x>1118||y<206||y>749)continue;
                var style=new GUIStyle(mapLabel);if(definition.Id==app.State.SelectedRegionId){style.fontStyle=FontStyle.Bold;style.normal.textColor=C("#2C422C");}
                float nameOffset=definition.Id=="champagne"?-18f:0f;
                Text(new Rect(x-83,y-34+nameOffset,166,24),T("region."+definition.Id),style);
                Text(new Rect(x-65,y+14,130,19),T("city."+definition.Id),cityLabel);
            }
            var geography=new GUIStyle(cityLabel);geography.fontSize=18;geography.normal.textColor=C("#486F70");
            Text(new Rect(271,448,132,60),T("ui.atlas.atlantic"),geography);
            Text(new Rect(699,714,300,28),T("ui.atlas.mediterranean"),geography);
            Text(new Rect(306,224,180,24),T("ui.atlas.channel"),geography);
            // Brass compass engraved in the sea, not an interactive control.
            var compass=new GUIStyle(heading);compass.alignment=TextAnchor.MiddleCenter;compass.normal.textColor=C("#46665E");
            Text(new Rect(280,654,65,25),T("ui.atlas.north"),tiny);Text(new Rect(280,678,65,45),T("ui.compass"),compass);
            Fill(new Rect(259,769,868,24),C("#E6DFC0"));
            Fill(new Rect(272,775,13,12),CampaignMap.ModeColor(app.Mode,0));Border(new Rect(272,775,13,12),rule);
            Fill(new Rect(289,775,13,12),CampaignMap.ModeColor(app.Mode,1));Border(new Rect(289,775,13,12),rule);
            Text(new Rect(314,773,623,18),T("ui.legend."+app.Mode),tiny);Text(new Rect(956,773,165,18),T("ui.atlas.scale"),tiny);
        }

        private void Province(GameApp app)
        {
            CampaignState state=app.State;RegionState region=CampaignCore.Region(state,state.SelectedRegionId);
            RegionDefinition definition=Array.Find(CampaignCore.Regions,r=>r.Id==state.SelectedRegionId);
            if(region==null||definition==null)return;
            if(shownProvince!=region.Id){shownProvince=region.Id;provinceScroll=Vector2.zero;}
            Fill(new Rect(0,94,245,706),paper);Fill(new Rect(244,94,1,706),C("#AEBBA0"));
            Text(new Rect(18,110,213,22),T("ui.province.dispatch"),tiny);Rule(18,141,208);
            var regionTitle=new GUIStyle(title);regionTitle.fontSize=26;
            Text(new Rect(18,152,212,64),T("region."+region.Id),regionTitle);
            Text(new Rect(19,214,209,24),T("city."+region.Id),small);
            // Emirler sabit kalır; yalnızca alttaki durum raporu kaydırılır.
            GUI.BeginGroup(new Rect(12,246,226,540));
            float y=0;Text(new Rect(4,y,195,25),T("ui.orders"),tiny);y+=29;
            Order(app,ref y,"bread",T("ui.order.bread"),T("ui.order.bread.detail"));
            Order(app,ref y,"tax",T("ui.order.tax"),T("ui.order.tax.detail"));
            Order(app,ref y,"recruit",T("ui.order.recruit"),T("ui.order.recruit.detail"));
            var march=CampaignCore.CanMarch(state,region.Id);bool here=state.ArmyRegionId==region.Id;
            // Düğme durumu değiştirebilir; varış tahmini tıklamadan önce alınır.
            var arrival=!here&&march.Ok?CampaignCore.PreviewMarch(state,region.Id):null;
            int movementCost=arrival==null?0:state.Moves-arrival.MovesAfter;
            if(Press(new Rect(4,y,195,34),T(here?"ui.army.here":march.RequiresBattle?"ui.army.battle":"ui.army.march"),!here&&march.Ok,true))app.March();y+=40;
            string marchDetail=here?T("ui.army.here.detail"):arrival!=null?T("ui.march.cost",Number(arrival.FoodCost),Number(movementCost)):L.Text(march.Key,march.Args);
            var marchStyle=new GUIStyle(tiny);if(!here&&!march.Ok)marchStyle.normal.textColor=red;
            Paragraph(ref y,marchDetail,marchStyle,195,8);
            if(arrival!=null&&arrival.Hungry)
            {
                var warning=new GUIStyle(tiny);warning.normal.textColor=red;Paragraph(ref y,T("ui.march.hungry"),warning,195,8);
            }
            if(region.Id=="ile")Order(app,ref y,"subsidy",T(state.SubsidyParis?"ui.order.subsidy.stop":"ui.order.subsidy"),T(state.SubsidyParis?"ui.order.subsidy.stop.detail":"ui.order.subsidy.detail"));
            Rule(4,y+3,195);y+=15;
            float reportTop=246+y,reportHeight=Mathf.Max(1,786-reportTop);
            GUI.EndGroup();
            provinceScroll=BeginMatteScroll(new Rect(12,reportTop,226,reportHeight),provinceScroll,new Rect(0,0,205,Mathf.Max(reportHeight,provinceContentHeight)),178901);
            y=4;
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
        private void Order(GameApp app,ref float y,string action,string name,string detail)
        {
            ActionResult check=orderChecks[action];
            bool breaksAccord=action=="tax"&&CampaignCore.TaxBreaksRegionalAccord(app.State,app.State.SelectedRegionId);
            if(Press(new Rect(4,y,195,34),breaksAccord?T("ui.accord.tax_button"):name,check.Ok))app.Act(action);y+=40;
            if(!check.Ok)
            {
                string reason=OrderReason(app.State,check);var reasonStyle=new GUIStyle(tiny);reasonStyle.normal.textColor=red;
                float reasonHeight=reasonStyle.CalcHeight(new GUIContent(reason),188);
                Text(new Rect(7,y,188,reasonHeight),reason,reasonStyle);y+=reasonHeight+8;
                return;
            }
            float detailHeight=tiny.CalcHeight(new GUIContent(detail),188);
            Text(new Rect(7,y,188,detailHeight),detail,tiny);y+=detailHeight+8;
            if(breaksAccord)
            {
                var warning=new GUIStyle(tiny);warning.normal.textColor=red;
                var effect=accordPreview.Break;
                string consequence=T("ui.accord.tax_warning",Change(effect.Unrest),Change(effect.Control),Change(effect.Relationship),Change(effect.Approval),Change(effect.Power));
                float warningHeight=warning.CalcHeight(new GUIContent(consequence),188);
                Text(new Rect(7,y,188,warningHeight),consequence,warning);y+=warningHeight+8;
            }
            if(action=="recruit")
            {
                string upkeep=T("ui.recruit.upkeep",Signed(-additionalRecruitPayroll),Signed(-additionalRecruitFood));
                float upkeepHeight=tiny.CalcHeight(new GUIContent(upkeep),188);
                Text(new Rect(7,y,188,upkeepHeight),upkeep,tiny);y+=upkeepHeight+8;
            }
        }
        private string OrderReason(CampaignState state,ActionResult check)
        {
            if(check.Key=="error.bread.cost")return T("ui.reason.missing",T("ui.food"),Number(40-state.Food));
            if(check.Key!="error.recruit.cost")return L.Text(check.Key,check.Args);
            var shortages=new List<string>();
            if(state.Gold<120)shortages.Add(T("ui.reason.missing",T("ui.gold"),Number(120-state.Gold)));
            if(state.Food<20)shortages.Add(T("ui.reason.missing",T("ui.food"),Number(20-state.Food)));
            if(state.MilitarySupplies<15)shortages.Add(T("ui.reason.missing",T("ui.supplies"),Number(15-state.MilitarySupplies)));
            if(state.Manpower<200)shortages.Add(T("ui.reason.missing",T("ui.manpower"),Number(200-state.Manpower)));
            return string.Join("\n",shortages);
        }
        private void Pair(float x,float y,float width,string key,string value)
        {
            Text(new Rect(x,y,width*.65f,28),key,small);var right=new GUIStyle(body);right.alignment=TextAnchor.UpperRight;Text(new Rect(x+width*.61f,y,width*.39f,28),value,right);
        }
        private void Meter(float x,float y,float width,string key,float value,Color color)
        {
            Pair(x,y,width,key,Number(value));Fill(new Rect(x,y+27,width,4),C("#D5D6BF"));Fill(new Rect(x,y+27,width*Mathf.Clamp01(value/100f),4),color);
        }
        private void Paragraph(ref float y,string text,GUIStyle style,float width=238,float after=12)
        {
            float height=style.CalcHeight(new GUIContent(text),width);
            Text(new Rect(4,y,width,height),text,style);y+=height+after;
        }

        private void Cabinet(GameApp app,EconomyForecast forecast)
        {
            Fill(new Rect(1140,94,300,706),paper);Fill(new Rect(1140,94,1,706),C("#AAB79C"));
            Text(new Rect(1161,109,258,20),T("ui.cabinet"),tiny);Rule(1161,141,257);
            string[] names={"council","economy","journal","mandate"};
            for(int i=0;i<names.Length;i++)
            {
                Rect rect=new Rect(1156+i*67,151,65,36);bool selected=document==names[i]||(document=="accord"&&names[i]=="council");if(selected){Fill(rect,pale);Fill(new Rect(rect.x,rect.yMax-2,rect.width,2),C("#839371"));}
                if(GUI.Button(rect,T(names[i]=="mandate"?"ui.mandate.tab":"ui.tab."+names[i]),tabStyle)){OpenDocument(names[i]);app.Feedback("paper");}
            }
            if(document=="journal")
            {
                documentContentHeight=heading.CalcHeight(new GUIContent(T("ui.journal.title")),242)+29;
                foreach(var entry in app.State.Journal)documentContentHeight+=JournalEntryHeight(entry);
            }
            documentScroll=BeginMatteScroll(new Rect(1156,201,278,584),documentScroll,new Rect(0,0,251,Mathf.Max(584,documentContentHeight)),178902);
            if(document=="council")Council(app);else if(document=="economy")Economy(app,forecast);else if(document=="mandate")Mandate(app);else if(document=="accord")RegionalAccord(app);else Journal(app);
            GUI.EndScrollView();
        }
        private void Council(GameApp app)
        {
            float y=0;Paragraph(ref y,T("ui.council.title"),heading,242,12);
            Paragraph(ref y,T("ui.council.intro"),small,242,19);
            RegionalAccordEntry(app,ref y);
            foreach(var faction in app.State.Factions)
            {
                var person=app.State.Characters.Find(p=>p.Id==faction.LeaderId);
                Rule(4,y,242);y+=17;
                Paragraph(ref y,T("faction."+faction.Id),heading,240,12);
                Seal(new Rect(2,y,78,88),faction.Id=="assembly"?1:faction.Id=="urban"?2:faction.Id=="army"?3:0);
                float personHeight=88;
                if(person!=null)
                {
                    float nameHeight=body.CalcHeight(new GUIContent(T(person.NameKey)),154);
                    float roleHeight=small.CalcHeight(new GUIContent(T(person.PositionKey)),154);
                    Text(new Rect(89,y+3,154,nameHeight),T(person.NameKey),body);
                    Text(new Rect(89,y+nameHeight+8,154,roleHeight),T(person.PositionKey),small);
                    personHeight=Mathf.Max(personHeight,nameHeight+roleHeight+8);
                }
                y+=personHeight+16;
                if(person!=null){Pair(5,y,236,T("ui.person.relationship"),Number(person.Relationship));y+=32;}
                Meter(5,y,236,T("ui.approval"),faction.Approval,C("#819168"));y+=46;
                Pair(5,y,236,T("ui.influence"),Number(faction.Influence));y+=29;Pair(5,y,236,T("ui.radicalism"),Number(faction.Radicalism));y+=36;
                Paragraph(ref y,T(faction.DemandKey),body,236,10);
                if(person!=null)Paragraph(ref y,T(person.AgendaKey),small,236,19);
            }
            documentContentHeight=y+12;
        }
        private void Economy(GameApp app,EconomyForecast forecast)
        {
            CampaignState state=app.State;
            float y=0;Paragraph(ref y,T("ui.economy.title"),heading,242,10);
            Paragraph(ref y,T("ui.economy.intro"),small,242,18);
            if(!weekCheck.Ok)
            {
                var warning=new GUIStyle(small);warning.normal.textColor=red;
                Paragraph(ref y,L.Text(weekCheck.Key,weekCheck.Args),warning);
            }
            StockProjection(ref y,T("ui.gold"),state.Gold,nextState.Gold,forecast.NetGold);
            StockProjection(ref y,T("ui.food"),state.Food,nextState.Food,forecast.NetFood);
            Rule(4,y,238);y+=18;
            Paragraph(ref y,T("ui.economy.treasury"),tiny,238,13);
            LedgerLine(ref y,T("ui.economy.tax"),forecast.TaxIncome);
            if(accordPreview!=null&&accordPreview.IsActive)
                Paragraph(ref y,T("ui.accord.economy",T("region."+accordPreview.RegionId),Number(accordPreview.TaxForgone),accordPreview.RemainingWeeks),small,238,14);
            int payroll=(int)Math.Ceiling(state.Troops/12d);
            LedgerLine(ref y,T("ui.economy.payroll"),-payroll);
            LedgerLine(ref y,T("ui.economy.equipment"),-(forecast.ArmyCost-payroll));
            LedgerLine(ref y,T("ui.economy.balance"),forecast.NetGold,true);y+=9;
            Paragraph(ref y,T("ui.economy.tax.reason"),small,238,19);
            Rule(4,y,238);y+=18;
            Paragraph(ref y,T("ui.economy.grain"),tiny,238,13);
            LedgerLine(ref y,T("ui.economy.production"),forecast.Production);
            LedgerLine(ref y,T("ui.economy.population"),-forecast.CivilianConsumption);
            LedgerLine(ref y,T("ui.economy.rations"),-forecast.ArmyConsumption);
            LedgerLine(ref y,T("ui.economy.subsidy"),-forecast.SubsidyConsumption);
            LedgerLine(ref y,T("ui.economy.balance"),forecast.NetFood,true);y+=9;
            Paragraph(ref y,T("ui.economy.food.reason"),small,238,18);
            Paragraph(ref y,T(state.SubsidyParis?"ui.economy.subsidy.active":"ui.economy.subsidy.inactive"),small,238,19);
            Rule(4,y,238);y+=18;
            Paragraph(ref y,T("ui.economy.unrest.title"),heading,238,12);
            float unrest=CampaignCore.AverageUnrest(state),after=CampaignCore.AverageUnrest(nextState);
            Paragraph(ref y,T("ui.economy.unrest.forecast",Number(unrest),Number(after),Change(after-unrest)),body,238,12);
            Paragraph(ref y,T("ui.economy.unrest.reason"),small,238,18);
            Paragraph(ref y,T("ui.economy.supply.detail"),small,238,18);
            if(Press(new Rect(4,y,238,41),T("ui.economy.paris")))app.SelectRegion("ile");
            y+=55;documentContentHeight=y;
        }

        private void Mandate(GameApp app)
        {
            CampaignState state=app.State;
            float y=0;
            Paragraph(ref y,MandatePresentation.RoleName(state.RoleId),heading,242,12);
            if(string.IsNullOrEmpty(state.RoleId)||state.RoleId=="legacy")
            {
                Paragraph(ref y,T("ui.mandate.legacy"),body,238,18);
                documentContentHeight=y+64;
                if(Press(new Rect(4,y,238,44),T("ui.mandate.new_role"),true,true))app.BeginRoleSelection();
                return;
            }
            bool active=state.Obligation!=null;
            MandateTerms terms=active?CampaignCore.GetObligationTerms(state):CampaignCore.GetMandateTerms(state,state.SelectedRegionId);
            if(terms==null)
            {
                Paragraph(ref y,T("ui.mandate.select_region"),body);documentContentHeight=y+12;return;
            }
            string patronId=MandatePresentation.PatronId(terms.Kind);
            var patron=state.Characters.Find(person=>person.Id==patronId);
            Seal(new Rect(2,y,82,96),MandatePresentation.PortraitIndex(terms.Kind));
            Text(new Rect(96,y+2,146,20),T("ui.mandate.patron"),tiny);
            float nameHeight=body.CalcHeight(new GUIContent(T("character."+patronId+".name")),146);
            Text(new Rect(96,y+26,146,nameHeight),T("character."+patronId+".name"),body);
            if(patron!=null)Text(new Rect(96,y+nameHeight+36,146,40),T("ui.mandate.patron_relation",Number(patron.Relationship)),small);
            y+=Mathf.Max(110,nameHeight+84);
            if(!active&&patron!=null&&patron.Relationship==0)
            {
                if(PatronTrustRepair(app,ref y)){documentContentHeight=y+12;return;}
            }
            else Paragraph(ref y,T((active?"ui.mandate.identity.":"ui.trust.identity.")+state.RoleId),small,238,18);
            Rule(4,y,238);y+=17;
            Paragraph(ref y,MandatePresentation.PrivilegeName(terms.Kind),heading,238,10);
            if(!active)Paragraph(ref y,T("ui.role_clarity.unsigned"),small,238,10);
            // Bu işaret yalnız belgenin koşullarına gider; hiçbir emir vermez.
            if(Press(new Rect(4,y,238,39),T(active?"ui.mandate.review_obligation":"ui.mandate.review_terms"),true,true))
                pendingMandateTerms=true;
            y+=49;
            Paragraph(ref y,T("ui.mandate.region",T("region."+terms.RegionId)),body,238,10);
            if(active)
            {
                var notice=new GUIStyle(small);notice.normal.textColor=red;
                Paragraph(ref y,T(CampaignCore.MandateDue(state)?"ui.mandate.active_due":"ui.mandate.active"),notice,238,10);
                Paragraph(ref y,T("ui.mandate.issued",MandatePresentation.Date(terms.IssuedWeek)),small,238,6);
            }
            Paragraph(ref y,T(active?"ui.mandate.due":"ui.role_clarity.proposed_due",MandatePresentation.Date(terms.DueWeek)),body,238,10);
            if(active)Paragraph(ref y,T("ui.mandate.original_region"),small,238,14);
            if(pendingMandateTerms){documentScroll.y=y;pendingMandateTerms=false;}
            MandateEffects(ref y,T(active?"ui.mandate.agreed_short":"ui.mandate.now"),terms.Immediate);
            MandateEffects(ref y,T("ui.mandate.choice.fulfil"),terms.Fulfil);
            MandateEffects(ref y,T("ui.mandate.choice.break"),terms.Break);
            Paragraph(ref y,T("ui.mandate.meter_limits"),tiny,238,12);
            if(!active)Paragraph(ref y,T("ui.mandate.rules",CampaignCore.MandateMinimumPower,
                CampaignCore.MandateDelayWeeks,CampaignCore.MandateCooldownWeeks),small,238,14);
            if(active)
            {
                string expectedId=CampaignCore.MandateId(state.Obligation);
                ActionResult fulfil=CampaignCore.CanResolveMandate(state,expectedId,"fulfil");
                ActionResult broken=CampaignCore.CanResolveMandate(state,expectedId,"break");
                Paragraph(ref y,T("ui.role_clarity.obligation_context",T("region."+terms.RegionId),
                    MandatePresentation.Date(terms.DueWeek)),body,238,10);
                Paragraph(ref y,T("ui.mandate.stocks",state.Gold,state.Food),small,238,12);
                if(!fulfil.Ok)Paragraph(ref y,L.Text(fulfil.Key,fulfil.Args),small,238,12);
                documentContentHeight=y+112;
                if(Press(new Rect(4,y,238,42),T("ui.mandate.fulfil_early"),fulfil.Ok,true))
                {app.ResolveMandate(expectedId,"fulfil");return;}
                y+=50;
                if(!broken.Ok)Paragraph(ref y,L.Text(broken.Key,broken.Args),small,238,12);
                if(Press(new Rect(4,y,238,42),T("ui.mandate.action.break"),broken.Ok))
                {app.ResolveMandate(expectedId,"break");return;}
                y+=54;
            }
            else
            {
                ActionResult issue=CampaignCore.CanIssueMandate(state,state.SelectedRegionId);
                if(!issue.Ok)
                {
                    var warning=new GUIStyle(small);warning.normal.textColor=red;
                    Paragraph(ref y,L.Text(issue.Key,issue.Args),warning,238,12);
                }
                if(state.NextMandateWeek>state.Week)
                    Paragraph(ref y,T("ui.mandate.available_date",MandatePresentation.Date(state.NextMandateWeek)),small,238,12);
                documentContentHeight=y+64;
                if(Press(new Rect(4,y,238,44),T("ui.mandate.issue"),issue.Ok,true))
                {app.IssueMandate();return;}
                y+=58;
            }
            documentContentHeight=y+12;
        }

        private bool PatronTrustRepair(GameApp app,ref float y)
        {
            var refusal=new GUIStyle(small);refusal.normal.textColor=red;
            Paragraph(ref y,T("ui.trust.refusal."+app.State.RoleId),refusal,238,12);
            PatronRepairTerms terms=CampaignCore.GetPatronRepairTerms(app.State);
            if(terms==null)return false;
            ActionResult repair=CampaignCore.CanRepairPatronTrust(app.State);
            string caption=T("ui.trust.repair.title");
            string effects=T("ui.trust.repair.effects",terms.PowerCost>0?Change(-terms.PowerCost):"0",Change(terms.RelationshipGain));
            string consequence=T("ui.trust.repair.consequence");
            string reason=repair.Ok?"":L.Text(repair.Key,repair.Args);
            const float width=220;
            float captionHeight=body.CalcHeight(new GUIContent(caption),width);
            float effectsHeight=body.CalcHeight(new GUIContent(effects),width);
            float consequenceHeight=small.CalcHeight(new GUIContent(consequence),width);
            float reasonHeight=repair.Ok?0:small.CalcHeight(new GUIContent(reason),width)+10;
            float height=12+captionHeight+10+effectsHeight+10+consequenceHeight+14+reasonHeight+44+12;
            Fill(new Rect(1,y,247,height),pale);Fill(new Rect(1,y,3,height),red);
            y+=12;
            Text(new Rect(14,y,width,captionHeight),caption,body);y+=captionHeight+10;
            Text(new Rect(14,y,width,effectsHeight),effects,body);y+=effectsHeight+10;
            Text(new Rect(14,y,width,consequenceHeight),consequence,small);y+=consequenceHeight+14;
            if(!repair.Ok){Text(new Rect(14,y,width,reasonHeight-10),reason,refusal);y+=reasonHeight;}
            bool accepted=Press(new Rect(14,y,width,44),T("ui.trust.repair.action"),repair.Ok,true);
            y+=68;
            if(!accepted)return false;
            app.RepairPatronTrust();documentScroll=Vector2.zero;
            return true;
        }

        private void MandateEffects(ref float y,string label,MandateEffect effect)
        {
            Rule(4,y,238);y+=10;
            Paragraph(ref y,label,body,238,7);
            Paragraph(ref y,MandatePresentation.Effects(effect),small,238,14);
        }
        private void StockProjection(ref float y,string label,int current,int projected,int net)
        {
            Fill(new Rect(1,y,247,100),pale);Fill(new Rect(1,y,3,100),brass);
            Text(new Rect(13,y+10,226,18),label,tiny);
            string values=Number(current)+"  →  "+Number(projected);
            var amount=new GUIStyle(heading);amount.font=sansFont;amount.fontSize=22;
            while(amount.fontSize>13&&amount.CalcSize(new GUIContent(values)).x>226)amount.fontSize--;
            Text(new Rect(13,y+33,226,28),values,amount);
            var caption=new GUIStyle(tiny);caption.normal.textColor=net<0?red:muted;
            Text(new Rect(13,y+69,226,26),T("ui.economy.projection",Signed(net)),caption);y+=114;
            long shortage=Math.Max(0L,-((long)current+net));
            if(shortage>0)
            {
                var warning=new GUIStyle(small);warning.normal.textColor=red;
                Paragraph(ref y,T("ui.economy.shortfall",Number(shortage)),warning,238,14);
            }
        }
        private void LedgerLine(ref float y,string label,int value,bool total=false)
        {
            GUIStyle labelStyle=total?body:small;
            float height=Mathf.Max(26,labelStyle.CalcHeight(new GUIContent(label),155));
            if(total){Rule(4,y,238);y+=8;}
            Text(new Rect(5,y,155,height),label,labelStyle);
            var number=new GUIStyle(body);number.alignment=TextAnchor.UpperRight;number.normal.textColor=value<0?red:ink;
            Text(new Rect(161,y,81,height),Signed(value),number);y+=height+9;
        }
        private float JournalEntryHeight(LogEntry entry){return 28+body.CalcHeight(new GUIContent(L.Text(entry.Key,entry.Args)),226)+29;}
        private static bool Important(LogEntry entry){return entry.Key.StartsWith("log.battle.",StringComparison.Ordinal)||entry.Key.StartsWith("log.petition.",StringComparison.Ordinal)||entry.Key.StartsWith("log.mandate.",StringComparison.Ordinal)||entry.Key.StartsWith("log.accord.",StringComparison.Ordinal)||entry.Key=="log.shortage"||entry.Key=="log.subsidy.failed";}
        private void Journal(GameApp app)
        {
            float y=0;Paragraph(ref y,T("ui.journal.title"),heading,242,19);bool first=true;
            foreach(var entry in app.State.Journal)
            {
                float height=JournalEntryHeight(entry);
                bool urgent=entry.Key=="log.shortage"||entry.Key=="log.battle.defeat"||entry.Key=="log.subsidy.failed"||entry.Key=="log.accord.broken"||
                    (entry.Key.StartsWith("log.mandate.",StringComparison.Ordinal)&&entry.Key.EndsWith(".break",StringComparison.Ordinal));
                if(first)
                {
                    float emphasis=Mathf.Clamp01(1f-(Time.unscaledTime-latestEntryTime)/1.4f);
                    Fill(new Rect(0,y-7,245,height-8),Color.Lerp(paper,C("#E2DEC5"),.25f+emphasis*.6f));
                    var stamp=new GUIStyle(tiny);stamp.alignment=TextAnchor.UpperRight;stamp.normal.textColor=red;
                    Text(new Rect(141,y,99,22),T("ui.journal.latest"),stamp);first=false;
                }
                if(Important(entry))Fill(new Rect(0,y-7,2,height-8),urgent?red:brass);
                Text(new Rect(9,y,226,22),Date(entry.Week),tiny);
                var entryStyle=new GUIStyle(body);if(urgent)entryStyle.normal.textColor=red;
                Text(new Rect(9,y+28,226,height-57),L.Text(entry.Key,entry.Args),entryStyle);
                y+=height;Rule(9,y-12,230);
            }
            documentContentHeight=y+10;
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
            Text(new Rect(476,328,488,54),T("ui.restart.title"),title);Text(new Rect(476,397,488,76),T("ui.role_clarity.restart_body"),body);
            if(Press(new Rect(476,511,231,43),T("ui.cancel")))confirmNew=false;
            if(Press(new Rect(722,511,241,43),T("ui.role_clarity.choose_role"),true,true)){confirmNew=false;app.BeginRoleSelection();}
        }
        private void OnDestroy(){foreach(var texture in medallions)if(texture)Destroy(texture);if(serifFont)Destroy(serifFont);if(sansFont)Destroy(sansFont);}
    }
}
