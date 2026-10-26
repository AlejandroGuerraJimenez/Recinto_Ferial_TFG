namespace Fairground.Model
{
    /// <summary>
    /// Higher class rank wins. Equal class rank uses surface area.
    /// </summary>
    public readonly struct PlaneRank
    {
        public PlaneRank(int classRank, float area)
        {
            ClassRank = classRank;
            Area = area;
        }

        public int ClassRank { get; }

        public float Area { get; }

        public int CompareTo(PlaneRank other)
        {
            var byClass = ClassRank.CompareTo(other.ClassRank);
            return byClass != 0 ? byClass : Area.CompareTo(other.Area);
        }
    }
}
