using System;

namespace Assets.Scripts.Serialization
{
    [Serializable]
    public class CharacterData : BaseEntityData
    {
        private string[] itemNames;

        public string[] ItemNames
        {
            get { return itemNames ?? (itemNames = new string[5]); }
            set { itemNames = value; }
        }
    }
}
