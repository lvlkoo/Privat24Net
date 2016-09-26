using System;
using Privat24Net;

namespace Privat24NetExmaple
{
    internal class Program
    {
        private static void Main()
        {
            //Set api credentials, your merchant id and password
            Privat24Api.Credentials = new Privat24Credentials
            {
                MerchantId = "123456",
                Password = "qwertyuiopasdfghjkl"
            };

            //Set request criteria
            var criteria = new RequestCriteria
            {
                CardNumber = "1234567890123456"
            };

            //Make request
            var balanceInfo = new Privat24BalanceRequest().Execute(criteria).Result;

            //Process responce data
            Console.WriteLine(balanceInfo.AvailableBalance);

            Console.ReadKey();
        }
    }
}