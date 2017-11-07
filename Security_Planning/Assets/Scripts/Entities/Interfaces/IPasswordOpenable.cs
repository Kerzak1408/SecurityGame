using Assets.Scripts.Entities.Characters;

namespace Assets.Scripts.Entities.Interfaces
{
    public interface IPasswordOpenable
    {
        void EnterPassword(string password, BaseCharacter character);
    }
}
