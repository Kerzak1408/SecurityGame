namespace Assets.Scripts.Model
{
    /// <summary>
    /// Representation of the navigation graph.
    /// </summary>
    public class AIModel
    {
        public TileNode[,] Tiles { get; set; }

        public AIModel(int width, int height)
        {
            Tiles = new TileNode[width, height];
        }

        public void Reset()
        {
            foreach (TileNode tileNode in Tiles)
            {
                tileNode.Reset();
            }
        }
    }
}
