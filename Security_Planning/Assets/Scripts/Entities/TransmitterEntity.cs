using System;

namespace Assets.Scripts.Entities
{
    public abstract class TransmitterEntity : BaseGenericEntity<TransmitterEntityData>
    {
        public abstract Type GetReceiverType();


    }
}
