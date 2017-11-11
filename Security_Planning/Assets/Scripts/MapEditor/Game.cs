using System.Linq;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Interfaces;
using Assets.Scripts.Extensions;
using UnityEngine;

namespace Assets.Scripts.MapEditor
{
    public class Game : GridBase
    {
        private Map map;

        // Use this for initialization
        protected override void Start ()
        {
            
            base.Start();
            string mapName = Scenes.GetParam("map");
            map = LoadMap(mapName, mapVisible:true);
            foreach (GameObject entity in map.Entities)
            {
                entity.GetComponent<BaseEntity>().StartGame();
            }

            foreach (Transform transform in map.EmptyParent.transform)
            {
                if (transform.name != "CCTV")
                {
                    continue;
                }
                Camera cctv =  transform.gameObject.GetComponentInChildren<Camera>();
                if (cctv != null)
                {
                    RenderTexture newRenderTexture = new RenderTexture(256, 256, 16);
                    newRenderTexture.Create();
                    cctv.targetTexture = newRenderTexture;
                    GameObject.Find("Monitor").GetComponent<Renderer>().material.mainTexture = newRenderTexture;
                }
            }
        }
	
        // Update is called once per frame
        private void Update ()
        {

        }


    }
}
