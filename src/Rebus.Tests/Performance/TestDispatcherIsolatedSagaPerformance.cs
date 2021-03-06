﻿using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using Rebus.Bus;
using Rebus.Persistence.InMemory;
using Rebus.Tests.Integration;

namespace Rebus.Tests.Performance
{
    [TestFixture]
    public class TestDispatcherIsolatedSagaPerformance : FixtureBase
    {
        Dispatcher dispatcher;
        HandlerActivatorForTesting activator;
        SagaDataPersisterForTesting persister;

        protected override void DoSetUp()
        {
            activator = new HandlerActivatorForTesting();
            persister = new SagaDataPersisterForTesting();
            dispatcher = new Dispatcher(persister,
                                        activator,
                                        new InMemorySubscriptionStorage(),
                                        new TrivialPipelineInspector());
        }

        /// <summary>
        /// Initial:
        ///     10000 iterations took 4,406 s - that's 2269,7 msg/s
        /// 
        /// After caching of fields to index:
        ///     10000 iterations took 0,859 s - that's 11638,5 msg/s
        /// 
        /// 
        /// 
        /// 
        /// 
        /// </summary>
        [TestCase(1000)]
        [TestCase(10000)]
        [TestCase(100000)]
        public void DispatchLotsOfMessagesToSaga(int iterations)
        {
            activator.UseHandler(new MessageCountingSaga());

            var correlationId = "some_id";
            var message = new MessageToCount { CorrelationId = correlationId };

            var stopwatch = Stopwatch.StartNew();
            for(var counter = 0; counter < iterations ;counter++)
            {
                dispatcher.Dispatch(message);
            }

            var sagaData = persister.Cast<MessageCountingSagaData>().Single();
            Assert.AreEqual(correlationId, sagaData.CorrelationId);
            Assert.AreEqual(iterations, sagaData.Counter);

            var elapsed = stopwatch.Elapsed;
            
            Console.WriteLine("{0} iterations took {1:0.000} s - that's {2:0.0} msg/s",
                              iterations,
                              elapsed.TotalSeconds,
                              iterations/elapsed.TotalSeconds);
        }

        class MessageCountingSaga : Saga<MessageCountingSagaData>,
            IAmInitiatedBy<MessageToCount>
        {
            public override void ConfigureHowToFindSaga()
            {
                Incoming<MessageToCount>(m => m.CorrelationId).CorrelatesWith(d => d.CorrelationId);
            }

            public void Handle(MessageToCount message)
            {
                Data.CorrelationId = message.CorrelationId;
                Data.Counter++;
            }
        }

        class MessageToCount
        {
            public string CorrelationId { get; set; }
        }

        class MessageCountingSagaData : ISagaData
        {
            public Guid Id { get; set; }

            public string CorrelationId { get; set; }

            public int Counter { get; set; }
        }
    }
}