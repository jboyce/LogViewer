using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace LogViewer
{
    public class LogController : ApiController
    {
        [Route("log")]
        [HttpPost]
        public void Post(FormDataCollection data)
        {
            LogRepository.AddEntry(data.ToDictionary(d => d.Key, d => d.Value));
        }

        [Route("log")]
        [HttpGet]
        public IHttpActionResult Get(string sortOrder, string searchText, bool useLambdaSearch)
        {
            try
            {
                var allLogEntries = LogRepository.GetAll();

                IEnumerable<dynamic> filteredEntries;
                if (!string.IsNullOrEmpty(searchText))
                {
                    var filter = LogFilter.CreateFilter(searchText, useLambdaSearch);
                    filteredEntries = allLogEntries.Where(filter);
                }
                else
                    filteredEntries = allLogEntries;

                if (sortOrder == "ascending")
                    return Ok(filteredEntries.OrderBy(x => x.Timestamp));
                else
                    return Ok(filteredEntries.OrderByDescending(x => x.Timestamp));
                
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                string message = "An error occurred retrieving the log.  " + ex.Message;
                return Content(HttpStatusCode.InternalServerError, message);
            }
        }

        [Route("log/clear")]
        [HttpPost]
        public void Clear()
        {
            LogRepository.Clear();
        }

        [Route("log/metadata")]
        [HttpGet]
        public dynamic GetMetadata()
        {
            return new
            {
                FieldNames = LogRepository.GetUniqueFieldNames(),
                TotalLogEntries = LogRepository.GetAll().Count()
            }; 
        }
    }
}
