using System;

namespace Privat24Net
{
    public class RequestCriteria
    {
        public string CardNumber { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; } = null;
        public DateTime? EndDate { get; set; } = null;
    }
}