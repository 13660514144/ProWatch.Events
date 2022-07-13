﻿using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
namespace HelperTools
{
    public class RestApi
    {
        public TimeSpan Ts = new TimeSpan();
        private RestClient _client;

        private RestClient client
        {
            get
            {
                if (_client == null)
                {
                    _client = new RestClient();
                }
                return _client;
            }
        }
        public async Task<RestResponse> SendAsync(RestRequest request, string json = "")
        {
            request.Timeout = 120000;
            var response = await RequestExecuteAsync(request, json);
            //过滤系统错误
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response;
            }
            else
            {
                return response;
            }
        }
        /// <summary>
        /// 发送请求
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="json">请求数据，用于打印</param>
        /// <returns>请求结果</returns>
        public async Task<RestResponse> RequestExecuteAsync(RestRequest request, string json = "")
        {
            var url = client.BaseUrl + request.Resource;

            long startTime = Ts.Timestamp();

            RestResponse response = await client.ExecuteAsync(request) as RestResponse;

            long endTime = Ts.Timestamp();
            long timeSecondSpan = (endTime - startTime);
            

            return response;
        }

        /// <summary>
        /// 创建请求对象
        /// </summary>
        /// <param name="partUrl"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public RestRequest CreateRequest(string partUrl, Method method, 
            Dictionary<string, object> Body=null, bool isHasBaseHeader=false)
        {            
            var request = new RestRequest(partUrl, method);
            request.RequestFormat = DataFormat.None;
            request.AlwaysMultipartFormData = true;
            /*if (isHasBaseHeader && BaseHeaders != null && BaseHeaders.Count > 0)
            {
                request.AddHeaders(BaseHeaders);
            }
            if (appendHeaders != null && appendHeaders.Count > 0)
            {
                request.AddHeaders(appendHeaders);
            }*/
            return request;
        }
    }
}
