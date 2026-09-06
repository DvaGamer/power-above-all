using System.Collections.Generic;
using UnityEngine;

namespace PowerAboveAll
{
    public sealed partial class WorldMapEntities
    {
        private WorldState supplyObserved;
        private readonly Dictionary<string,Transform> supplyViews=new Dictionary<string,Transform>();
        private Transform SupplyShape(string id,bool depot)
        {
            if(supplyViews.TryGetValue(id,out var root))return root;
            root=new GameObject(id+" · physical supply").transform;root.SetParent(transform,false);supplyViews.Add(id,root);
            if(depot)
            {
                var walls=Shape("Granary",root,new Vector3(1,.6f,.7f),paper);walls.localPosition=Vector3.up*.3f;
                var roof=Shape("Sloped slate",root,new Vector3(1.1f,.15f,.9f),blue);roof.localPosition=Vector3.up*.68f;roof.localRotation=Quaternion.Euler(0,0,-7);
                var door=Shape("Loading door",root,new Vector3(.3f,.4f,.04f),ink);door.localPosition=new Vector3(0,.2f,.36f);
            }
            else
            {
                var bed=Shape("Cart bed",root,new Vector3(1,.35f,.65f),ink);bed.localPosition=Vector3.up*.3f;
                var load=Shape("Folded canvas",root,new Vector3(.75f,.36f,.6f),paper);load.localPosition=new Vector3(-.07f,.62f,0);load.localRotation=Quaternion.Euler(0,0,-5);
                for(int x=-1;x<=1;x+=2)for(int z=-1;z<=1;z+=2)
                {var wheel=Shape("Wheel",root,new Vector3(.24f,.28f,.07f),gold);wheel.localPosition=new Vector3(x*.32f,.16f,z*.36f);}
            }
            return root;
        }
        private void UpdateSupplyViews(float distance)
        {
            if(supplyObserved!=observed)
            {foreach(var root in supplyViews.Values){root.gameObject.SetActive(false);Destroy(root.gameObject);}supplyViews.Clear();supplyObserved=observed;}
            var player=observed.Army(observed.PlayerArmyId);
            foreach(var depot in observed.Depots)
            {
                var root=SupplyShape(depot.Id,true);var point=observed.Sites.Find(s=>s.Id==depot.SiteId).Position;
                root.gameObject.SetActive(distance<350&&(depot.FactionId==player.FactionId||WorldTerrain.Visible(observed,player,point)));
                root.position=Position(point)+Vector3.right*Mathf.Clamp(distance*.002f,.002f,.2f);
                root.localScale=Vector3.one*Mathf.Clamp(distance/75,25*Metre,.35f);
            }
            foreach(var convoy in observed.Convoys)
            {
                var root=SupplyShape(convoy.Id,false);
                bool seen=convoy.FactionId==player.FactionId||WorldTerrain.Visible(observed,player,convoy.Position);
                root.gameObject.SetActive(distance<350&&seen&&WorldSupply.Active(convoy));root.position=Position(convoy.Position);
                root.localScale=Vector3.one*Mathf.Clamp(distance/65,18*Metre,.23f);
                if(convoy.Route.Segment+1<convoy.Route.Points.Count)
                    root.rotation=Quaternion.Euler(0,(float)WorldCombat.Heading(convoy.Route.Points[convoy.Route.Segment+1]-convoy.Position)+90,0);
            }
        }
    }
}
