namespace Assets.Scripts.Model
{
    public class AIModel
    {
        public TileNode[,] Tiles;

        public AIModel(int width, int height)
        {
            Tiles = new TileNode[width, height];
        }
    }
}
