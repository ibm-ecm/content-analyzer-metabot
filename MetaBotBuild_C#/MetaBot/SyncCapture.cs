/*disclaimer of warranties.
 this code is sample code created by ibm corporation. ibm grants you a
 nonexclusive copyright license to use this sample code example. this
 sample code is not part of any standard ibm product and is provided to you
 solely for the purpose of assisting you in the development of your
 applications. this example has not been thoroughly tested under all
 conditions. ibm, therefore cannot guarantee nor may you imply reliability,
 serviceability, or function of these programs. the code is provided "as is",
 without warranty of any kind. ibm shall not be liable for any damages
 arising out of your or any other parties use of the sample code, even if ibm
 has been advised of the possibility of such damages. if you do not agree with
 these terms, do not use the sample code.
 copyright ibm corp. 2019 all rights reserved.
 to run, see readme.md
 * */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using System.Security.Cryptography;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MetaBot
{
    public class SyncCapture
    {

        /// <summary>
        ///This project is to return the key-value pairs based on the input of a JSON configuratuin file          
        ///Input params in JSON configuration file (example):
        ///{
        ///"functionalID": "",
        ///"password": "",
        ///"apiKey": "",
        ///"mainURL": "",
        ///"responseType": "\"utf8\",\"json\"",
        ///"jsonOptions": "\"ocr\",\"dc\",\"kvp\",\"sn\"",
        ///"fields":["Pickup_Date","Pickup_Location"]
        ///}
        ///</summary>       

        class Config
        {
            public string functionalID { get; set; }
            public string password { get; set; }
            public string apiKey { get; set; }
            public string mainURL { get; set; }
            public string responseType { get; set; }
            public string jsonOptions { get; set; }
            public string[] fields { get; set; }

        }

        class JSONRes
        {
            public string DocumentName {get; set;}
            public object[] KVPTable { get; set; }
        }

        class SessionParams
        {
            public JArray servers { get; set; }
            public string security_token { get; set; }
        }

        class ServerParams
        {
            public string dcAppName { get; set; }
            public string repositoryId { get; set; }
        }

        /// <summary>
        /// start 
        /// </summary>
        /// <param>configFilePath, imagePath</param>
        ///<returns>name-value pairs</returns>
        public string MainJson(string configFilePath, string imagePath)
        {
           ServicePointManager.ServerCertificateValidationCallback +=
       (sender, certificate, chain, sslPolicyErrors) => true;
           ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string result = String.Empty;
            
            Config configParams = new Config();
            string jsonParams = File.ReadAllText(@configFilePath);
            try
            {
                configParams = (JsonConvert.DeserializeObject<Config>(jsonParams));
            }
            catch (JsonReaderException err) {
                return "jsonParams: " + err.ToString();
            }


            //=============Step 1: submit the file for processing and get the analyzerId=========

            IRestResponse responseSubmit = submitFile(configParams, imagePath);
            string analyzerId = String.Empty;
            JObject responseJSON = new JObject();
            if (responseSubmit.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                
                try
                {
                    responseJSON = JObject.Parse(responseSubmit.Content);
                }
                catch (JsonReaderException err)
                {
                    return "submitRes: " + err.ToString();
                }
                var analyzer = responseJSON["data"]["analyzerId"];
                analyzerId = analyzer.ToString();
            }
            else
            {
                try
                {
                    responseJSON = JObject.Parse(responseSubmit.Content);
                }
                catch (JsonReaderException err)
                {
                    return "submitResBad: " + err.ToString();
                }
                JObject errorJSON = new JObject();
                errorJSON["errors"] = responseJSON["errors"];
                string error1 = errorJSON.ToString();
                return error1;
            }

            //=============Step 2: retrieve the processing status based on the analyzerId=========

            IRestResponse responseStatus = null;
            //string analyzerId = "fa7ecde0-3701-11e9-91d8-f983d5494b7d";
            string statusStr = String.Empty;
            for (int i = 1; i < 10; i++) {
                System.Threading.Thread.Sleep(5000);
                responseStatus = getStatus(configParams, analyzerId);
                JObject res = new JObject();
                if (responseStatus.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    try
                    {
                        res = JObject.Parse(responseStatus.Content);
                    }
                    catch (JsonReaderException err)
                    {
                        return "statusRes: " + err.ToString();
                    }
                    var status = res["data"]["statusDetails"][0]["status"];
                    statusStr = status.ToString();
                    if (statusStr == "Completed")
                    {
                        break;
                    }
                }
                else {
                    try
                    {
                        res = JObject.Parse(responseStatus.Content);
                    }
                    catch (JsonReaderException err)
                    {
                        return "statusResBad:" + err.ToString();
                    }
                
                    JObject errorJSON = new JObject();
                    errorJSON["errors"] = res["errors"];
                    string error2 = errorJSON.ToString();
                    return error2;
                }
            }

            //=============Step 3: Get and filter the JSON output=============

            string jsonStr = String.Empty;
            if (statusStr == "Completed")
            {
                IRestResponse responseOutput = getJson(configParams, analyzerId);
                JObject json = new JObject();
                if (responseOutput.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    JObject resFinal = new JObject();
                    string resStr = String.Empty;
                    string responseStr = responseOutput.Content;
                    try
                    {
                        json = JObject.Parse(responseStr);
                    }
                    catch (JsonReaderException err)
                    {
                        return "jsonRes: " + err.ToString();
                    }

                    resFinal = formatJSON(configParams.fields, json);
                    resStr = resFinal.ToString();

                    //==============step 4: Delete content from db=============

                    //deleteContent(configParams, analyzerId);
                    return resStr;
                }
                else
                {
                    string responseStr = responseOutput.Content;
                    try
                    {
                        json = JObject.Parse(responseStr);
                    }
                    catch (JsonReaderException err)
                    {
                        return "jsonResBad: " + err.ToString();
                    }
                    JObject errorJSON = new JObject();
                    errorJSON["errors"] = json["errors"];
                    string error3 = errorJSON.ToString();
                    return error3;
                }
            }
            return jsonStr;
        }



        /// <summary>
        /// API Function 1: submit the file for processing
        /// </summary>
        /// <param>configParams, imagePath</param>
        ///<returns>POST method response</returns>
        private static IRestResponse submitFile(Config configParams, string imagePath) {
            string functionalID = configParams.functionalID;
            string password = configParams.password;
            string mainURL = configParams.mainURL;
            string apiKey = configParams.apiKey;
            string responseType = configParams.responseType;
            string jsonOptions = configParams.jsonOptions;

            string url = mainURL + "/contentAnalyzer";
            var auth = Base64Encode(functionalID + ":" + password);
            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);
            
            request.AddHeader("Authorization", "Basic " + auth);
            request.AddHeader("apiKey", apiKey);
            request.AddHeader("content-type", "multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW");
            request.AddParameter("responseType", "\"json\", \"pdf\"");
            request.AddParameter("jsonOptions", jsonOptions);
            request.AddFile("file", imagePath);
            
            IRestResponse response = client.Execute(request);
            return response;
        }


        /// <summary>
        /// API Function 2: retrieve the processing status
        /// </summary>
        /// <param>configParams, analyzerId</param>
        ///<returns>GET method response</returns>
        private static IRestResponse getStatus(Config configParams, string analyzerId) {
            string functionalID = configParams.functionalID;
            string password = configParams.password;
            string mainURL = configParams.mainURL;
            string apiKey = configParams.apiKey;

            string url = mainURL + "/contentAnalyzer/" + analyzerId;
            var auth = Base64Encode(functionalID + ":" + password);
            var client = new RestClient(url);
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Basic " + auth);
            request.AddHeader("apiKey", apiKey);
            IRestResponse response = client.Execute(request);

            return response;
        }


        /// <summary>
        /// API Function 3: GETJSON output
        /// </summary>
        /// <param>configParams, analyzerId</param>
        ///<returns>KVP and Classification JSON output</returns>
        private static IRestResponse getJson(Config configParams, string analyzerId)
        {
            string functionalID = configParams.functionalID;
            string password = configParams.password;
            string mainURL = configParams.mainURL;
            string apiKey = configParams.apiKey;
            string[] fields = configParams.fields;

            string url = mainURL + "/contentAnalyzer/" + analyzerId + "/json";
            var auth = Base64Encode(functionalID + ":" + password);
            var client = new RestClient(url);
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Basic " + auth);
            request.AddHeader("apiKey", apiKey);
            
            IRestResponse response = client.Execute(request);
            return response;   
        }


        /// <summary>
        /// format JSON output
        /// </summary>
        /// <param>configParams.fields, JSON output</param>
        ///<returns>formatted JSON result</returns>
        private static JObject formatJSON(Array fields, JObject json) {
            JObject resFinal = new JObject();
            var pageList = json["data"]["pageList"];
            JArray fields_res = new JArray();

            if (fields.Length > 0)
            {
                foreach (var field in fields) {
                    JObject temp = new JObject();
                    temp["key"] = field.ToString();
                    temp["values"] = new JArray();
                    fields_res.Add(temp);
                }
            } else {
                List<string> key = new List<string>();
                foreach (var page in pageList)
                {
                    var KVPTable = page["KVPTable"];
                    foreach (var table in KVPTable)
                    {
                        JObject temp = new JObject();
                        if (table["KeyClass"] != null)
                        {
                            if ((!key.Contains(table["KeyClass"].ToString())) && (table["KeyClass"].ToString() != ""))
                            {
                                key.Add(table["KeyClass"].ToString());
                                JObject temp1 = new JObject();
                                temp1["key"] = table["KeyClass"].ToString();
                                temp1["values"] = new JArray();
                                fields_res.Add(temp1);
                            }
                        }
                    }
                }
                fields = key.ToArray();
            }

            foreach (var page in pageList)
            {
                var KVPTable = page["KVPTable"];
                foreach (JObject table in KVPTable)
                {
                    if (table["KeyClass"] != null) {
                        string key = table["KeyClass"].ToString();
                        int index = Array.IndexOf(fields, key);
                        if (index >= 0)
                        {
                            int pageNum = (int) table["PageNumber"] + 1;
                            JObject value = new JObject();
                            value["value"] = table["Value"];
                            value["pageNum"] = pageNum;
                            JArray temp = new JArray();
                            foreach (var v in fields_res[index]["values"]) {
                                temp.Add(v);
                            }
                            temp.Add(value);
                            fields_res[index]["values"] = temp;
                        }
                    }
                }
            }

            
            resFinal["Fields"] = fields_res;
            resFinal["Classification"] = new JObject();
            resFinal["Classification"]["DocumentClass"] = json["data"]["Classification"]["DocumentClass"]["Actual"];
            resFinal["Classification"]["ClassMatch"] = json["data"]["Classification"]["DocumentClass"]["ClassMatch"];

            return resFinal;
        }

        /// <summary>
        /// API Function 4: DELETE content
        /// </summary>
        /// <param>configParams, analyzerId</param>
        ///<returns>response</returns>
        private static IRestResponse deleteContent(Config configParams, string analyzerId)
        {
            //    ServicePointManager.ServerCertificateValidationCallback +=
            //(sender, certificate, chain, sslPolicyErrors) => true;
            //    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string functionalID = configParams.functionalID;
            string password = configParams.password;
            string mainURL = configParams.mainURL;
            string apiKey = configParams.apiKey;

            string url = mainURL + "/contentAnalyzer/" + analyzerId;
            var auth = Base64Encode(functionalID + ":" + password);
            var client = new RestClient(url);
            var request = new RestRequest(Method.DELETE);
            request.AddHeader("Authorization", "Basic " + auth);
            request.AddHeader("apiKey", apiKey);

            IRestResponse response = client.Execute(request);
            return response;
        }



        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }       
    }
}
