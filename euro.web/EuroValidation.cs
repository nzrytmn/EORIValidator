using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;

namespace euro.web
{
    public class EuroValidation
    {
        public string ValidateEoriNumber(List<string> numberList)
        {
            var result = string.Empty;
            try
            {
                 result = CallWebService(numberList);
            }
            catch (Exception e)
            {
                throw;
            }
            return result;
        }


        private static  string CallWebService(List<string> numberList)
        {
            var _url = "http://ec.europa.eu/taxation_customs/dds2/eos/validation/services/validation?wsdl";

            XmlDocument soapEnvelopeXml = CreateSoapEnvelope(numberList);
            HttpWebRequest webRequest = CreateWebRequest(_url);

            InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);

            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);
            asyncResult.AsyncWaitHandle.WaitOne();

            string soapResult;
            using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
            {
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    soapResult = rd.ReadToEnd();
                }
                return soapResult;
            }
        }

        private static HttpWebRequest CreateWebRequest(string url)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }

        private static XmlDocument CreateSoapEnvelope(List<string> numberList)
        {
            XmlDocument soapEnvelop = new XmlDocument();
            var numberEnvelop = string.Empty;
            foreach (var item in numberList)
            {
                numberEnvelop += string.Format("<ev:eori>{0}</ev:eori>", item);
            }
            var xmlEnvelop = string.Format(@"<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/""> 
            <soap:Body>
            <ev:validateEORI xmlns:ev = ""http://eori.ws.eos.dds.s/"">
                {0}
             </ev:validateEORI>
             </soap:Body>
              </soap:Envelope>", numberEnvelop);
            soapEnvelop.LoadXml(xmlEnvelop);
            return soapEnvelop;
        }

        private static void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
        }
    }
}