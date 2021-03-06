﻿using System;
using UnityEngine;

namespace Assets.Scripts.Serialization
{
    [Serializable]
    public class BaseEntityData
    {
        public string name;
        public Vector3Wrapper position;
        public Vector3Wrapper eulerAngles;
        public string prefabName;

        public void ExtractValuesFromGameObject(GameObject gameObject)
        {
            name = gameObject.name;
            position = gameObject.transform.position;
            eulerAngles = gameObject.transform.eulerAngles;
        }
    }
}
