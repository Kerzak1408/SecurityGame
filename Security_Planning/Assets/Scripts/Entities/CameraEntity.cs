using System;
using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Entities.Interfaces;
using Assets.Scripts.Model;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    public class CameraEntity : DetectorEntity
    {
        public override PlanningEdgeType PlanningEdgeType
        {
            get { return PlanningEdgeType.CAMERA; }
        }

        public override void StartGame()
        {
            //Debug.Log("camera related name = " + Data.relatedName);
            Vector3 position = transform.position;
            transform.position = new Vector3(position.x, position.y, -3f);
            GameObject monitor = GameObject.Find(Data.relatedName);
            if (monitor == null)
            {   
                Camera.enabled = false;
            }
            else
            {
                RenderTexture renderTexture = new RenderTexture(1600, 1000, 16, RenderTextureFormat.ARGB32);
                renderTexture.Create();
                Camera.targetTexture = renderTexture;
                
                GameObject screen = monitor.transform.GetChild(0).gameObject;
                screen.GetComponent<Renderer>().material.mainTexture = renderTexture;
                screen.GetComponent<Renderer>().material.shader = Shader.Find("Unlit/Texture");
            }
        }

        public override Type GetReceiverType()
        {
            return typeof(MonitorEntity);
        }

    }
}
