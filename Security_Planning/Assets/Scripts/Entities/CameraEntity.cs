using System;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    public class CameraEntity : TransmitterEntity
    {

        public override void StartGame()
        {
            Debug.Log("camera related name = " + Data.relatedName);
            Vector3 position = transform.position;
            transform.position = new Vector3(position.x, position.y, -1.8f);
            Camera camera = GetComponentInChildren<Camera>();
            if (Data.relatedName == "None")
            {
                camera.enabled = false;
            }
            else
            {
                RenderTexture renderTexture = new RenderTexture(1600, 1000, 16, RenderTextureFormat.ARGB32);
                renderTexture.Create();
                camera.targetTexture = renderTexture;
                GameObject monitor = GameObject.Find(Data.relatedName);
                GameObject screen = monitor.transform.GetChild(0).gameObject;
                screen.GetComponent<Renderer>().material.mainTexture = renderTexture;
                screen.GetComponent<Renderer>().material.shader = Shader.Find("Unlit/Texture");
            }
        }

        public override Type GetReceiverType()
        {
            return typeof(ReceiverEntity);
        }
    }
}
