using NUnit.Framework;
using CityTraffics;

namespace CityTrafficsTests
{
    public class Tests
    {
        [TestCase(@"[""1:[5]"", ""2:[5,18]"", ""3:[5,12]"", ""4:[5]"", ""5:[1,2,3,4]"", ""18:[2]"", ""12:[3]""]", ExpectedResult = "1:44,2:25,3:30,4:41,5:20,12:33,18:27")]
        [TestCase(@"[""1:[5]"", ""2:[5]"", ""3:[5]"", ""4:[5]"", ""5:[1,2,3,4]""]", ExpectedResult = "1:14,2:13,3:12,4:11,5:4")]
        [TestCase(@"[""2:[1]"", ""1:[2]""]", ExpectedResult = "1:2,2:1")]
        [TestCase(@"[""1:[5]"", ""4:[5]"", ""3:[5]"", ""5:[1,4,3,2]"", ""2:[5,15,7]"", ""7:[2,8]"", ""8:[7,38]"", ""15:[2]"", ""38:[8]""]",
            ExpectedResult = "1:82,2:53,3:80,4:79,5:70,7:46,8:38,15:68,38:45")]
        // [TestCase(@"[""2:[1]"", ""1:[2]"", ""3:[4]"", ""4:[3]""]", ExpectedResult = "1:2,2:1,3:4,4:3")]
        public string Test1(string input)
        {
            var result = Program.GetCitiesTraffics(input);
            return result;
        }


    }
}