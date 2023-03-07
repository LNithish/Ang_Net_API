namespace API.Errors
{
    public class ApiValidationErrorResponse : ApiResponse
    {
        //We know this class is for 400 status code , so we removed following from constructor
        //int statusCode, string errorMessage = null
        public ApiValidationErrorResponse() : base(400)
        {
        }   
        public IEnumerable<string> Errors { get; set; }
    }
}
