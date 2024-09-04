using System.Net.Mime;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using Microsoft.Net.Http.Headers;
using s3_multipart_component.SERVICES;


namespace s3_multipart_component.CONTROLLERS;


[ApiController]
[Route("api/multipart")]
public class MultipartController : ControllerBase
{
   
    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile()
    {
        if (!Request.HasFormContentType || !Request.ContentType.Contains("multipart/"))
        {
            return BadRequest("The request doesn't contain a valid multipart form-data.");
        }

        var boundary = MultipartRequestHelper.GetBoundary(
            MediaTypeHeaderValue.Parse(Request.ContentType),
            FormOptions.DefaultMultipartBoundaryLengthLimit);

        var reader = new MultipartReader(boundary, HttpContext.Request.Body);
        var section = await reader.ReadNextSectionAsync();

        while (section != null)
        {
            var contentDisposition = ContentDispositionHeaderValue.Parse(section.ContentDisposition);

            if (contentDisposition.DispositionType.Equals("form-data") && 
                contentDisposition.FileName.HasValue)
            {
                var fileName = contentDisposition.FileName.Value.Trim('"');

                using (var stream = new FileStream(Path.Combine("uploads", fileName), FileMode.Create))
                {
                    await section.Body.CopyToAsync(stream);
                }
            }

            section = await reader.ReadNextSectionAsync();
        }

        return Ok("File uploaded successfully.");
    }



    [HttpGet()]
    public String check()
    {
        return "hello";
    }
    
    
}