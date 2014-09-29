using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Trains;

namespace TrainTests
{
    [TestClass]
    public class TrainTests
    {
        [TestMethod]
        public void RoutePlanner_ParseArgs()
        {
            string testFileDir = @"C:\Users\Public\TestFolder\";
            System.IO.Directory.CreateDirectory(testFileDir);
            string fileName = System.IO.Path.GetRandomFileName();
            string pathString = System.IO.Path.Combine(testFileDir, fileName);

            string sampleGraph = "AB3, AE2, BC4, BE9, CD8, DC8, DA5, ED3";
            System.IO.File.WriteAllText(pathString, sampleGraph);
            string[] testArgs = { pathString };
            string[] results = RoutePlanner.parseArgs(testArgs);
            Assert.IsNotNull(results);
            Assert.AreEqual(8, results.Length);

            sampleGraph = "CD5";
            System.IO.File.WriteAllText(pathString, sampleGraph);
            results = RoutePlanner.parseArgs(testArgs);
            Assert.IsNotNull(results);

            sampleGraph = " ";
            System.IO.File.WriteAllText(pathString, sampleGraph);
            results = RoutePlanner.parseArgs(testArgs);
            Assert.IsNull(results);

            sampleGraph = "";
            System.IO.File.WriteAllText(pathString, sampleGraph);
            results = RoutePlanner.parseArgs(testArgs);
            Assert.IsNull(results);

            sampleGraph = null;
            System.IO.File.WriteAllText(pathString, sampleGraph);
            results = RoutePlanner.parseArgs(testArgs);
            Assert.IsNull(results);
        }

        [TestMethod]
        public void RoutePlanner_VerifyArgCount()
        {
            string[] args = {"input.txt"};
            Assert.AreEqual(1, RoutePlanner.verifyArgCount(args));
        }

        [TestMethod]
        public void RoutePlanner_TownCharToInt()
        {
            Assert.AreEqual(0, RoutePlanner.townCharToInt('A'));
            Assert.AreEqual(3, RoutePlanner.townCharToInt('d'));
            Assert.AreEqual(-1, RoutePlanner.townCharToInt('q'));
            Assert.AreEqual(-1, RoutePlanner.townCharToInt('7'));
            Assert.AreEqual(-1, RoutePlanner.townCharToInt('%'));
        }

        [TestMethod]
        public void RoutePlanner_TownIntToChar()
        {
            Assert.AreEqual('A', RoutePlanner.townIntToChar(0));
            Assert.AreEqual('C', RoutePlanner.townIntToChar(2));
            Assert.AreEqual('0', RoutePlanner.townIntToChar(5));
            Assert.AreEqual('0', RoutePlanner.townIntToChar(5096300));
            Assert.AreEqual('0', RoutePlanner.townIntToChar(-10));
        }

        [TestMethod]
        public void RoutePlanner_BuildRoutesAdjacencyMatrix()
        {
            string[] allPaths1 = { "AB3", "BC40", "CD8", "ED3", "EB1000"};
            int[,] adjMatrix = RoutePlanner.buildRoutesAdjacencyMatrix(allPaths1);
            Assert.AreEqual(adjMatrix[0, 1], 3);
            Assert.AreEqual(adjMatrix[1, 2], 40);
            Assert.AreEqual(adjMatrix[2, 3], 8);
            Assert.AreEqual(adjMatrix[4, 3], 3);
            Assert.AreEqual(adjMatrix[4, 1], 1000);
            Assert.AreEqual(adjMatrix[1, 1], 0);
            Assert.AreEqual(adjMatrix[4, 2], 0);

            string[] allPaths2 = {"ZA5"};
            adjMatrix = RoutePlanner.buildRoutesAdjacencyMatrix(allPaths2);
            Assert.IsNull(adjMatrix);

            allPaths2[0] = "BCA3";
            adjMatrix = RoutePlanner.buildRoutesAdjacencyMatrix(allPaths2);
            Assert.IsNull(adjMatrix);

            allPaths2[0] = "EA-3";
            adjMatrix = RoutePlanner.buildRoutesAdjacencyMatrix(allPaths2);
            Assert.IsNull(adjMatrix);

            allPaths2[0] = "3857";
            adjMatrix = RoutePlanner.buildRoutesAdjacencyMatrix(allPaths2);
            Assert.IsNull(adjMatrix);

            allPaths2[0] = "";
            adjMatrix = RoutePlanner.buildRoutesAdjacencyMatrix(allPaths2);
            Assert.IsNull(adjMatrix);

            string[] allPaths3 = null;
            adjMatrix = RoutePlanner.buildRoutesAdjacencyMatrix(allPaths3);
            Assert.IsNull(adjMatrix);
        }

        [TestMethod]
        public void TrainRoute_ToString()
        {
            char[] allStops = {'A', 'B', 'E', 'B'};
            int distance = 27;
            char start = 'A';
            char end = 'B';

            TrainRoute route = new TrainRoute(allStops, distance, start, end);
            Console.WriteLine(route.toString());
            string expected = "starting town: " + start + " | ending town: " + end + " | all stops on route: "; 
            for (int i = 0; i < allStops.Length; i++)
            {
                expected += allStops[i];
            }
            expected += " | total distance: " + distance;

            Assert.AreEqual(expected, route.toString(), "Actual output from toString does not match expected output.");
        }
    }
}

