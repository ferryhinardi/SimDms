using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using SimDms.Common.Models;
using System.Net;
using System.Xml;
using System.IO;

namespace SimDms.Common
{
    /// <summary>
    /// SimDmsDcsAdapter is using for communicate between SimDms to DCS Web Service
    /// </summary>
    public class SimDmsDcsAdapter
    {
        private string WsUrl;
        public SimDmsDcsAdapter(string ParamID)
        {
            //DataContext ctx = new DataContext();
            //var sysParam = ctx.SysParameters.Find(ParamID);
            DbContext ctx = new DbContext(MyHelpers.GetConnString("DataContext"));
            var sysParams = ctx.Database.SqlQuery<string>("Select ParamValue from SysParameter where ParamID='" + ParamID + "'").FirstOrDefault();
            this.WsUrl = sysParams;
        }

        public bool IsWsOnline()
        {
            WebRequest request = WebRequest.Create(WsUrl);
            HttpWebResponse ping = (HttpWebResponse)request.GetResponse();
            if (ping == null || ping.StatusCode != HttpStatusCode.OK)
                return false;
            else
                return true;
        }

        /// <summary>
        /// This function 
        /// </summary>
        /// <param name="WsAction">Web Service Action Name</param>
        /// <param name="WsParams">Contains Key(Web Service Parameter Name), Value(Web Service Parameter Value)
        ///                        for provide SOAP Request Message to Web Service</param>
        /// <returns>
        ///     Return list value from Web Service Response
        /// </returns>
        public List<string> InvokeWsData(string WsAction, Dictionary<string, string> WsParams)
        {
            List<string> ResponseValue = new List<string>();
            string action = "http://tempuri.org/" + WsAction;
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(WsUrl);
            webRequest.Headers.Add("SOAPAction", action);
            webRequest.ContentType = "text/xml; charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            XmlDocument SoapEnvelope = CreateSoapMessage(WsAction, WsParams);
            using (Stream stream = webRequest.GetRequestStream())
            {
                SoapEnvelope.Save(stream);
            }

            // begin async call to web request.
            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

            // suspend this thread until call is complete. You might want to
            // do something usefull here like update your UI.
            asyncResult.AsyncWaitHandle.WaitOne();

            // get the response from the completed web request.
            string soapResult;
            XmlDocument SoapResponse = new XmlDocument();
            using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
            {
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    soapResult = rd.ReadToEnd();
                }
                //Console.Write(soapResult);
                if(!string.IsNullOrEmpty(soapResult))
                {
                    SoapResponse.LoadXml(soapResult);
                }
            }
            string selectNode = "string";
            XmlNodeList NodeList = SoapResponse.GetElementsByTagName(selectNode);
            foreach (XmlNode Node in NodeList)
            {
                ResponseValue.Add(Node.InnerXml);
            }
            return ResponseValue;
        }

        /// <summary>
        /// Build XML Web Service Soap Message Request
        /// </summary>
        /// <param name="WsAction">Web Service Action Name</param>
        /// <param name="WsParams">Contains Key(Web Service Parameter Name), Value(Web Service Parameter Value)
        ///                        for provide SOAP Request Message to Web Service</param>
        /// <returns>
        /// Return Xml Document Soap Message
        /// </returns>
        public XmlDocument CreateSoapMessage(string WsAction, Dictionary<string, string> WsParams)
        {
            XmlDocument soapMeseg = new XmlDocument();

            //Soap Envelop
            XmlElement soapEnvelop = soapMeseg.CreateElement("soap", "Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
            soapEnvelop.SetAttribute("xmlns:xsd", "http://www.w3.org/2001/XMLSchema");
            soapEnvelop.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");

            //Soap Body
            XmlElement soapBody = soapMeseg.CreateElement("soap", "Body", "http://schemas.xmlsoap.org/soap/envelope/");
            soapEnvelop.AppendChild(soapBody);

            //Soap WsAction
            XmlElement soapWsAction = soapMeseg.CreateElement(WsAction);
            soapWsAction.SetAttribute("xmlns", "http://tempuri.org/");

            foreach (var Node in WsParams)
            {
                XmlElement NodeAction = soapMeseg.CreateElement(Node.Key);
                NodeAction.AppendChild(soapMeseg.CreateTextNode(Node.Value));
                soapWsAction.AppendChild(NodeAction);
            }

            soapBody.AppendChild(soapWsAction);
            soapMeseg.AppendChild(soapEnvelop);
            //only for testing to find out the context of xml
            //soapMeseg.Save(@"D:\\Text.xml");
            return soapMeseg;
        }
    }
}
