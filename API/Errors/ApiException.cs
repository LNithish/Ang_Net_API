namespace API.Errors
{
    public class ApiException : ApiResponse
    {
        //This class will handle developer exception like follows,

        //System.NullReferenceException: Object reference not set to an instance of an object.
        //at API.Controllers.BuggyController.GetServerError() in D:\Angular & DotNet\Project\API\API\Controllers\BuggyController.cs:line 31
 //  at lambda_method14(Closure, Object, Object[])
 
        //This will be used in the middleware to handle developer exceptions
        //New Exceptionmiddleware is created to catch the devloper exception
        public ApiException(int statusCode, string errorMessage = null,string Details=null) 
            : base(statusCode, errorMessage)
        {
            this.Details = Details;
        }
        public string Details { get; set; }
    }
}
