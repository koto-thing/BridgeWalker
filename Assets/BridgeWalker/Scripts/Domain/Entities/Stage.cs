namespace BridgeWalker.Scripts.Domain.Entities
{
    public class Stage
    {
        public float Width { get; private set; }
        public float Height { get; private set; }
        
        public Stage(float width, float height)
        {
            Width = width;
            Height = height;
        }
    }
}