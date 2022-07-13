using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using HelperTools;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using EmbeddedWebBrowserSolution.Code.Class;

namespace ProWatch
{
    public class Respon
    {
      
        public static dynamic BuildRespon(int Code,string Message,object Data)
        {
            var Obj = new { 
                code=Code,
                message=Message,
                data=Data
            };
            return Obj;
        }
        
        public static bool ReposeHttp(LogModels Log)
        {
            bool Flg = false;
            string ReqUrl = Program.PasslogUrl;
            string Result = string.Empty;  
            HttpFactoryOwner Hreq = new HttpFactoryOwner();
            try
            {
                var parameters = new Dictionary<string, object>();
                parameters.Add("equipmentId", Log.equipmentId);
                //parameters.Add("extra", string.Empty);
                parameters.Add("equipmentType", Log.equipmentType);
                parameters.Add("accessType", Log.accessType);
                parameters.Add("accessToken", Log.accessToken);
                parameters.Add("accessDate", Log.accessDate.Replace("T", " ").Replace("/", "-"));
                /*if ((bool)Dt.Rows[x]["bReaderOut"] == false)
                {
                    parameters.Add("wayType", "INLET");
                }
                else
                {
                    parameters.Add("wayType", "OUTLET");
                }*/
                parameters.Add("description", Log.description);
                //parameters.Add("temperature", string.Empty);
                //parameters.Add("mask", string.Empty);

                var Obj = Hreq.HttpWebRequestPost(ReqUrl, parameters).Result;
                JObject json = JObject.Parse(Obj.Content);
               
                Flg = true;
            }
            catch (Exception ex)
            {
               
            }

            return Flg;
        }
    }
}
