using Aranzadi.DocumentAnalysis.DTO;
using Aranzadi.DocumentAnalysis.DTO.Request;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using ThomsonReuters.BackgroundOperations.Messaging;
using ThomsonReuters.BackgroundOperations.Messaging.Extensions;
using ThomsonReuters.BackgroundOperations.Messaging.Models;

namespace Aranzadi.DocumentAnalysis.Messaging.BackgroundOperations
{
    public class MessagingConsumer : IConsumer

    {
        private readonly IMessageReceiver receiver;
        private Func<AnalysisContext, DocumentAnalysisRequest, Task<bool>> theAction;
        private CancellationToken cancellationToken;
        private readonly MessagingConfiguration confi;
        private readonly IMessageSender sender;

        public MessagingConsumer(IMessageReceiver receiver
            , MessagingConfiguration confi
            , IMessageSender sender)
        {
            this.receiver = receiver;

            this.confi = confi;
            this.sender = sender;
        }

        public async void StartProcess(Func<AnalysisContext, DocumentAnalysisRequest, Task<bool>> theAction)
        {

            this.theAction = theAction;

            await receiver.OnReceiveMessage<DocumentAnalysisRequest>(confi.ServicesBusCola, async (menssage, cancelationToken) =>
            {
                try
                {

                    AnalysisContext additionalData = JsonConvert.DeserializeObject<AnalysisContext>(
                        menssage.Message.AdditionalData != null ?
                        menssage.Message.AdditionalData.ToString() : String.Empty);
                    foreach (var chunck in menssage.Message.DataChunks)
                    {

                        await this.theAction(additionalData, chunck.Data);
                    }
                }
                catch (Exception ex)
                {
                    menssage.Message.DataChunks.ForEach(chunk =>
                    {
                        chunk.Success = false;
                        chunk.Detail = ex.Message;
                    });
                }
                finally
                {
                    var workDoneMessage = menssage.Message.GetWorkDoneMessage(confi.Source);

                    await sender.Send(MessageDestinations.WorkDone, workDoneMessage);
                    await menssage.Complete();
                }
            });
        }


    }
}
