using ITS.BLL.DTOs;
using ITS.BLL.Services;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace ITS.API.Controllers
{
    [RoutePrefix("api/issues")]
    public class IssueController : ApiController
    {
        [HttpGet]
        [Route("")]
        public HttpResponseMessage GetAll()
        {
            var issues = IssueService.GetAllIssues();
            return Request.CreateResponse(HttpStatusCode.OK, issues);
        }

        [HttpGet]
        [Route("{id:int}")]
        public HttpResponseMessage GetById(int id)
        {
            var issue = IssueService.GetIssueById(id);
            if (issue == null)
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Issue not found");

            return Request.CreateResponse(HttpStatusCode.OK, issue);
        }

        [HttpPost]
        [Route("")]
        public HttpResponseMessage Create(IssueDTO issueDto)
        {
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);

            var success = IssueService.CreateIssue(issueDto);
            if (success)
                return Request.CreateResponse(HttpStatusCode.Created, "Issue created successfully");

            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Failed to create issue (invalid user/status?)");
        }

        [HttpPut]
        [Route("{id:int}")]
        public HttpResponseMessage Update(int id, IssueDTO issueDto)
        {
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);

            if (id != issueDto.Id)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "ID mismatch");

            var success = IssueService.UpdateIssue(issueDto);
            if (success)
                return Request.CreateResponse(HttpStatusCode.OK, "Issue updated successfully");

            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Issue not found");
        }

        [HttpDelete]
        [Route("{id:int}")]
        public HttpResponseMessage Delete(int id)
        {
            var success = IssueService.DeleteIssue(id);
            if (success)
                return Request.CreateResponse(HttpStatusCode.OK, "Issue deleted successfully");

            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Issue not found");
        }


        [HttpPost]
        [Route("upload")]
        public async Task<HttpResponseMessage> Upload()
        {
            if (!Request.Content.IsMimeMultipartContent())
                return Request.CreateErrorResponse(HttpStatusCode.UnsupportedMediaType, "Unsupported media type");

            var root = HttpContext.Current.Server.MapPath("~/UploadedFiles");
            Directory.CreateDirectory(root);
            var provider = new MultipartFormDataStreamProvider(root);

            await Request.Content.ReadAsMultipartAsync(provider);

            var form = provider.FormData;
            var files = provider.FileData;

            // Extract issue data from form
            var issueDto = new IssueDTO
            {
                Title = form["Title"],
                Description = form["Description"],
                StatusId = int.Parse(form["StatusId"]),
                CreatedByUserId = int.Parse(form["CreatedByUserId"]),
                CreatedAt = DateTime.Now
            };

            if (files.Count > 0)
            {
                var file = files[0];
                var fileName = Path.GetFileName(file.LocalFileName);
                issueDto.AttachmentPath = "/UploadedFiles/" + file.Headers.ContentDisposition.FileName.Trim('"');
                File.Move(file.LocalFileName, Path.Combine(root, Path.GetFileName(issueDto.AttachmentPath)));
            }

            var success = IssueService.CreateIssue(issueDto);
            if (success)
                return Request.CreateResponse(HttpStatusCode.Created, "Issue created with file");
            else
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Failed to create issue");
        }


        [HttpPost]
        [Route("search")]
        public HttpResponseMessage Search(IssueFilterDTO filter)
        {
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);

            var result = IssueService.SearchIssues(filter);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }


    }
}
