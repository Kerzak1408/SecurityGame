using System;
using Assets.Scripts.Serialization;

namespace Assets.Scripts.Entities
{
    public abstract class TransmitterEntity : BaseGenericEntity<TransmitterEntityData>
    {
        public abstract Type GetReceiverType();


    }
}
