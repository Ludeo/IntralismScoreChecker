using System.IO;
using System.Linq;

namespace IntralismScoreChecker
{
    /// <summary>
    ///     Class that is used for reading a csv file.
    /// </summary>
    public static class CsvReader
    {
        /// <summary>
        ///     Reads a csv file and returns it's content.
        /// </summary>
        /// <param name="path"> The path where the csv file is saved. </param>
        /// <returns> Returns the content of the csv file in a 2d string array. </returns>
        public static string[][] GetCsvContent(string path)
        {
            if (File.Exists(path) && !string.IsNullOrWhiteSpace(File.ReadAllText(path!)))
            {
                return File.ReadLines(path).Select(line => line.Split(",")).ToArray();
            }

            return System.Array.Empty<string[]>();
        }
    }
}