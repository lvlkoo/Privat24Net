using System.Threading.Tasks;

namespace Privat24Net
{
    public abstract class Privat24Request<T> where T : IResponse
    {
        protected Privat24Credentials Credentials;

        protected Privat24Request()
        {
            if (Privat24Api.Credentials == null)
                throw new Privat24Exception("Credentials can't be null");

            Credentials = Privat24Api.Credentials;
        }

        protected Privat24Request(Privat24Credentials credentials)
        {
            if (credentials == null && Privat24Api.Credentials == null)
                throw new Privat24Exception("Credentials can't be null");

            Credentials = credentials;
        }

        public abstract Task<T> Execute(RequestCriteria criteria);
        protected abstract T GenerateResponse(string content);
    }
}