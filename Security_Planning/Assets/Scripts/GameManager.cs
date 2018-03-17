namespace Assets.Scripts
{
    public class GameManager
    {
        private static GameManager instance;
        public static GameManager Instance
        {
            get { return instance ?? (instance = new GameManager()); }
        }

        private GameManager()
        {

        }

        public string PathToGridFile { get; set; }
    }
}
