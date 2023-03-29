using System;
using System.Collections.Generic;
using System.Text;
using ThomsonReuters.BackgroundOperations.Messaging.Models;
using ThomsonReuters.BackgroundOperations.Messaging;

using ThomsonReuters.BackgroundOperations.Messaging.Models.Status;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Aranzadi.DocumentAnalysis.Messaging.BackgroundOperations
{
    public class AnalisisDocumentosDefaultFactory : IAnalisisDocumentosFactory
    {



        private static Object lockObject = new Object();
        private static MessageFactory messageFactory;        
        private static IHttpClientFactory httpClientFactory;

        private static void ConfigureFactory()
        {
            if (messageFactory == null || httpClientFactory == null)
            {
                lock (lockObject)
                {
                    if (messageFactory == null)
                    {
                        messageFactory = new MessageFactory();
                    }
                    if (httpClientFactory == null)
                    {
                        ServiceProvider serviceProvider = new ServiceCollection().AddHttpClient()
                            .BuildServiceProvider();
                        httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
                        //serviceProvider.Dispose();
                    }
                }
            }
        }

        
        private MessagingConfiguration confi;

        public AnalisisDocumentosDefaultFactory(MessagingConfiguration confi)
        {
            this.confi = confi;
        }
             

        public IClient GetClient()
        {
            ConfigureFactory();

            var sender = messageFactory.GetSender(new ConnectionSettings(confi.ServicesBusConnectionString), null);

            return new MessagingClient(sender, confi, httpClientFactory);
        }

        public IConsumer GetConsumer()
        {
            ConfigureFactory();
            var receiver = messageFactory.GetReceiver(new ConnectionSettings(confi.ServicesBusConnectionString), null);
            var sender = messageFactory.GetSender(new ConnectionSettings(confi.ServicesBusConnectionString), null);
            return new MessagingConsumer(receiver, confi, sender);
        }

  
    }
}
