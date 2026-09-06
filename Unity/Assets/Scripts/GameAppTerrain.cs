using System.Collections.Generic;

namespace PowerAboveAll
{
    public sealed partial class GameApp
    {
        private void CreateWorldTerrain()
        {
            PopulateWorldTerrain(State.World,Map.WorldRivers);
            WorldCommand.Observe(State.World);
        }
        public static void PopulateWorldTerrain(WorldState world,GeoRiver[] rivers)
        {
            foreach(var site in world.Sites)
            {
                world.Terrain.Add(new WorldTerrainFeature{Id=site.Id+"-town",Kind=WorldTerrainKind.Town,Centre=site.Position,Radius=70,Source="Atlas settlement coordinates; PAA footprint",Confidence="Authored footprint, not a verified 1789 street plan"});
                // Aynı ağaç grupları yakın görsel ve görüş hesabında kullanılır; kadastro iddiası yok.
                for(int i=0;i<2;i++)world.Terrain.Add(new WorldTerrainFeature{Id=site.Id+"-shelterbelt-"+i,Kind=WorldTerrainKind.Woodland,Centre=site.Position+new WorldPoint(i==0?-610:680,i==0?240:-620),Radius=95,Source="PAA authored local shelterbelts",Confidence="Artistic local land cover; historical extent unverified"});
            }
            int part=0;
            foreach(var data in rivers)
            {
                // Nehir merkez çizgisi Natural Earth'tür; genişlik oynanış soyutlamasıdır.
                var points=new List<WorldPoint>();
                bool relevant=false;
                for(int i=0;i<data.points.Length;i+=2)
                {
                    if(data.points[i]>=-6&&data.points[i]<=10&&data.points[i+1]>=41&&data.points[i+1]<=52)relevant=true;
                    points.Add(WorldPoint.FromGeographic(data.points[i],data.points[i+1]));
                }
                if(!relevant||points.Count<2)continue;
                // Natural Earth ID nehrin kimliğidir; ayrı geometrik parçalar aynı ID'yi taşır.
                world.Terrain.Add(new WorldTerrainFeature{Id="river-"+data.id+"-part-"+part++,Kind=WorldTerrainKind.River,Points=points,Centre=points[0],Radius=15,Source="Natural Earth physical rivers, public domain",Confidence="Generalized centreline; width and crossings not a historical survey"});
            }
        }
    }
}
