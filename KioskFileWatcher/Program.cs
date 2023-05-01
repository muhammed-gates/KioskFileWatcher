using KioskFileWatcher.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace KioskFileWatcher
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                if (!string.IsNullOrEmpty(KioskFileWatcher.Default.WEBAPIURL))
                {
                    var api_url = KioskFileWatcher.Default.WEBAPIURL;
                    
                    await UtilityHelper.processFiles(KioskFileWatcher.Default.DateFormat, KioskFileWatcher.Default.FolderLocation, api_url);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Error(ex, "Main", "Grabber", "Process OSR Files");
            }
        }
    }
}
