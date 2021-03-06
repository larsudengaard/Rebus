using System;
using Rebus.Bus;
using Rebus.Persistence.SqlServer;

namespace Rebus
{
    /// <summary>
    /// This is the main API of Rebus. Most application code should not depend on
    /// any other operation of <see cref="RebusBus"/>.
    /// </summary>
    public interface IBus : IDisposable
    {
        /// <summary>
        /// Sends the specified command message to the destination as specified by the currently
        /// used implementation of <see cref="IDetermineDestination"/>.
        /// </summary>
        void Send<TCommand>(TCommand message);

        /// <summary>
        /// Sends a reply back to the sender of the message currently being handled. Can only
        /// be called when a <see cref="MessageContext"/> has been established, which happens
        /// during the handling of an incoming message.
        /// </summary>
        void Reply<TReply>(TReply message);
        
        /// <summary>
        /// Sends a subscription request to the destination as specified by the currently used
        /// implementation of <see cref="IDetermineDestination"/>.
        /// </summary>
        void Subscribe<TMessage>();
        
        /// <summary>
        /// Publishes the specified event message to all endpoints that are currently subscribed.
        /// The publisher should have some kind of <see cref="IStoreSubscriptions"/> implementation,
        /// preferably a durable implementation like e.g. <see cref="SqlServerSubscriptionStorage"/>.
        /// </summary>
        void Publish<TEvent>(TEvent message);
    }
}