using System;

namespace IntralismScoreChecker
{
    /// <summary>
    ///     The class that represents the score of a player on a intralism map.
    /// </summary>
    public class MapScore
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MapScore"/> class.
        /// </summary>
        /// <param name="mapLink"> The link to the map. </param>
        /// <param name="mapName"> The name of the map. </param>
        /// <param name="maxPoints"> The maximum points that is achievable on the map. </param>
        /// <param name="brokenStatus"> The <see cref="BrokenType"/> of the map. </param>
        public MapScore(string mapLink, string mapName, double maxPoints, string brokenStatus)
        {
            this.MapLink = mapLink;
            this.MapId = long.Parse(mapLink[(mapLink.LastIndexOf("=", StringComparison.Ordinal) + 1) ..]!);
            this.MapName = mapName;
            this.MaximumPoints = maxPoints;
            this.BrokenStatus = Enum.Parse<BrokenType>(brokenStatus.Replace(" ", "_"));
        }

        /// <summary>
        ///     Gets or sets the name of the map.
        /// </summary>
        public string     MapName       { get; set; }

        /// <summary>
        ///     Gets or sets the link to the map.
        /// </summary>
        public string     MapLink       { get; set; }

        /// <summary>
        ///     Gets or sets the id of the map.
        /// </summary>
        public long       MapId         { get; set; }

        /// <summary>
        ///     Gets or sets the score that a user achieved on the map.
        /// </summary>
        public int        Score         { get; set; }

        /// <summary>
        ///     Gets or sets the accuracy that a user achieved on the map.
        /// </summary>
        public double     Accuracy      { get; set; }

        /// <summary>
        ///     Gets or sets the count of misses a user got on the map.
        /// </summary>
        public int        Miss          { get; set; }

        /// <summary>
        ///     Gets or sets the points that a user got on the map.
        /// </summary>
        public double     Points        { get; set; }

        /// <summary>
        ///     Gets or sets the maximum points of the map that's achievable.
        /// </summary>
        public double     MaximumPoints { get; set; }

        /// <summary>
        ///     Gets or sets the BrokenType of the map.
        /// </summary>
        public BrokenType BrokenStatus  { get; set; }

        /// <summary>
        ///     Gets or sets the difference between the maximum points and the points that a user achieved.
        /// </summary>
        public double     Difference    { get; set; }
    }
}