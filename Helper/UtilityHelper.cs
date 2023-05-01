using DigiDocFileWatcher.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiDocFileWatcher.Helper
{
    public class UtilityHelper
    {
        public static UtilityResponseModel fetchAPIURL(string connectionString,string property_code)
        {
            try
            {
                var spResponse = new DapperHelper().ExecuteSP<URLModelList>("usp_getAPIurlforthepropertycode", connectionString, new { PropertyCode = property_code });
                if (spResponse != null)
                {
                    if (spResponse.First().Result.Equals("200"))
                    {
                        return new UtilityResponseModel()
                        {
                            ResponseData = spResponse,
                            result = true,
                            ResultCode = "200",
                            ResponseMessage = spResponse.First().Message
                        };
                    }
                    else
                    {
                        return new UtilityResponseModel()
                        {
                            result = false,
                            ResultCode = spResponse.First().Result,
                            ResponseMessage = spResponse.First().Message
                        };
                    }
                }
                else
                {
                    return new UtilityResponseModel()
                    {
                        result = false,
                        ResultCode = "-2",
                        ResponseMessage = "DB Error"
                    };
                }
            }
            catch (Exception ex)
            {
                return new UtilityResponseModel()
                {
                    result = false,
                    ResultCode = "-1",
                    ResponseMessage = "Generic exception"
                };
            }
        }
    }
}
