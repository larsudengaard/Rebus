using System;
using System.Collections.Generic;
using Raven.Abstractions.Commands;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Json.Linq;
using System.Linq;

namespace Rebus.RavenDB
{
    public class RavenDbSubscriptionStorage : IStoreSubscriptions
    {
        private readonly IDocumentStore store;

        public RavenDbSubscriptionStorage(IDocumentStore store)
        {
            this.store = store;
        }

        public void Store(Type messageType, string subscriberInputQueue)
        {
            EnsureSubscription(messageType);
            using (var session = store.OpenSession())
            {

                session.Advanced.DatabaseCommands.Batch(new[]
                {
                    new PatchCommandData
                    {
                        Key = messageType.FullName,
                        Patches = new[]
                        {
                            new PatchRequest
                            {
                                Type = PatchCommandType.Add,
                                Name = "Endpoints",
                                Value = new RavenJValue(subscriberInputQueue),
                            }
                        }
                    }
                });
            }
        }

        public void Remove(Type messageType, string subscriberInputQueue)
        {
            EnsureSubscription(messageType);
            using (var session = store.OpenSession())
            {
                session.Advanced.DatabaseCommands.Batch(new[]
                {
                    new PatchCommandData
                    {
                        Key = messageType.FullName,
                        Patches = new[]
                        {
                            new PatchRequest
                            {
                                Type = PatchCommandType.Remove,
                                Name = "Endpoints",
                                Value = new RavenJValue(subscriberInputQueue),
                            }
                        }
                    }
                });
            }
        }

        public string[] GetSubscribers(Type messageType)
        {
            using (var session = store.OpenSession())
            {
                var subscription = session.Load<RebusSubscription>(messageType.FullName);
                return subscription == null ? new string[0] : subscription.Endpoints.ToArray();
            }
        }

        void EnsureSubscription(Type messageType)
        {
            using (var session = store.OpenSession())
            {
                var subscription = session.Load<RebusSubscription>(messageType.FullName);
                if (subscription == null)
                {
                    session.Store(new RebusSubscription
                    {
                        Id = messageType.FullName,
                        Endpoints = new List<string>()
                    });
                    session.SaveChanges();
                }
            }
        }
    }
}