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
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Test
{
    static class test
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            //project2:  read json file input, return key-vaule pairs  
            //To run it, please edit the input params according to your request
            string configPath = "C:\\Users\\Administrator\\Desktop\\capture_config.json";
            string imagePath = "C:\\Users\\Administrator\\Desktop\\images_BACA\\car5.pdf";

            var res = new MetaBot.SyncCapture();
            string result = res.MainJson(configPath, imagePath);
        }
    }
}
