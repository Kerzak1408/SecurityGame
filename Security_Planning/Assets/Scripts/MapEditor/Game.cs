﻿using System.Collections.Generic;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Entities;
using Assets.Scripts.Extensions;
using Assets.Scripts.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.MapEditor
{
    public class Game : GridBase
    {
        public Text TextMoney;
        public Button ButtonExit;
        public Image CurrentItemIcon;

        public Map Map { get; private set; }

        // Use this for initialization
        protected override void Start ()
        {
            
            base.Start();
            string mapName = Scenes.GetParam("map");
            Map = LoadMap(mapName, mapVisible:true);

            GenerateCeiling(Map.Tiles, Map.EmptyParent.transform);

            foreach (GameObject entity in Map.Entities)
            {
                BaseEntity baseEntity = entity.GetComponent<BaseEntity>();
                baseEntity.CurrentGame = this;
                baseEntity.StartGame();
            }

            foreach (Transform transform in Map.EmptyParent.transform)
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

            foreach (GameObject tile in Map.Tiles)
            {
                if (tile.HasScriptOfType<BaseObject>())
                {
                    tile.GetComponent<BaseObject>().StartGame();
                }
            }
            Map.EmptyParent.transform.Rotate(90, 0, 0);
            //Map.EmptyParent.transform.eulerAngles = new Vector3(90, Map.EmptyParent.transform.eulerAngles.y, Map.EmptyParent.transform.eulerAngles.z);
        }

        // Update is called once per frame
        private void Update ()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                ButtonExit.gameObject.SetActive(!ButtonExit.IsActive());
            }
        }

        public void ExitToMenu()
        {
            Scenes.Load(Scenes.MAIN_MENU);
        }

        private void OnDrawGizmos()
        {
            
            if (Map == null) return;
            foreach (TileModel tileModel in Map.AIModel.Tiles)
            {
                Gizmos.color = Color.green;
                GameObject mapTile = Map.Tiles.Get(tileModel.Position);
                Gizmos.DrawSphere(mapTile.transform.position, 0.1f);
                foreach (Edge edge in tileModel.Neighbors)
                {
                    GameObject neighborTile = Map.Tiles.Get(edge.OtherIndices);
                    switch (edge.Type)
                    {
                        case EdgeType.NORMAL: Gizmos.color = Color.green;
                            break;
                        default: Gizmos.color = Color.red;
                            break;
                    }
                    
                    Gizmos.DrawLine(mapTile.transform.position, neighborTile.transform.position);
                }

            }
        }

    }
}
