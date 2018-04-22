using System;

namespace Assets.Scripts.Serialization
{
    [Serializable]
    public class TransmitterEntityData : BaseEntityData
    {
        private string relatedName = "None";

        public string RelatedName
        {
            get { return relatedName; }
            set { relatedName = value; }
        }
    }
}
