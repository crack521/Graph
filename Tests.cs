using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trains;

namespace TrainTests
{
    [TestClass]
    public class TrainTests
    {
        [TestMethod]
        public void TrainRoute_ToString()
        {
            char[] allStops = {'A', 'B', 'E', 'B'};
            int distance = 27;
            char start = 'A';
            char end = 'B';
            TrainRoute route = new TrainRoute(allStops, distance, start, end);
        }
    }
}

