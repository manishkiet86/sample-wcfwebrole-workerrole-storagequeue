using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            wcfservicewebrole.Service1Client client = new wcfservicewebrole.Service1Client();
            wcfservicewebrole.testmessage testmessage = new wcfservicewebrole.testmessage();



            var msg = insertmessagequeue(client, testmessage);
        }

        private static wcfservicewebrole.testmessage insertmessagequeue(wcfservicewebrole.Service1Client client, wcfservicewebrole.testmessage testmessage)
        {
            testmessage.header = "mytestheader";
            testmessage.body = "testmessagebody";

            var resultinsert = client.senttestmessagetoqueue(testmessage);
            return resultinsert;
        }
    }
}
