


namespace WorldGenerator.Scripts
{

    /// <summary>
    ///   Strategy for selecting world segments based on biome and generation index.
    /// </summary>
    public class SelectionStrategy
    {
        private SO_BiomeData _biomeData;


        public SelectionStrategy(SO_BiomeData biomeData)
        {
            _biomeData = biomeData;
        }

        public WorldSegment Select(int generatedIndex)
        {
            switch (BiomesDataManager.Instance.current.BiomeName)
            {
                case "Tutorial":

                    if (generatedIndex <= 3)
                        return _biomeData.Segments.Find(s => s.name == "Straight Segment");

                    else if (generatedIndex <= 10)
                        return _biomeData.Segments.Find(s => s.name == "Change Lane");

                    else if (generatedIndex <= 12)
                        return _biomeData.Segments.Find(s => s.name == "Straight Segment");

                    else if (generatedIndex <= 16)
                        return _biomeData.Segments.Find(s => s.name == "Jump");

                    else if (generatedIndex <= 18)
                        return _biomeData.Segments.Find(s => s.name == "Straight Segment");

                    else if (generatedIndex <= 22)
                        return _biomeData.Segments.Find(s => s.name == "Slide");

                    else if (generatedIndex <= 24)
                        return _biomeData.Segments.Find(s => s.name == "Straight Segment");

                    else
                    {
                        int randIndex = UnityEngine.Random.Range(0, _biomeData.Segments.Count);
                        return _biomeData.Segments[randIndex];
                    }


                case "World 1 - Gray":
                case "World 2 - Red":
                case "World 3 - Green":


                    if (generatedIndex <= 3)
                        return _biomeData.Segments.Find(s => s.name == "Straight Segment");

                    else
                        return _biomeData.Segments.FindAll(s => s.name != "Straight Segment")
                                                  .PickRandom();

                default:

                    if (generatedIndex <= 3)
                        return _biomeData.Segments.Find(s => s.name == "Straight Segment");

                    else
                        return _biomeData.Segments.Find(s => s.name == "Random Obstacles");

            }

        }
    }

}