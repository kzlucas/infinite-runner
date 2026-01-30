
namespace Assets.Components.SquareColliders.Scripts
{
    /// <summary>
    ///  Helper class to store point information
    /// </summary>
    public class PointInfo
    {
        public int pointIndex;
        public float angle;
        public float distance;

        public PointInfo(int pointIndex, float angle, float distance)
        {
            this.pointIndex = pointIndex;
            this.angle = angle;
            this.distance = distance;
        }
    }
}
