﻿using System.Collections.Generic;
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
        public IEnumerable<dynamic> Get()
        {
            return LogRepository.GetAll();
        }

        [Route("log/fieldNames")]
        [HttpGet]
        public IEnumerable<dynamic> GetFieldNames()
        {
            return LogRepository.GetUniqueFieldNames();
        }
    }
}
