﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeText.Common.Events;

namespace WeText.Common.Messaging
{
    public sealed class EventConsumer : IEventConsumer
    {
        private readonly IEnumerable<IDomainEventHandler> eventHandlers;
        private readonly IMessageSubscriber subscriber;
        private bool disposed;

        ~EventConsumer()
        {
            Dispose(false);
        }

        public EventConsumer(IMessageSubscriber subscriber, IEnumerable<IDomainEventHandler> eventHandlers)
        {
            this.subscriber = subscriber;
            this.eventHandlers = eventHandlers;
            subscriber.MessageReceived += async (sender, e) =>
            {
                if (this.eventHandlers != null)
                {
                    foreach (var handler in this.eventHandlers)
                    {
                        await handler.HandleAsync(e.Message);
                    }
                }
            };
        }

        public IEnumerable<IDomainEventHandler> EventHandlers => eventHandlers;

        public IMessageSubscriber Subscriber => subscriber;

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!disposed)
                {
                    this.subscriber.Dispose();
                    disposed = true;
                }
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}