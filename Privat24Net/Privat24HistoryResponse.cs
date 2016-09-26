using System.Collections.Generic;

namespace Privat24Net
{
    public class Privat24HistoryResponse : IResponse
    {
        public string Status { get; set; }
        public double Credit { get; set; }
        public double Debet { get; set; }
        public List<Privat24HistoryStatement> HistoryStatements { get; set; }
        public string StringResponse { get; set; }
    }
}