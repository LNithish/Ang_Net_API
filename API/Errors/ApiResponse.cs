namespace API.Errors
{
    public class ApiResponse
    {
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;

        //setting constructor variable to null allows value to be assigned if not passed
        public ApiResponse(int statusCode, string errorMessage=null)
        {
            StatusCode = statusCode;
            //?? is he null-coalescing operators
            //it executes right side part of ??, if left side part of ?? is null
            ErrorMessage = errorMessage??GetDefaultMessageForStatusCode(statusCode);
        }

        private string GetDefaultMessageForStatusCode(int statusCode)
        {
            //new switch case
            return statusCode switch
            {
                400 => "A bad request, You have made",
                401 => "Authorized, You are not",
                404 => "Resource found, it was not",
                500 => "Errors are the path to the dark sid. Errors lead to Anger. Anger leads to Hate." +
                "Hates leads to Career change",
                _ => null
            };
        }
    }
}
