namespace DAKKN.Application.Common.Exceptions
{
    public class UnAuthorizedException : Exception
    {
        public UnAuthorizedException()
           : base()
        {
        }

        public UnAuthorizedException(string message) :
            base(message)
        {
        }
    }
}
