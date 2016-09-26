using System;

namespace Privat24Net
{
    public class Privat24HistoryStatement
    {
        public string Description { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Amount { get; set; }
        public string CardAmount { get; set; }
        public string Rest { get; set; }
        public string Terminal { get; set; }
    }
}