using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiDocFileWatcher.Model
{
    class APIRequestModel
    {
        public string DocumentBase64 { get; set; }

        public string Username { get; set; }

        public string DocumentName { get; set; }

        public string DocumentType { get; set; }
    }

    public class APIResponseModel
    {
        public bool Result { get; set; }
        public object Data { get; set; }
        public string ResponseMessage { get; set; }
        public string ResponseCode { get; set; }
    }
}
