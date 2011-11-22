using NUnit.Framework;
using Rebus.Extensions;
using Rebus.RavenDB;
using Shouldly;

namespace Rebus.Tests.Persistence.RavenDb
{
    [TestFixture, Category(TestCategories.Raven)]
    public class TestRavenDbSubscriptionStorage : RavenDbFixtureBase
    {
        RavenDbSubscriptionStorage storage;

        protected override void DoSetUp()
        {
            storage = new RavenDbSubscriptionStorage(db);
        }

        [Test]
        public void CanRemoveSubscriptionsAsWell()
        {
            // arrange
            storage.Store(typeof(SomeMessageType), "some_sub");
            storage.Store(typeof(SomeMessageType), "another_sub");
            storage.Store(typeof(AnotherMessageType), "some_sub");

            // act
            storage.Remove(typeof(SomeMessageType), "some_sub");

            // assert
            var someMessageTypeSubscribers = storage.GetSubscribers(typeof(SomeMessageType));
            someMessageTypeSubscribers.Length.ShouldBe(1);
            someMessageTypeSubscribers[0].ShouldBe("another_sub");

            var anotherMessageTypeSubscribers = storage.GetSubscribers(typeof(AnotherMessageType));
            anotherMessageTypeSubscribers.Length.ShouldBe(1);
            anotherMessageTypeSubscribers[0].ShouldBe("some_sub");
        }

        [Test]
        public void StoresSubscriptionsLikeExpected()
        {
            // arrange
            storage.Store(typeof(SomeMessageType), "some_sub");
            storage.Store(typeof(SomeMessageType), "another_sub");
            storage.Store(typeof(AnotherMessageType), "yet_another_sub");

            // act
            var someSubscribers = storage.GetSubscribers(typeof (SomeMessageType));
            var anotherSubscribers = storage.GetSubscribers(typeof (AnotherMessageType));

            // assert
            someSubscribers.ShouldContain(e => e.In(new[]{"some_sub", "another_sub"}));
            anotherSubscribers.ShouldContain(e => e.In(new[]{"yet_another_sub"}));
        }

        class SomeMessageType {}
        class AnotherMessageType {}
    }
}