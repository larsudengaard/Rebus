﻿using NUnit.Framework;
using Rebus.MongoDb;
using Rebus.Tests.Persistence.MongoDb;

namespace Rebus.Tests.Performance
{
    [TestFixture, Category(TestCategories.Mongo)]
    public class TestMongoDbSagaPersisterPerformance : MongoDbFixtureBase
    {
        MongoDbSagaPersister persister;

        protected override void DoSetUp()
        {
            persister = new MongoDbSagaPersister(ConnectionString, "sagas");
        }

        /// <summary>
        /// Initial:
        ///     Saving/updating 100 sagas 10 times took 0,2 s - that's 5026 ops/s
        ///     Saving/updating 1000 sagas 2 times took 0,8 s - that's 2404 ops/s
        /// 
        /// "Save" instead of "Insert":
        ///     Saving/updating 100 sagas 10 times took 0,2 s - that's 4249 ops/s
        ///     Saving/updating 1000 sagas 2 times took 0,9 s - that's 2234 ops/s
        /// 
        /// Safe mode true:
        ///     Saving/updating 100 sagas 10 times took 0,6 s - that's 1613 ops/s
        ///     Saving/updating 1000 sagas 2 times took 1,7 s - that's 1176 ops/s
        /// 
        /// Only ensure indexes created the first time:
        ///     Saving/updating 100 sagas 10 times took 0,6 s - that's 1626 ops/s   
        ///     Saving/updating 1000 sagas 2 times took 1,5 s - that's 1320 ops/s
        /// </summary>
        [TestCase(100, 10)]
        [TestCase(1000, 2)]
        public void RunTest(int numberOfSagas, int iterations)
        {
            SagaPersisterPerformanceFixtureBase.DoTheTest(persister, numberOfSagas, iterations);
        }
    }
}