using ITS.BLL.DTOs;
using ITS.BLL.Services;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ITS.API.Controllers
{
    [RoutePrefix("api/statuses")]
    public class StatusController : ApiController
    {


        // GET api/statuses
        [HttpGet]
        [Route("")]
        public HttpResponseMessage GetAll()
        {
            var statuses = StatusService.GetAllStatuses();
            return Request.CreateResponse(HttpStatusCode.OK, statuses);
        }

        // GET api/statuses/{id}
        [HttpGet]
        [Route("{id:int}")]
        public HttpResponseMessage GetById(int id)
        {
            var status = StatusService.GetStatusById(id);
            if (status == null)
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Status not found");

            return Request.CreateResponse(HttpStatusCode.OK, status);
        }

        // POST api/statuses
        [HttpPost]
        [Route("create")]
        public HttpResponseMessage Create(StatusDTO status)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            var result = StatusService.CreateStatus(status);
            if (result)
                return Request.CreateResponse(HttpStatusCode.OK, "Status created successfully");

            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Creation failed");
        }

        // PUT api/statuses/{id}
        [HttpPut]
        [Route("{id:int}")]
        public HttpResponseMessage Update(int id, StatusDTO statusDto)
        {
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);

            if (id != statusDto.Id)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "ID mismatch");

            var success = StatusService.UpdateStatus(statusDto);
            if (success)
                return Request.CreateResponse(HttpStatusCode.OK, "Status updated successfully");
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Status not found");
        }

        // DELETE api/statuses/{id}
        [HttpDelete]
        [Route("{id:int}")]
        public HttpResponseMessage Delete(int id)
        {
            var success = StatusService.DeleteStatus(id);
            if (success)
                return Request.CreateResponse(HttpStatusCode.OK, "Status deleted successfully");
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Status not found");
        }
    }
}
