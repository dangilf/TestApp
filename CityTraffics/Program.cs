using Newtonsoft.Json;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("CityTrafficsTests")]
namespace CityTraffics
{    
    internal class Program
    {
        static void Main(string[] args)
        {
            
            //var strArr = "[\"1:[5]\", \"2:[5]\", \"3:[5]\", \"4:[5]\", \"5:[1,2,3,4]\"]";            
            var strArr = @"[""1:[5]"", ""2:[5,18]"", ""3:[5,12]"", ""4:[5]"", ""5:[1,2,3,4]"", ""18:[2]"", ""12:[3]""]";          

            Console.WriteLine(GetCitiesTraffics(strArr));
        }

        /// <summary>
        /// Gets the highest city traffic based on input
        /// </summary>
        /// <param name="input">List of cities with their neighbors</param>
        /// <returns>Sorted list of cities with highest traffic</returns>
        public static string GetCitiesTraffics(string input)
        {
            string[] nodeStr = JsonConvert.DeserializeObject<string[]>(input);

            var country = new Dictionary<int, HashSet<int>>();
            var updatedPopulation = new Dictionary<int, int>();
            var roads = new HashSet<(int, int)>();
            var allPopulation = 0;

#region Parsing O(N)
            foreach (string str in nodeStr)
            {
                var splitted = str.Split(':');
                var source = Convert.ToInt32(splitted[0]);
                allPopulation += source;
                var destinations = JsonConvert.DeserializeObject<HashSet<int>>(splitted[1]);
                country.Add(source, destinations);
                foreach (var dest in destinations)
                {
                    roads.Add(ToRoad(source, dest));
                }

                updatedPopulation.Add(source, source);
            }
            #endregion

#region find terminal city O(N)

            var terminals = new HashSet<int>();
            foreach (var city in country)
            {
                if (city.Value.Count <= 1)
                {
                    terminals.Add(city.Key);
                }
            }
#endregion
            var markedCities = new HashSet<int>();
            var populationOfRoads = new Dictionary<(int, int), (int, int)>();

#region Cut of terminal nodes O(N) for all steps
            while (markedCities.Count() < country.Count)
            {
                var newTerminals = new HashSet<int>();
                foreach (var terminal in terminals)
                {
                    var populationOfSource = updatedPopulation[terminal];
                    markedCities.Add(terminal);
                    var destinations = country[terminal].Where(c => !markedCities.Contains(c));
                    if (!destinations.Any())
                    {
                        break;
                    }

                    var destination = destinations.Single();

                    // 
                    var road = ToRoad(terminal, destination);
                    var population = ToRoadWithPopulation(terminal, destination, populationOfSource, allPopulation - populationOfSource);
                    populationOfRoads.Add(road, population);
                    updatedPopulation[destination] += populationOfSource;

                    if (country[destination].Count(c => !markedCities.Contains(c)) == 1)
                    {
                        newTerminals.Add(destination);
                    }
                }

                terminals = newTerminals;
            }
#endregion 
            var resultDict = new Dictionary<int, int>();

#region Calculate max traffic based on traffic population O(N)

            foreach (var city in country)
            {
                var source = city.Key;
                var maxPopulation = 0;
                foreach (var destination in city.Value)
                {
                    var (left, right) = populationOfRoads[ToRoad(source, destination)];
                    var populationOutside = source < destination ? right : left;
                    maxPopulation = Math.Max(maxPopulation, populationOutside);
                }

                resultDict[source] = maxPopulation;
            }
#endregion

#region Sorting O(NlogN)
            return string.Join(",", resultDict.OrderBy(r=>r.Key).Select(r => $"{r.Key}:{r.Value}"));
#endregion
        }

        /// <summary>
        /// Get the road with cities at both ends. Puts city with highest population to the left
        /// </summary>
        /// <param name="a">First city</param>
        /// <param name="b">Second city</param>
        /// <returns>Tuple of 2 cities</returns>
        private static (int, int) ToRoad(int a, int b) => a < b ? (a, b) : (b, a);

        /// <summary>
        /// Get the popultation from each side of the road. Puts city with highest population to the left
        /// </summary>
        /// <param name="a">First city</param>
        /// <param name="b">Second city</param>
        /// <param name="pa">First city side population</param>
        /// <param name="pb">Second city side population</param>
        /// <returns>Tuple of 2 populations</returns>
        private static (int, int) ToRoadWithPopulation(int a, int b, int pa, int pb) => a < b ? (pa, pb) : (pb, pa);
    }
}
