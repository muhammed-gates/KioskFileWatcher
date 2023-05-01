using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace DigiDocFileWatcher.Model
{
    public class UtilityResponseModel
    {
        public bool result { get; set; }
        public string ResponseMessage { get; set; }
        public object ResponseData { get; set; }
        public string ResultCode { get; set; }
        public DataTable DataTable { get; set; }
    }
}
