using System;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Privat24Net
{
    public class Privat24BalanceRequest : Privat24Request<Privat24BalanceRepsonse>
    {
        public Privat24BalanceRequest()
        {
        }

        public Privat24BalanceRequest(Privat24Credentials credentials) : base(credentials)
        {
        }

        public override async Task<Privat24BalanceRepsonse> Execute(RequestCriteria criteria)
        {
            if (criteria == null)
                throw new Privat24Exception("Request criteria can't be null");

            if (string.IsNullOrEmpty(criteria.CardNumber))
                throw new Privat24Exception("Card number can't be null or empty");

            var request = RequestBuilder.GetBalanceRequest(Credentials, criteria.CardNumber);

            using (var client = new HttpClient())
            {
                var response =
                    await client.PostAsync(Constants.BalanceMthodUrl, new StringContent(request, Encoding.UTF8));

                if (!response.IsSuccessStatusCode)
                    return null;

                using (response)
                {
                    var stringContent = await response.Content.ReadAsStringAsync();
                    return GenerateResponse(stringContent);
                }
            }
        }

        protected override Privat24BalanceRepsonse GenerateResponse(string content)
        {
            var document = XDocument.Parse(content);
            var balanceNode = document.Root?.Element("data")?.Element("info")?.Element("cardbalance");
            var cardInfoNode = balanceNode?.Element("card");

            var response = new Privat24BalanceRepsonse
            {
                Account = cardInfoNode?.Element("account")?.Value,
                CardName = cardInfoNode?.Element("acc_name")?.Value,
                CardNumber = cardInfoNode?.Element("card_number")?.Value,
                CardState = cardInfoNode?.Element("card_stat")?.Value,
                Currency = cardInfoNode?.Element("currency")?.Value,
                CardType = cardInfoNode?.Element("card_type")?.Value,
                StringResponse = content,
                UpdateTime =
                    DateTime.ParseExact(balanceNode?.Element("bal_date")?.Value ?? "01.01.01 00:00",
                        "dd.MM.yy HH:mm", CultureInfo.InvariantCulture),
                FinLimit =
                    double.Parse(balanceNode?.Element("fin_limit")?.Value ?? "0",
                        CultureInfo.InvariantCulture),
                AvailableBalance =
                    double.Parse(balanceNode?.Element("av_balance")?.Value ?? "0",
                        CultureInfo.InvariantCulture),
                Balance =
                    double.Parse(balanceNode?.Element("balance")?.Value ?? "0",
                        CultureInfo.InvariantCulture)
            };

            return response;
        }
    }
}