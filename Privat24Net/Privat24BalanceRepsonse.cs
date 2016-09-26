using System;

namespace Privat24Net
{
    public class Privat24BalanceRepsonse : IResponse
    {
        public string Account { get; set; }
        public string CardNumber { get; set; }
        public string CardName { get; set; }
        public string CardType { get; set; }
        public string Currency { get; set; }
        public string CardState { get; set; }
        public double FinLimit { get; set; }
        public double AvailableBalance { get; set; }
        public double Balance { get; set; }
        public DateTime UpdateTime { get; set; }
        public string StringResponse { get; set; }
    }
}