using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.Azure;

namespace WorkerRole1
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.TraceInformation("WorkerRole1 is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("WorkerRole1 has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("WorkerRole1 is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("WorkerRole1 has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            // Retrieve storage account from connection string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create the queue client
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue
            CloudQueue queue = queueClient.GetQueueReference("requestqueue");

            // Peek at the next message
            //CloudQueueMessage peekedMessage = queue.PeekMessage();

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    // Get the message from the queue and update the message contents.
                    CloudQueueMessage message = queue.GetMessage();

                    if (queue.ApproximateMessageCount>0)
                    {

                    message.SetMessageContent(message.Id);
                    queue.UpdateMessage(message,
                        TimeSpan.FromSeconds(60.0),  // Make it invisible for another 60 seconds.
                        MessageUpdateFields.Content | MessageUpdateFields.Visibility);

                    // Display message.
                    //Console.WriteLine(peekedMessage.AsString);
                    Console.WriteLine("deleting message..");
                    queue.DeleteMessage(message);

                    // Retrieve a reference to a container.
                    CloudQueue queue2 = queueClient.GetQueueReference("responsequeue");

                    // Display message.
                    // Create the queue if it doesn't already exist
                    queue2.CreateIfNotExists();

                    // Create a message and add it to the queue.
                    queue2.AddMessage(message);
                    Console.WriteLine("message added successfully to response queue");


                    Trace.TraceInformation("Working");
                    await Task.Delay(1000);
                }

                }
                catch (Exception ex)
                {
                    cancellationTokenSource.Cancel();

                }

            }
            

        }
    }
}
