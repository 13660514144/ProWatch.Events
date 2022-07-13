using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmbeddedWebBrowserSolution.Code.Class
{
    public class LogModels
    {
        public string equipmentId { get; set; }
        //public string extra { get; set; } = string.Empty;
        public string equipmentType { get; set; } = "IC";
        public string accessType { get; set; } = "IC_CARD";
        public string accessToken { get; set; }
        public string accessDate { get; set; }
        //public string wayType { get; set; }
        public string description { get; set; } = string.Empty;
        //public string temperature { get; set; } = string.Empty;
        //public string mask { get; set; } = string.Empty;
        
    }
}
