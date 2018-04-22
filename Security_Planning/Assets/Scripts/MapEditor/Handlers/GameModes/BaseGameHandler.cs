using Assets.Scripts.Entities.Characters;

namespace Assets.Scripts.MapEditor.Handlers.GameModes
{
    public abstract class BaseGameHandler : BaseHandler
    {
        protected Game Game;
        protected Burglar Burglar;

        public virtual void Start(Game game)
        {
            Game = game;
            Burglar = Game.Map.Burglar;
        }

        public virtual void Update() { }

        public virtual void GoalsCompleted(BaseCharacter baseCharacter)
        {
        }
    }
}
