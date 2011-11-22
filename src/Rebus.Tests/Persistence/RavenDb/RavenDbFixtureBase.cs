using MongoDB.Driver;
using NUnit.Framework;
using Raven.Client;
using Raven.Client.Embedded;

namespace Rebus.Tests.Persistence.RavenDb
{
    public abstract class RavenDbFixtureBase
    {
        protected IDocumentStore db;

        [SetUp]
        public void SetUp()
        {
            db = new EmbeddableDocumentStore
            {
                RunInMemory = true
            };
            db.Initialize();

            DoSetUp();
        }

        protected virtual void DoSetUp()
        {
        }

        [TearDown]
        public void TearDown()
        {
            DoTearDown();
        }

        protected virtual void DoTearDown()
        {
        }
    }
}