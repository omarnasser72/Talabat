namespace Talabat.APIs.Errors
{
    public class ApiExceptionResponse : ApiResponse
    {
        /*
         * handling general exceptions that occur during the processing of a request. 
         * This could include unhandled exceptions, such as null reference exceptions or database errors.
         */
        public string? Details { get; set; }

        public ApiExceptionResponse(int statusCode, string? message = null, string? details = null) : base(statusCode, message)
        {
            Details = details;
        }
    }
}
