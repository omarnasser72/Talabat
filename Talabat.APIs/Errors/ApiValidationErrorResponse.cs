namespace Talabat.APIs.Errors
{
    public class ApiValidationErrorResponse : ApiResponse
    {
        /*
         * specifically addresses errors related to model validation. 
         * These errors occur when the input data does not meet the defined validation criteria 
         * (e.g., missing required fields, invalid data types).
         */
        public IEnumerable<string> Errors { get; set; }
        public ApiValidationErrorResponse() : base(400)
        {
            Errors = new List<string>();
        }
    }
}
