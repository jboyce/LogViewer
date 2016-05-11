using System.Collections.Generic;
using System.Linq;
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
        public IEnumerable<dynamic> Get(string sortOrder)
        {
            var allLogEntries = LogRepository.GetAll();

            if (sortOrder == "ascending")
                return allLogEntries.OrderBy(x => x.Timestamp);
            else
                return allLogEntries.OrderByDescending(x => x.Timestamp);
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
