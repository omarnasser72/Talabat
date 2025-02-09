using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;

namespace Talabat.APIs.Controllers
{
    [Route("errors/{code}")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    /*
     * Purpose:
     * The primary purpose of this attribute is to exclude specific controllers or actions from the API
     * documentation. When set to IgnoreApi = true, it tells the API documentation generator 
     * (like Swagger) not to include the marked class or method in the generated API documentation.
     */
    /*
     * Use Cases:
     *      Internal APIs: If you have endpoints that are only meant for internal use or testing 
     *                     and do not need to be exposed in public documentation, you can mark them 
     *                     with this attribute.
     *      Error Handling Controllers: For controllers that deal with error handling 
     *                                  (like a global error controller), you typically wouldn’t want 
     *                                  these to show up in your API documentation since they are not 
     *                                  part of the core functionality.
     *      Development or Deprecated Endpoints: If there are endpoints that are still being developed 
     *                                           or are deprecated and should not be available to 
     *                                           consumers, marking them with this attribute helps 
     *                                           keep the API documentation clean.
     */
    public class ErrorController : ControllerBase
    {
        public ActionResult Error(int statusCode)
        {
            return NotFound(new ApiResponse(statusCode));
        }
    }
}
