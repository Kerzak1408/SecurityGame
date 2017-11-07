using Assets.Scripts.Entities.Interfaces;

namespace Assets.Scripts.Entities.Characters
{
    public abstract class BaseCharacter : BaseGenericEntity<BaseEntityData> {

        public abstract void RequestPassword(IPasswordOpenable passwordOpenableObject);
        public abstract void InterruptRequestPassword();
    }
}
