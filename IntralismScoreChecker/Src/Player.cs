using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace IntralismScoreChecker
{
    /// <summary>
    ///     Class that represents a intralism player.
    /// </summary>
    public class Player
    {
        private const string ScoresCsvPath = "scores.csv";

        /// <summary>
        ///     Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        public Player()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="profileLink"> Link to the profile of the player or the search input. </param>
        /// <param name="withLink"> Boolean that indicates if profileLink is the actual profile link or if you want to search for a player. </param>
        public Player(string profileLink, bool withLink)
        {
            if (withLink == false)
            {
                profileLink = SearchForPlayer(profileLink);
            }

            this.Link = profileLink;
            this.Id = long.Parse(profileLink[(profileLink.LastIndexOf("=", StringComparison.Ordinal) + 1) ..]!);
            this.FillScoresList();
            this.ReadHtmlCode();
            this.RecalculateScores();
            this.CalculateRankUpPoints();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="rank"> Global Rank of the player. </param>
        public Player(int rank)
        {
            string profileLink = GetPlayerLink(rank);
            this.Link = profileLink;
            this.Id = long.Parse(profileLink[(profileLink.LastIndexOf("=", StringComparison.Ordinal) + 1) ..]!);
            this.FillScoresList();
            this.ReadHtmlCode();
            this.RecalculateScores();
            this.CalculateRankUpPoints();
        }

        /// <summary>
        ///     Gets or sets the link of the player.
        /// </summary>
        [JsonProperty("link")]
        public string         Link             { get; set; }

        /// <summary>
        ///     Gets or sets the id of the player.
        /// </summary>
        [JsonProperty("id")]
        public long           Id               { get; set; }

        /// <summary>
        ///     Gets or sets the name of the player.
        /// </summary>
        [JsonProperty("name")]
        public string         Name             { get; set; }

        /// <summary>
        ///     Gets or sets the link to the profile picture of the player.
        /// </summary>
        [JsonProperty("picturelink")]
        public string         PictureLink      { get; set; }

        /// <summary>
        ///     Gets or sets the List of <see cref="MapScore"/> that the user played.
        /// </summary>
        [JsonProperty("scores")]
        public List<MapScore> Scores           { get; set; } = new List<MapScore>();

        /// <summary>
        ///     Gets or sets the global rank of the player.
        /// </summary>
        [JsonProperty("globalrank")]
        public int            GlobalRank       { get; set; }

        /// <summary>
        ///     Gets or sets the total amount of players that have a global rank.
        /// </summary>
        [JsonProperty("totalglobalrank")]
        public int            TotalGlobalRank  { get; set; }

        /// <summary>
        ///     Gets or sets the country rank of the player.
        /// </summary>
        [JsonProperty("countryrank")]
        public int            CountryRank      { get; set; }

        /// <summary>
        ///     Gets or sets the total amount of players that have a country rank in the same country as the player.
        /// </summary>
        [JsonProperty("totalcountryrank")]
        public int            TotalCountryRank { get; set; }

        /// <summary>
        ///     Gets or sets the country of the player.
        /// </summary>
        [JsonProperty("country")]
        public string         Country          { get; set; }

        /// <summary>
        ///     Gets or sets the average miss count of the player.
        /// </summary>
        [JsonProperty("averagemisses")]
        public double         AverageMisses    { get; set; }

        /// <summary>
        ///     Gets or sets the average accuracy of the player.
        /// </summary>
        [JsonProperty("averageaccuracy")]
        public double         AverageAccuracy  { get; set; }

        /// <summary>
        ///     Gets or sets the total amount of points that the player got.
        /// </summary>
        [JsonProperty("points")]
        public double         Points           { get; set; }

        /// <summary>
        ///     Gets or sets the real amount of points that the player would get, if maps wouldn't have <see cref="BrokenType"/> Broken.
        /// </summary>
        [JsonProperty("realpoints")]
        public double         RealPoints       { get; set; }

        /// <summary>
        ///     Gets or sets the total amount of points that are achievable.
        /// </summary>
        [JsonProperty("maximumpoints")]
        public double         MaximumPoints    { get; set; }

        /// <summary>
        ///     Gets or sets the difference between the points and the maximum points of the player.
        /// </summary>
        [JsonProperty("difference")]
        public double         Difference       { get; set; }

        /// <summary>
        ///     Gets or sets the total amount of 100% accuracy plays of the player.
        /// </summary>
        [JsonProperty("hundredplays")]
        public int            HundredPlays     { get; set; }

        /// <summary>
        ///     Gets or sets the total amount of ranked maps.
        /// </summary>
        [JsonProperty("totalmaps")]
        public int            TotalMaps        { get; set; }

        /// <summary>
        ///     Gets or sets the amount of points that the player needs to rank up in global ranks.
        /// </summary>
        [JsonProperty("rankuppoints")]
        public double         RankUpPoints     { get; set; }

        /// <summary>
        ///     Gets or sets the time when the player was checked.
        /// </summary>
        [JsonProperty("timechecked")]
        public DateTime TimeChecked { get; set; } = DateTime.Now;

        private static string SearchForPlayer(string searchInput)
        {
            string url = "https://intralism.khb-soft.ru/?page=ranks&search=" + searchInput;
            HtmlWeb web = new ();
            HtmlDocument doc = web.Load(url);

            HtmlNode table = doc.DocumentNode.SelectSingleNode("/html/body/main/div[3]/table");
            HtmlNode row = table.SelectSingleNode("tbody").SelectNodes("tr")[0];
            string profile = row.Attributes[2].Value.TrimStart('.');
            return "https://intralism.khb-soft.ru/" + profile;
        }

        private static string GetPlayerLink(int rank)
        {
            int siteNumber;
            int index;

            if (rank % 100 == 0)
            {
                siteNumber = rank / 100;
                index = 99;
            }
            else
            {
                index = (rank % 100) - 1;
                siteNumber = (rank / 100) + 1;
            }

            string url = "https://intralism.khb-soft.ru/?page=ranks&n=" + siteNumber;
            HtmlWeb web = new ();
            HtmlDocument doc = web.Load(url);

            HtmlNode table = doc.DocumentNode.SelectSingleNode("/html/body/main/div[3]/table");
            HtmlNode row = table.SelectSingleNode("tbody").SelectNodes("tr")[index];
            string profile = row.Attributes[2].Value.TrimStart('.');
            return "https://intralism.khb-soft.ru/" + profile;
        }

        private void ReadHtmlCode()
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(this.Link);

            HtmlNode usernameNode = doc.DocumentNode.SelectSingleNode("/html/body/main/div[1]/h1/span");

            HtmlNode countryNode = doc.DocumentNode.SelectSingleNode("/html/body/main/div[1]/p/span");

            HtmlNode pictureNode = doc.DocumentNode.SelectSingleNode("/html/body/main/div[1]/img");

            List<HtmlNode> totalGlobalRankNode = doc.DocumentNode.SelectNodes("/html/body/main/div[2]/div[2]").Nodes()?.ToList();

            string user = usernameNode.InnerText;
            this.Name = user.Substring(0, user.LastIndexOf("#", StringComparison.Ordinal));

            if (!int.TryParse(user[(user.LastIndexOf("#", StringComparison.Ordinal) + 1) ..], out int tempGlobalRank))
            {
                tempGlobalRank = -1;
            }

            this.GlobalRank = tempGlobalRank;

            this.Country = countryNode.InnerText;

            this.TotalGlobalRank = int.Parse(totalGlobalRankNode[4].InnerText.Replace(" / ", string.Empty));

            if (!int.TryParse(totalGlobalRankNode[9].InnerText, out int tempCountryRank))
            {
                tempCountryRank = -1;
            }

            this.CountryRank = tempCountryRank;

            this.TotalCountryRank = int.Parse(totalGlobalRankNode[10].InnerText.Replace(" / ", string.Empty));
            this.PictureLink = pictureNode.Attributes[1].Value;

            this.TotalMaps = this.Scores.Count;

            HtmlNode table = doc.DocumentNode.SelectSingleNode("/html/body/main/div[3]/table");

            if (table.SelectSingleNode("thead").SelectSingleNode("tr").SelectSingleNode("th[2]").InnerText.Contains("Until"))
            {
                table = doc.DocumentNode.SelectSingleNode("/html/body/main/div[4]/table");
            }

            try
            {
                int testValue = table.SelectSingleNode("tbody").SelectNodes("tr").Count;
            }
            catch
            {
                return;
            }

            foreach (HtmlNode row in table.SelectSingleNode("tbody").SelectNodes("tr"))
            {
                List<HtmlNode> cells = row.SelectNodes("th|td")?.ToList();

                string mapLink = cells[1].FirstChild.Attributes[0].Value;
                string mods = "Normal";

                if (cells[1].ChildNodes.Count == 3)
                {
                    mods = cells[1].ChildNodes[2].InnerHtml;
                }

                string score = cells[2].InnerText.Replace(" ", string.Empty);
                string accuracy = cells[3].InnerText.Replace("%", string.Empty);
                string miss = cells[4].InnerText;
                string points = cells[5].InnerText;

                if (mods.Equals("Random") ||
                    mods.Equals("Hidden") ||
                    mods.Equals("Relax") ||
                    mods.Equals("Endless"))
                {
                    continue;
                }

                if (this.Scores.All(x => x.MapLink != mapLink))
                {
                    continue;
                }

                MapScore map = this.Scores.First(x => x.MapLink == mapLink);
                map.Score = int.Parse(score);
                map.Accuracy = double.Parse(accuracy);
                map.Miss = int.Parse(miss!);
                map.Points = double.Parse(points!);
            }
        }

        private void FillScoresList()
        {
            string[][] scores = CsvReader.GetCsvContent(ScoresCsvPath);

            foreach (string[] score in scores)
            {
                this.Scores.Add(new MapScore(
                                    "https://steamcommunity.com/sharedfiles/filedetails/?id=" + score[3],
                                    score[0],
                                    double.Parse(score[1]!),
                                    score[2]));
            }
        }

        private void CalculateRankUpPoints()
        {
            if (this.GlobalRank == -1)
            {
                this.RankUpPoints = 0.01;
                return;
            }

            int n = ((this.GlobalRank - 1) / 100) + 1;

            if (this.GlobalRank % 100 == 1)
            {
                n -= 1;
            }

            string url = "https://intralism.khb-soft.ru/?page=ranks&n=" + n;
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//tr");
            HtmlNode before = nodes[0];
            HtmlNode current = nodes[0];

            if (this.GlobalRank % 100 == 1)
            {
                before = nodes[^1];
            }
            else
            {
                foreach (HtmlNode e in nodes)
                {
                    if (e.Id.Contains(this.Id.ToString()))
                    {
                        current = e;

                        break;
                    }

                    before = e;
                }
            }

            double currentPoints;

            if (this.GlobalRank % 100 == 1)
            {
                before = nodes[^1];
                currentPoints = this.Points;
            }
            else
            {
                currentPoints = double.Parse(current.SelectNodes("td")[3].InnerText.Replace(" ", string.Empty));
            }

            double beforePoints = double.Parse(before.SelectNodes("td")[3].InnerText.Replace(" ", string.Empty));

            if (this.GlobalRank == 1)
            {
                this.RankUpPoints = 0;
                return;
            }

            this.RankUpPoints = Math.Round((beforePoints - currentPoints) * 10000) / 10000;
        }

        private void RecalculateScores()
        {
            double totalAcc = 0.0;
            double totalDifference = 0.0;
            double realPoints = 0.0;
            double rankedPoints = 0.0;
            double maximumPoints = 0.0;
            int totalMiss = 0;
            int hundredCount = 0;
            int mapCount = this.Scores.Count;
            int notPlayed = 0;

            foreach (MapScore score in this.Scores)
            {
                double points = score.Points;
                double maxPoints = score.MaximumPoints;
                double actPoints = points;
                realPoints += points;
                maximumPoints += maxPoints;

                if (score.BrokenStatus == BrokenType.Broken)
                {
                    if (points == maxPoints)
                    {
                        actPoints = points - 0.01;
                        score.Points = Math.Round(actPoints * 100) / 100;
                    }

                    rankedPoints += actPoints;
                }
                else
                {
                    rankedPoints += points;
                }

                totalAcc += score.Accuracy;
                totalMiss += score.Miss;

                double a = score.MaximumPoints;
                double b = score.Points;
                score.Difference = Math.Round((a - b) * 100) / 100;
                totalDifference += score.Difference;

                if (score.Accuracy == 100)
                {
                    hundredCount++;
                }

                if (score.Score == 0)
                {
                    notPlayed++;
                }
            }

            double avgAccExact = totalAcc / (mapCount - notPlayed);
            double avgAcc = Math.Round(avgAccExact * 10000) / 10000;

            double avgMiss = (double)totalMiss / (mapCount - notPlayed);
            avgMiss = Math.Round(avgMiss * 100) / 100;

            totalDifference = Math.Round(totalDifference * 100) / 100;

            realPoints = Math.Round(realPoints * 100) / 100;
            rankedPoints = Math.Round(rankedPoints * 100) / 100;
            maximumPoints = Math.Round(maximumPoints * 100) / 100;

            this.AverageMisses = avgMiss;
            this.AverageAccuracy = avgAcc;
            this.Points = rankedPoints;
            this.MaximumPoints = maximumPoints;
            this.Difference = totalDifference;
            this.HundredPlays = hundredCount;
            this.RealPoints = realPoints;
        }
    }
}