using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RESTClientApp
{
    public class RestHelper
    {
        public static string formdigest = "";
        public int GetIdOfUser(string req, string digesturl, string userlogin)
        {
            XmlNamespaceManager xmlnspm = new XmlNamespaceManager(new NameTable());
            xmlnspm.AddNamespace("atom", "http://www.w3.org/2005/Atom");
            xmlnspm.AddNamespace("d", "http://schemas.microsoft.com/ado/2007/08/dataservices");
            xmlnspm.AddNamespace("m", "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata");

            HttpWebRequest contextinfoRequest = (HttpWebRequest)HttpWebRequest.Create(digesturl);
            contextinfoRequest.Method = "POST";
            contextinfoRequest.Proxy = new WebProxy();
            contextinfoRequest.ContentType = "text/xml;charset=utf-8";
            contextinfoRequest.ContentLength = 0;
            contextinfoRequest.Credentials = CredentialCache.DefaultCredentials;

            HttpWebResponse contextinfoResponse = (HttpWebResponse)contextinfoRequest.GetResponse();
            StreamReader contextinfoReader = new StreamReader(contextinfoResponse.GetResponseStream(), System.Text.Encoding.UTF8);
            var formDigestXML = new XmlDocument();
            formDigestXML.LoadXml(contextinfoReader.ReadToEnd());
            var formDigestNode = formDigestXML.SelectSingleNode("//d:FormDigestValue", xmlnspm);
            string formDigest = formDigestNode.InnerXml;
            formdigest = formDigest;

            HttpWebRequest itemRequest = (HttpWebRequest)HttpWebRequest.Create(req);
            itemRequest.Method = "GET";
            itemRequest.ContentType = "application/json;odata=verbose";
            itemRequest.Proxy = new WebProxy();
            itemRequest.Accept = "application/json;odata=verbose";
            itemRequest.Credentials = CredentialCache.DefaultCredentials;
            itemRequest.Headers.Add("X-RequestDigest", formDigest);
            JObject o = null;
            int idofuser = 0;
            try
            {
                WebResponse webResponse = itemRequest.GetResponse();
                Stream webStream = webResponse.GetResponseStream();
                StreamReader responseReader = new StreamReader(webStream);
                string response = responseReader.ReadToEnd();
                o = JObject.Parse(response);
                idofuser = Int32.Parse(o["d"]["Id"].ToString());
            }
            catch (Exception)
            {
                idofuser = 0;
            }

            return idofuser;
        }

        public void AddGroup(string data, string digesturl, string requesturl)
        {
            XmlNamespaceManager xmlnspm = new XmlNamespaceManager(new NameTable());
            xmlnspm.AddNamespace("atom", "http://www.w3.org/2005/Atom");
            xmlnspm.AddNamespace("d", "http://schemas.microsoft.com/ado/2007/08/dataservices");
            xmlnspm.AddNamespace("m", "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata");

            HttpWebRequest contextinfoRequest = (HttpWebRequest)HttpWebRequest.Create(digesturl);
            contextinfoRequest.Method = "POST";
            contextinfoRequest.Proxy = new WebProxy();
            contextinfoRequest.ContentType = "text/xml;charset=utf-8";
            contextinfoRequest.ContentLength = 0;
            contextinfoRequest.Credentials = CredentialCache.DefaultCredentials;

            HttpWebResponse contextinfoResponse = (HttpWebResponse)contextinfoRequest.GetResponse();
            StreamReader contextinfoReader = new StreamReader(contextinfoResponse.GetResponseStream(), System.Text.Encoding.UTF8);
            var formDigestXML = new XmlDocument();
            formDigestXML.LoadXml(contextinfoReader.ReadToEnd());
            var formDigestNode = formDigestXML.SelectSingleNode("//d:FormDigestValue", xmlnspm);
            string formDigest = formDigestNode.InnerXml;

            string itemPostBody = data;
            Encoding utf8NoBom = new UTF8Encoding(false);
            Byte[] itemPostData = utf8NoBom.GetBytes(itemPostBody);
            //string a = utf8NoBom.GetString(itemPostData);
            var datak = utf8NoBom.GetString(itemPostData);
            HttpWebRequest itemRequest = (HttpWebRequest)HttpWebRequest.Create(requesturl);
            itemRequest.Method = "POST";
            //itemRequest.Proxy = new WebProxy("127.0.0.1", 8888);
            //itemRequest.ContentLength = itemPostBody.Length;
            itemRequest.ContentLength = utf8NoBom.GetByteCount(datak);
            itemRequest.ContentType = "application/json;odata=verbose";
            itemRequest.MaximumResponseHeadersLength = -1;
            itemRequest.Proxy = new WebProxy();
            itemRequest.Accept = "application/json;odata=verbose";
            itemRequest.Credentials = CredentialCache.DefaultCredentials;
            itemRequest.Headers.Add("X-RequestDigest", formDigest);

            Stream itemRequestStream = itemRequest.GetRequestStream();
            itemRequestStream.Write(itemPostData, 0, itemPostData.Length);
            itemRequestStream.Close();

            try
            {
                HttpWebResponse itemResponse = (HttpWebResponse)itemRequest.GetResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static string GetIdOfList(string req, string digesturl)
        {
            XmlNamespaceManager xmlnspm = new XmlNamespaceManager(new NameTable());
            xmlnspm.AddNamespace("atom", "http://www.w3.org/2005/Atom");
            xmlnspm.AddNamespace("d", "http://schemas.microsoft.com/ado/2007/08/dataservices");
            xmlnspm.AddNamespace("m", "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata");

            HttpWebRequest contextinfoRequest = (HttpWebRequest)HttpWebRequest.Create(digesturl);
            contextinfoRequest.Method = "POST";
            contextinfoRequest.Proxy = new WebProxy();
            contextinfoRequest.ContentType = "text/xml;charset=utf-8";
            contextinfoRequest.ContentLength = 0;
            contextinfoRequest.Credentials = CredentialCache.DefaultCredentials;

            HttpWebResponse contextinfoResponse = (HttpWebResponse)contextinfoRequest.GetResponse();
            StreamReader contextinfoReader = new StreamReader(contextinfoResponse.GetResponseStream(), System.Text.Encoding.UTF8);
            var formDigestXML = new XmlDocument();
            formDigestXML.LoadXml(contextinfoReader.ReadToEnd());
            var formDigestNode = formDigestXML.SelectSingleNode("//d:FormDigestValue", xmlnspm);
            string formDigest = formDigestNode.InnerXml;
            formdigest = formDigest;

            HttpWebRequest listRequest = (HttpWebRequest)HttpWebRequest.Create(req);
            listRequest.Method = "GET";
            listRequest.Proxy = new WebProxy();
            listRequest.Accept = "application/atom+xml";
            listRequest.ContentType = "application/atom+xml;type=entry";
            listRequest.Credentials = CredentialCache.DefaultCredentials;
            HttpWebResponse listResponse = (HttpWebResponse)listRequest.GetResponse();
            StreamReader listReader = new StreamReader(listResponse.GetResponseStream());
            var listXml = new XmlDocument();
            listXml.LoadXml(listReader.ReadToEnd());
            
            var listguid = listXml.SelectSingleNode("//atom:feed//atom:entry/atom:id", xmlnspm);
           
            return listguid.InnerXml;
        }

        public static void RestYap(string data, string digesturl, string requesturl, string listrequesturl, bool ifmatch, bool merge, bool delete, bool put)
        {
            XmlNamespaceManager xmlnspm = new XmlNamespaceManager(new NameTable());
            xmlnspm.AddNamespace("atom", "http://www.w3.org/2005/Atom");
            xmlnspm.AddNamespace("d", "http://schemas.microsoft.com/ado/2007/08/dataservices");
            xmlnspm.AddNamespace("m", "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata");

            HttpWebRequest contextinfoRequest = (HttpWebRequest)HttpWebRequest.Create(digesturl);
            contextinfoRequest.Method = "POST";
            contextinfoRequest.Proxy = new WebProxy();
            contextinfoRequest.ContentType = "text/xml;charset=utf-8";
            contextinfoRequest.ContentLength = 0;
            contextinfoRequest.Credentials = CredentialCache.DefaultCredentials;

            HttpWebResponse contextinfoResponse = (HttpWebResponse)contextinfoRequest.GetResponse();
            StreamReader contextinfoReader = new StreamReader(contextinfoResponse.GetResponseStream(), System.Text.Encoding.UTF8);
            var formDigestXML = new XmlDocument();
            formDigestXML.LoadXml(contextinfoReader.ReadToEnd());
            var formDigestNode = formDigestXML.SelectSingleNode("//d:FormDigestValue", xmlnspm);
            string formDigest = formDigestNode.InnerXml;

            HttpWebRequest listRequest = (HttpWebRequest)HttpWebRequest.Create(listrequesturl);
            listRequest.Method = "GET";
            listRequest.Proxy = new WebProxy();
            listRequest.Accept = "application/atom+xml";
            listRequest.ContentType = "application/atom+xml;type=entry";
            listRequest.Credentials = CredentialCache.DefaultCredentials;
            HttpWebResponse listResponse = (HttpWebResponse)listRequest.GetResponse();
            StreamReader listReader = new StreamReader(listResponse.GetResponseStream());
            var listXml = new XmlDocument();
            listXml.LoadXml(listReader.ReadToEnd());

            var entityTypeNode = listXml.SelectSingleNode("//atom:entry/atom:content/m:properties/d:ListItemEntityTypeFullName", xmlnspm);
            var listNameNode = listXml.SelectSingleNode("//atom:entry/atom:content/m:properties/d:Title", xmlnspm);

            string entityTypeName = entityTypeNode.InnerXml;
            string listName = listNameNode.InnerXml;

            string itemPostBody = data;
            Encoding utf8NoBom = new UTF8Encoding(false);
            Byte[] itemPostData = utf8NoBom.GetBytes(itemPostBody);
            var datak = utf8NoBom.GetString(itemPostData);
            HttpWebRequest itemRequest = (HttpWebRequest)HttpWebRequest.Create(requesturl);
            itemRequest.Method = "POST";
            //itemRequest.Proxy = new WebProxy("127.0.0.1", 8888); İsteğinizin Fiddler’a düşmesini sağlamak için local makinenizin 8888 portunu kullanmalısınız.
            //itemRequest.ContentLength = itemPostBody.Length;
            itemRequest.ContentLength = utf8NoBom.GetByteCount(datak);
            itemRequest.ContentType = "application/json;odata=verbose";
            itemRequest.MaximumResponseHeadersLength = -1;
            itemRequest.Proxy = new WebProxy();
            itemRequest.Accept = "application/json;odata=verbose";
            itemRequest.Credentials = CredentialCache.DefaultCredentials;
            itemRequest.Headers.Add("X-RequestDigest", formDigest);

            if (merge)
            {
                itemRequest.Headers.Add("X-HTTP-Method", "PATCH");
            }
            if (ifmatch)
            {
                itemRequest.Headers.Add("IF-MATCH", "*");
            }
            if (delete)
            {
                itemRequest.Headers.Add("X-HTTP-Method", "DELETE");
            }

            if (put)
            {
                itemRequest.Headers.Add("X-HTTP-Method", "PUT");
            }
            /// karşılaştırma file upload da if match ve delete true ise  yeni oluşturulacak  aksi taktirde üzerine yazıcak
            if (ifmatch && delete)
            {
                itemRequest.ContentLength = 0;
            }
            else
            {
                Stream itemRequestStream = itemRequest.GetRequestStream();
                itemRequestStream.Write(itemPostData, 0, itemPostData.Length);
                itemRequestStream.Close();
            }
        }

    }
}
