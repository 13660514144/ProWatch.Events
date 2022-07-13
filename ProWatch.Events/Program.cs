using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using HoneywellAccess.ProWatch.PWDataModel.DataContracts;
using System.Configuration;
using System.Diagnostics;
using EmbeddedWebBrowserSolution;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using EmbeddedWebBrowserSolution.Code.Class;
using HelperTools;
using System.Timers;
using System.IO;

namespace ProWatch
{
    public class Program
    {
        public static string PasslogUrl = string.Empty;
        public static JObject ConFig;
        public static string CurrentPath = AppDomain.CurrentDomain.BaseDirectory;
        public static string LogFile = "SendErr.Log";
        public static long sleep;
        static void Main(string[] args)
        {
            string processName = Process.GetCurrentProcess().ProcessName;
            Process[] processes = Process.GetProcessesByName(processName);
            if (processes.Length > 1)
            {
                Environment.Exit(1);
            }
            ConFig = AppConfigurtaionServices.GetConfig($@"{CurrentPath}ConFig.json");
            PasslogUrl = ConFig["PassLog"].ToString();
            sleep=(long)ConFig["sleep"];
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(theout); //到达时间的时候执行事件；
            // 设置引发时间的时间间隔  
            aTimer.Interval = sleep;
            aTimer.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；
            aTimer.Enabled = true; //是否执行System.Timers.Timer.Elapsed事件；

            EventSubscribe();

        }
        private static void theout(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                List<LogModels> L = new List<LogModels>();
             
                using (FileStream fs = new FileStream(LogFile, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        String line;                       
                        while ((line = sr.ReadLine()) != null)
                        {
                            LogModels Lm = new LogModels();
                            Lm = (LogModels)JsonConvert.DeserializeObject(line, typeof(LogModels));
                            L.Add(Lm);
                        }
                        sr.Close();
                        sr.Dispose();
                    }
                }
                System.IO.File.Delete(LogFile);
                for (int x = 0; x < L.Count; x++)
                {
                    bool s = Respon.ReposeHttp(L[x]);
                    string Msg = s == true ? "补充上传成功" : "补充上传失败";
                    if (!s)
                    {
                        Task.Run(() => { Errlog(L[x]); });
                    }
                    Console.WriteLine(Msg);
                }
            }
            catch (Exception ex)
            { 
            }
        }
        static void EventSubscribe()
        {
            string url = ConFig["url"].ToString();
            string username = ConFig["username"].ToString();
            string workstation = ConFig["workstation"].ToString();
            HubConnection conn = new HubConnection(url);
            IHubProxy EventSrvProxy = conn.CreateHubProxy("PWEventService");
            EventSrvProxy["userName"] = username;
            EventSrvProxy["wrkstName"] = workstation;
            EventSrvProxy.On<PwEvent>("onProwatchEvent", OnProwatchEvent);
            conn.Start().Wait();
            conn.Closed += async () =>
            {
                Console.WriteLine("Closed...");
                System.Console.ReadLine();
                //等待3s后重新创建连接
                await Task.Delay(3 * 1000);
                Console.WriteLine("reconnect...");
                await conn.Start();
            };
            EventSrvProxy.Invoke("subscribe");
            System.Console.WriteLine("subscribe success.");
            System.Console.ReadLine();
            EventSrvProxy.Invoke("unsubscribe");
            System.Console.WriteLine("unsubscribe success.");
        }

        static void OnProwatchEvent(PwEvent evt)
        {
            string LogType = evt.EventTypeDesc.ToString().Trim();
            Console.WriteLine(LogType);
            string des = LogType == "防反传错误" ? "防反传错误" : string.Empty;
            LogModels Model = new LogModels();
            try
            {
                if (LogType == "本地授权" || LogType == "主机准许" || LogType == "防反传错误")
                {

                    Model.equipmentId = $@"{evt.LogicalDevice.LogDevID}";
                    //extra = string.Empty,
                    Model.equipmentType = "ACCESS_READER";
                    Model.accessType = "QR_CODE";
                    Model.accessToken = $@"{evt.Card.CardNumber}";
                    Model.accessDate = evt.SystemDate.ToString();
                    //wayType = "OUTLET",
                    Model.description = des;
                    //temperature = string.Empty,
                    //mask = string.Empty
                    bool s = Respon.ReposeHttp(Model);
                    string Msg = s == true ? "上传成功" : "上传失败";
                    if (!s)
                    {
                        Task.Run(()=> { Errlog(Model); });

                    }
                    
                    Console.WriteLine(Msg);
                    Console.WriteLine(JsonConvert.SerializeObject(Model));
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private static void Errlog(LogModels Record)
        {
            string Line = $"{JsonConvert.SerializeObject(Record)}\r\n";
            FileHelper.FileAppend(CurrentPath,Line,LogFile);
        }
    }
}
