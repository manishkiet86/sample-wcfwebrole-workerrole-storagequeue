using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using Microsoft.Azure;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WCFServiceWebRole1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }


        public testmessage senttestmessagetoqueue(testmessage composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            // Create a message and add it to the queue.
            CloudQueueMessage message = new CloudQueueMessage(composite.body);
            try
            {
                // Retrieve storage account from connection string.
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

                // Create the queue client.
                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

                // Retrieve a reference to a container.
                CloudQueue queue = queueClient.GetQueueReference("requestqueue");

                // Display message.
                // Create the queue if it doesn't already exist
                queue.CreateIfNotExists();
                             
                queue.AddMessage(message);
                Console.WriteLine("message added successfully to request queue");
                               
            }
            catch (Exception ex)
            {
                composite.error = ex.ToString();
                
            }
            return composite;
        }
    }
}
