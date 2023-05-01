
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KioskFileWatcher.Helper
{
    public class UtilityHelper
    {
        public static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            return false;
        }

        public async static Task processFiles(string DateFormat, string FolderLocation, string API_URL)
        {
            if (Directory.Exists(FolderLocation))
            {
                var osrFiles = new DirectoryInfo(FolderLocation).GetFiles("*.txt");
                if (osrFiles != null)
                {
                    if (osrFiles.Length > 0)
                    {
                        foreach (var osrFileInfo in osrFiles)
                        {
                            if (osrFileInfo.Exists &&
                                !string.IsNullOrEmpty(osrFileInfo.Name) &&
                                osrFileInfo.Name.ToUpper().Contains("OSR"))
                            {
                                do
                                {
                                    Thread.Sleep(1000);
                                } while (IsFileLocked(osrFileInfo));

                                LogHelper.Instance.Debug($"Processing file :-  {osrFileInfo.Name}", "processFiles", "Grabber", "Process OSR Files");
                                List<Model.ReservationModel> reservationModels = new List<Model.ReservationModel>();

                                using (StreamReader reader = new StreamReader(osrFileInfo.FullName))
                                {
                                    try
                                    {
                                        string line = "";
                                        while ((line = reader.ReadLine()) != null)
                                        {
                                            Model.ReservationModel reservationModel = new Model.ReservationModel();

                                            string[] tokens = line.Split(';');
                                            if (tokens != null && tokens.Length >= 18)
                                            {
                                                reservationModel.ConfirmationNumber = tokens[0];
                                                reservationModel.CRSNumber = tokens[1];
                                                reservationModel.ExternalRefNumber = tokens[2];
                                                reservationModel.MembershipType = tokens[3];
                                                reservationModel.MembershipNumber = tokens[4];
                                                if (!string.IsNullOrEmpty(tokens[5]))
                                                {
                                                    reservationModel.CheckinDate = DateTime.ParseExact(tokens[5], DateFormat, CultureInfo.InvariantCulture);
                                                }
                                                if (!string.IsNullOrEmpty(tokens[6]))
                                                {
                                                    reservationModel.CheckoutDate = DateTime.ParseExact(tokens[6], DateFormat, CultureInfo.InvariantCulture);
                                                }
                                                reservationModel.FullName = tokens[7];
                                                if (!string.IsNullOrEmpty(reservationModel.FullName))
                                                {
                                                    reservationModel.FullName = reservationModel.FullName.Replace(", ", ",");
                                                }
                                                reservationModel.FirstName = tokens[8];
                                                reservationModel.MiddleName = tokens[9];
                                                reservationModel.LastName = tokens[10];
                                                reservationModel.RoomNumber = tokens[11];
                                                reservationModel.ReservationNameID = tokens[12];
                                                reservationModel.ReservationStatus = tokens[13];
                                                if (!string.IsNullOrEmpty(tokens[14]))
                                                {
                                                    reservationModel.ShareFlag = tokens[14] == "Y" ? true : false;
                                                }
                                                reservationModel.ShareID = tokens[15];
                                                reservationModel.AccorRefernce = tokens[16];
                                                if (!string.IsNullOrEmpty(tokens[17]))
                                                {
                                                    if (!string.IsNullOrEmpty(reservationModel.FullName))
                                                    {
                                                        reservationModel.FullName += "," + tokens[17];
                                                    }
                                                }
                                                reservationModels.Add(reservationModel);
                                            }
                                            //else
                                            //{
                                            //    LogHelper.Instance.Log($"Unsupported data format {osrFileInfo.Name}", "processFiles", "Grabber", "Process OSR Files");
                                            //    if (File.Exists(osrFileInfo.FullName))
                                            //    {
                                            //        if (!Directory.Exists(Path.Combine(FolderLocation, "ErrorFiles")))
                                            //            Directory.CreateDirectory(Path.Combine(FolderLocation, "ErrorFiles"));
                                            //        File.Move(osrFileInfo.FullName, Path.Combine(FolderLocation, $"ErrorFiles/{osrFileInfo.Name}"));
                                            //    }
                                            //    break;
                                            //}
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        if (reader != null)
                                            reader.Dispose();
                                        LogHelper.Instance.Error(ex, "processFiles", "Grabber", "Process OSR Files");
                                        if (File.Exists(osrFileInfo.FullName))
                                        {
                                            if (!Directory.Exists(Path.Combine(FolderLocation, "ErrorFiles")))
                                                Directory.CreateDirectory(Path.Combine(FolderLocation, "ErrorFiles"));
                                            File.Move(osrFileInfo.FullName, Path.Combine(FolderLocation, $"ErrorFiles/{osrFileInfo.Name}"));
                                        }
                                        continue;
                                    }
                                }


                                if (reservationModels != null && reservationModels.Count > 0)
                                {
                                    try
                                    {
                                        using (var httpClient = new HttpClient())
                                        {
                                            //Console.WriteLine("sending files to API");
                                            //httpClient.BaseAddress = new Uri(API_URL);
                                            httpClient.DefaultRequestHeaders.Clear();
                                            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                                            HttpContent requestContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(new { RequestObject = reservationModels }), Encoding.UTF8, "application/json");
                                            httpClient.Timeout = TimeSpan.FromMinutes(10);
                                            HttpResponseMessage httpResponse = await httpClient.PostAsync(new Uri($"{API_URL}/Local/PushKioskReservationReportDetails"), requestContent);
                                            if (httpResponse != null && httpResponse.IsSuccessStatusCode)
                                            {
                                                var responseMessage = await httpResponse.Content.ReadAsStringAsync();
                                                //Console.WriteLine("response : "+ responseMessage);
                                                if (Newtonsoft.Json.JsonConvert.DeserializeObject<Model.LocalAPIResponseModel>(responseMessage).result)
                                                {
                                                    if (File.Exists(osrFileInfo.FullName))
                                                    {
                                                        try
                                                        {
                                                            File.Delete(osrFileInfo.FullName);
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            LogHelper.Instance.Error(ex, "processFiles", "Grabber", "Process OSR Files");
                                                            try
                                                            {
                                                                if (!Directory.Exists(Path.Combine(FolderLocation, "ErrorFiles")))
                                                                    Directory.CreateDirectory(Path.Combine(FolderLocation, "ErrorFiles"));
                                                                File.Move(osrFileInfo.FullName, Path.Combine(FolderLocation, $"ErrorFiles/{osrFileInfo.Name}"));
                                                            }
                                                            catch (Exception ex1)
                                                            {
                                                                LogHelper.Instance.Error(ex1, "processFiles", "Grabber", "Process OSR Files");
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    LogHelper.Instance.Log($"Failled to upload the data the from the file : {osrFileInfo.Name}, with API response- {responseMessage}", "processFiles", "Grabber", "Process OSR Files");
                                                    try
                                                    {
                                                        if (!Directory.Exists(Path.Combine(FolderLocation, "ErrorFiles")))
                                                            Directory.CreateDirectory(Path.Combine(FolderLocation, "ErrorFiles"));
                                                        File.Move(osrFileInfo.FullName, Path.Combine(FolderLocation, $"ErrorFiles/{osrFileInfo.Name}"));
                                                    }
                                                    catch (Exception ex1)
                                                    {
                                                        LogHelper.Instance.Error(ex1, "processFiles", "Grabber", "Process OSR Files");
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                LogHelper.Instance.Log($"Failled to upload the data the from the file : {osrFileInfo.Name}, with response- {httpResponse.ReasonPhrase}", "processFiles", "Grabber", "Process OSR Files");
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        LogHelper.Instance.Error(ex, "processFiles", "Grabber", "Process OSR Files");
                                        try
                                        {
                                            File.Delete(osrFileInfo.FullName);
                                        }
                                        catch (Exception ex1)
                                        {
                                            LogHelper.Instance.Error(ex1, "processFiles", "Grabber", "Process OSR Files");
                                            try
                                            {
                                                if (!Directory.Exists(Path.Combine(FolderLocation, "ErrorFiles")))
                                                    Directory.CreateDirectory(Path.Combine(FolderLocation, "ErrorFiles"));
                                                File.Move(osrFileInfo.FullName, Path.Combine(FolderLocation, $"ErrorFiles/{osrFileInfo.Name}"));
                                            }
                                            catch (Exception ex2)
                                            {
                                                LogHelper.Instance.Error(ex2, "processFiles", "Grabber", "Process OSR Files");
                                            }
                                        }
                                        continue;
                                    }
                                }
                                else
                                {
                                    if (File.Exists(osrFileInfo.FullName))
                                    {
                                        if (!Directory.Exists(Path.Combine(FolderLocation, "ErrorFiles")))
                                            Directory.CreateDirectory(Path.Combine(FolderLocation, "ErrorFiles"));
                                        File.Move(osrFileInfo.FullName, Path.Combine(FolderLocation, $"ErrorFiles/{osrFileInfo.Name}"));
                                    }
                                }
                            }
                            else
                            {
                                LogHelper.Instance.Log($"Unsupported file name format, moving file to error folder", "processFiles", "Grabber", "Process OSR Files");
                                if (File.Exists(osrFileInfo.FullName))
                                {
                                    if (!Directory.Exists(Path.Combine(FolderLocation, "ErrorFiles")))
                                        Directory.CreateDirectory(Path.Combine(FolderLocation, "ErrorFiles"));
                                    File.Move(osrFileInfo.FullName, Path.Combine(FolderLocation, $"ErrorFiles/{osrFileInfo.Name}"));
                                }
                            }
                        }
                    }
                    else
                    {
                        LogHelper.Instance.Debug($"No files to process", "processFiles", "Grabber", "Process OSR Files");
                    }
                }
                else
                {
                    LogHelper.Instance.Debug($"No files to process", "processFiles", "Grabber", "Process OSR Files");
                }
            }
            else
            {
                LogHelper.Instance.Log($"Configured folder location does not exist \"{FolderLocation}\"", "processFiles", "Grabber", "Process OSR Files");
                Environment.Exit(0);
            }
        }
    }
}
