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
        public IEnumerable<dynamic> Get(string sortOrder, string searchText, bool useLambdaSearch)
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
                return filteredEntries.OrderBy(x => x.Timestamp);
            else
                return filteredEntries.OrderByDescending(x => x.Timestamp);
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
