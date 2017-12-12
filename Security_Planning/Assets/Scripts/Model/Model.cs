namespace Assets.Scripts.Model
{
    public class AIModel
    {
        public TileModel[,] Tiles;

        public AIModel(int width, int height)
        {
            Tiles = new TileModel[width, height];
        }
    }
}
