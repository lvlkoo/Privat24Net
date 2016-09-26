using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Privat24Net
{
    public class Privat24HistoryRequest : Privat24Request<Privat24HistoryResponse>
    {
        public Privat24HistoryRequest()
        {
        }

        public Privat24HistoryRequest(Privat24Credentials credentials) : base(credentials)
        {
        }

        public override async Task<Privat24HistoryResponse> Execute(RequestCriteria criteria)
        {
            if (criteria == null)
                throw new Privat24Exception("Request criteria can't be null");

            if (string.IsNullOrEmpty(criteria.CardNumber))
                throw new Privat24Exception("Card number can't be null or empty");

            var request = RequestBuilder.GetHistoryRequest(Credentials, criteria.CardNumber,
                criteria.StartDate ?? DateTime.Today.AddDays(-30), criteria.EndDate ?? DateTime.Today);


            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(Constants.HistoryMethodUrl, new StringContent(request));

                if (!response.IsSuccessStatusCode)
                    return null;

                using (response)
                {
                    var stringContent = await response.Content.ReadAsStringAsync();
                    return GenerateResponse(stringContent);
                }
            }
        }

        protected override Privat24HistoryResponse GenerateResponse(string content)
        {
            var document = XDocument.Parse(content);
            var statementsElement = document.Root?.Element("data")?.Element("info")?.Element("statements");

            var response = new Privat24HistoryResponse
            {
                StringResponse = content,
                Status = statementsElement?.Attribute("status")?.Value,
                Credit = double.Parse(statementsElement?.Attribute("credit")?.Value ?? "0",
                    CultureInfo.InvariantCulture),
                Debet = double.Parse(statementsElement?.Attribute("debet")?.Value ?? "0", CultureInfo.InvariantCulture),
                HistoryStatements = new List<Privat24HistoryStatement>()
            };


            var statements = statementsElement?.Elements();
            if (statements != null)
            {
                foreach (var statement in statements)
                {
                    response.HistoryStatements.Add(new Privat24HistoryStatement
                    {
                        Amount = statement.Attribute("amount")?.Value,
                        CardAmount = statement.Attribute("cardamount")?.Value,
                        Description = statement.Attribute("description")?.Value,
                        Rest = statement.Attribute("rest")?.Value,
                        Terminal = statement.Attribute("terminal")?.Value,
                        TransactionDate =
                            DateTime.ParseExact(statement.Attribute("trandate")?.Value, "yyyy-MM-dd",
                                CultureInfo.InvariantCulture)
                    });
                }
            }

            return response;
        }
    }
}