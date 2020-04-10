using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace NUnitTest
{
    //TODO: Unit тесты под симплекс метод
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            const string patternVector = @"(\[((\d+,)+\d+\]))"; 

            string input = "[24,5,8000,9000,8]    [80,8000]";

            Regex regex =new Regex(patternVector);
            MatchCollection match = regex.Matches(input);
            foreach (Match match1 in match)
            {
                Console.WriteLine("Вот строка(Вектор) " + match1.Value);
            }

            string patternMatrix = $@"(\[({patternVector},)+{patternVector}\])|(\[{patternVector}\])";

            string inputMatrix = "[[24,5,8],[789,458,988],[24,5,8]]    [[105500,5888,1000555]]";

            Regex regexMatrix = new Regex(patternMatrix);
            MatchCollection matchMatrix = regexMatrix.Matches(inputMatrix);
            foreach (Match match1 in matchMatrix)
            {
                Console.WriteLine("Вот строка(Матрица) " + match1.Value);
            }

            Console.WriteLine("Матрица\nГруппа 0");
            foreach (Group gGroup in matchMatrix[0].Groups)
            {
                Console.WriteLine(gGroup.Value);
            }

            Console.WriteLine("Группа 1");
            foreach (Group gGroup in matchMatrix[1].Groups)
            {
                Console.WriteLine(gGroup.Value);
            }

            Console.WriteLine("Векторы\nГруппа 0");
            foreach (Group gGroup in match[0].Groups)
            {
                Console.WriteLine(gGroup.Value);
            }

            Console.WriteLine("Группа 1");
            foreach (Group gGroup in match[1].Groups)
            {
                Console.WriteLine(gGroup.Value);
            }
        }
    }
}