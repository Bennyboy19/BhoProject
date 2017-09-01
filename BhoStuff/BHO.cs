using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SHDocVw;
using mshtml;
using System.IO;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using Newtonsoft.Json;

namespace BhoStuff
{
    [
         ComVisible(true),
         Guid("2159CB25-EF9A-54C1-B43C-E30D1A4A8277"),
         ClassInterface(ClassInterfaceType.None)
    ]
    public class BHO : IObjectWithSite
    {
        private WebBrowser webBrowser;
        string urlLink;





        public int SetSite(object site)
        {
            if (site != null)
            {
                webBrowser = (WebBrowser)site;
                webBrowser.DocumentComplete +=
                  new DWebBrowserEvents2_DocumentCompleteEventHandler(
                  this.OnDocumentComplete);
            }
            else
            {
                webBrowser.DocumentComplete -=
                  new DWebBrowserEvents2_DocumentCompleteEventHandler(
                  this.OnDocumentComplete);
                webBrowser = null;
            }

            return 0;


        }

        public int GetSite(ref Guid guid, out IntPtr ppvSite)
        {
            IntPtr punk = Marshal.GetIUnknownForObject(webBrowser);
            int hr = Marshal.QueryInterface(punk, ref guid, out ppvSite);
            Marshal.Release(punk);
            return hr;
        }


        public void OnDocumentComplete(object pDisp, ref object URL)
        {
            
            const string myUhc = "https://www.myuhc.com/member/prewelcome.do?currentLanguageFromPreCheck=en";
            const string gnc = "https://www.gnc.com/login?original=%2Faccount";
            const string wngstp = "https://order.wingstop.com/user/login?url=%2F";
            string apppId = null;
            HTMLDocument document = (HTMLDocument)webBrowser.Document;
            if (document.url == myUhc)
            {
                apppId = "1";
            }
            else if (document.url == gnc)
            {
                apppId = "2";
            }
            else if (document.url == wngstp)
            {
                apppId = "3";
            }
            
            HttpClient client;
            string url = "http://localhost:52566";
            client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync("api/Tables/" + apppId).Result;
            
            if (response.IsSuccessStatusCode)
            {
                // Calls Web API information and 
                string resp = response.Content.ReadAsStringAsync().Result;
                LoginInfo lgIn = JsonConvert.DeserializeObject<LoginInfo>(resp);
                // Checks to see which website we're currently visiting
                if (myUhc == lgIn.url)
                {
                    IHTMLInputElement userName = (IHTMLInputElement)document.getElementById(lgIn.userIdPattern);
                    userName.defaultValue = lgIn.userId;
                    IHTMLInputElement password = (IHTMLInputElement)document.getElementById(lgIn.passwordPattern);
                    password.defaultValue = lgIn.password;
                    var links = document.getElementsByTagName("a");
                    foreach (IHTMLElement link in links)
                    {
                        if (link.getAttribute("className") == lgIn.submitPattern && link.innerText == "Login")
                        {
                            link.click();
                        }

                    }
                }
                else if (gnc == lgIn.url)
                {
                    IHTMLInputElement userName = (IHTMLInputElement)document.getElementById(lgIn.userIdPattern);
                    userName.defaultValue = lgIn.userId;
                    IHTMLInputElement password = (IHTMLInputElement)document.getElementById(lgIn.passwordPattern);
                    password.defaultValue = lgIn.password;

                    var links = document.getElementsByTagName("button");
                    foreach (IHTMLElement link in links)
                    {
                        if (link.getAttribute("type").Equals(lgIn.submitPattern))
                        {
                            link.click();
                        }
                    }
                }
                else if (wngstp == lgIn.url)
                {
                    IHTMLInputElement userName = (IHTMLInputElement)document.getElementById(lgIn.userIdPattern);
                    userName.defaultValue = lgIn.userId;
                    IHTMLInputElement password = (IHTMLInputElement)document.getElementById(lgIn.passwordPattern);
                    password.defaultValue = lgIn.password;
                    IHTMLElement clicky = document.getElementById(lgIn.submitPattern);
                    clicky.click();
                }

            }


           
        }

        public const string BHO_REGISTRY_KEY_NAME =
   "Software\\Microsoft\\Windows\\" +
   "CurrentVersion\\Explorer\\Browser Helper Objects";

        [ComRegisterFunction]
        public static void RegisterBHO(Type type)
        {
            RegistryKey registryKey =
              Registry.LocalMachine.OpenSubKey(BHO_REGISTRY_KEY_NAME, true);

            if (registryKey == null)
                registryKey = Registry.LocalMachine.CreateSubKey(
                                        BHO_REGISTRY_KEY_NAME);

            string guid = type.GUID.ToString("B");
            RegistryKey ourKey = registryKey.OpenSubKey(guid);

            if (ourKey == null)
            {
                ourKey = registryKey.CreateSubKey(guid);
            }

            ourKey.SetValue("NoExplorer", 1, RegistryValueKind.DWord);

            registryKey.Close();
            ourKey.Close();
        }

        [ComUnregisterFunction]
        public static void UnregisterBHO(Type type)
        {
            RegistryKey registryKey =
              Registry.LocalMachine.OpenSubKey(BHO_REGISTRY_KEY_NAME, true);
            string guid = type.GUID.ToString("B");

            if (registryKey != null)
                registryKey.DeleteSubKey(guid, false);
        }


    }
}