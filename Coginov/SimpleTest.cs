using Coginov.CoginovService;
using Microsoft.Samples.CustomTextMessageEncoder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Newtonsoft.Json;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using Coginov.Objects;

namespace Coginov
{
    public class FormDigest
    {
        public string FormDigestValue { get; set; }
    }

    public partial class SimpleTest : Form
    {
        public SimpleTest()
        {
            InitializeComponent();
        }
        private void btnTest_Click(object sender, EventArgs e)
        {
            List<MetaListInfo> metaLists = AnalyseCollectionSite("http://addins.metadev.local/sites/semantic/");
        }

        /// <summary>
        /// Obtenir les informations sémantiques des fichiers de la collection de site en paramètre.
        /// </summary>
        /// <param name="siteCollectionURL"></param>
        /// <returns>Retourne une liste des informations de bibliothèque SharePoint après l'analyse sémantique de leurs fichiers</returns>
        static public List<MetaListInfo> AnalyseCollectionSite(string siteCollectionURL)
        {
            List<MetaListInfo> metaLists = getMetaLists(siteCollectionURL);
            foreach (MetaListInfo metaListInfo in metaLists)
            {
                try
                {
                    metaListInfo.MetaFilesInfo = getMetaFiles(metaListInfo, siteCollectionURL);

                    foreach (MetaFileInfo metaFileInfo in metaListInfo.MetaFilesInfo)
                    {
                        metaFileInfo.ParentMetaListInfo = metaListInfo;
                        metaFileInfo.LocalAbsUrl = downloadFile(metaFileInfo);
                    }
                }
                catch (Exception err)
                {
                    // Écrire dans un fichier log.txt l'erreur, heure;
                }
            }
            for (int i = 0; i < metaLists.Count(); i++)
            {
                for (int j = 0; j < metaLists[i].MetaFilesInfo.Count(); j++)
                {
                    /* Coginov supporte seulement UTF-8*/
                    metaLists[i].MetaFilesInfo[j] = analyseFile(metaLists[i].MetaFilesInfo[j]);
                }
            }
            return metaLists;
        }

        /// <summary>
        /// Envoyer le texte du document à aux services d'analyse de Coginov
        /// </summary>
        /// <param name="metaFile"></param>
        /// <returns>retourne une copie du MetaFileInfo avec les listes des colonnes SP peuplés </returns>
        static private MetaFileInfo analyseFile(MetaFileInfo metaFile)
        {
            /*listElements.Items.Add("--=- FICHIER: " + metaFile.getFileName() + "-=---");*/
            using (StreamReader sr = new StreamReader(metaFile.LocalAbsUrl))
            {
                metaFile = getServicesWords(sr.ReadToEnd(),metaFile);
                /*foreach (var service in services){
                    listElements.Items.Add("== Service:" + service.Key);
                    foreach (var word in service.Value) listElements.Items.Add(word);
                }*/
            }
            return metaFile;
        }

        static private void genericHeaders(ref HttpClient client, string siteCollectionURL)
        {
            client.BaseAddress = new System.Uri(siteCollectionURL);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");
            client.DefaultRequestHeaders.Add("ContentType", "application/json");
            client.DefaultRequestHeaders.Add("X-RequestDigest", getDigestOfContextInfo(siteCollectionURL));
        }

        static private string getDigestOfContextInfo(string siteCollectionURL)
        {
            try{
                string url = siteCollectionURL;
                string cmd = "_api/contextinfo";
                HttpClient client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true });
                client.BaseAddress = new System.Uri(url);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");
                client.DefaultRequestHeaders.Add("ContentType", "application/json");
                client.DefaultRequestHeaders.Add("ContentLength", "0");
                var response = client.GetAsync(cmd).Result;
                if (response.IsSuccessStatusCode)
                {
                    string content = response.Content.ReadAsStringAsync().Result;
                    JObject jobj = JObject.Parse(content);
                    JObject context = (JObject)jobj["d"]["GetContextWebInformation"];
                    JValue FDValue = (JValue)context["FormDigestValue"];
                    return FDValue.Value.ToString();
                }
            }
            catch (Exception err){
            }
            return "";
        }
        /// <summary>
        /// Récupérer l'informations des listes SharePoint de la collection de site passée en paramètre.
        /// </summary>
        /// <param name="siteCollectionURL"></param>
        /// <returns>Liste de MetaListInfo</returns>
        static private List<MetaListInfo> getMetaLists(string siteCollectionURL)
        {
            List<MetaListInfo> metaLists = new List<MetaListInfo>();
            try{
                HttpClient client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true });
                string cmd = "_api/web/lists?$select=Id,Title,BaseTemplate,baseType,LastItemDeletedDate,LastItemModifiedDate";
                string filter = "$filter= (BaseTemplate eq 101) and (BaseType eq 1)";
                genericHeaders(ref client, siteCollectionURL);
                var response = client.GetAsync(cmd + '&' + filter).Result;
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    string content = response.Content.ReadAsStringAsync().Result;
                    JObject jobj = JObject.Parse(content);
                    JArray array = (JArray)jobj["d"]["results"];

                    for (int i = 0; i < array.Count; i++)
                    {
                        metaLists.Add(array[i].ToObject<MetaListInfo>());
                    }
                }
            }
            catch (Exception err){
                throw new Exception("Une exception s'est produite durant la tentative de récupération des informations des listes. | Erreur: " + err.Message);
            }
            return metaLists;
        }
        /// <summary>
        /// Récupérer l'informations des fichiers SharePoint de la liste passée en paramètre
        /// </summary>
        /// <param name="metaList"></param>
        /// <param name="siteCollectionURL"></param>
        /// <returns>Liste de MetaFileInfo</returns>
        static private List<MetaFileInfo> getMetaFiles(MetaListInfo metaList, string siteCollectionURL)
        {
            List<MetaFileInfo> metaFiles = new List<MetaFileInfo>();
            try{
                HttpClient client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true });
                string cmd = "_api/web/lists/GetById('" + metaList.Id + "')/Items?$select=Id,EncodedAbsUrl,Title";
                genericHeaders(ref client, siteCollectionURL);
                var response = client.GetAsync(cmd).Result;
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    string content = response.Content.ReadAsStringAsync().Result;
                    JObject jobj = JObject.Parse(content);
                    JArray array = (JArray)jobj["d"]["results"];

                    for (int i = 0; i < array.Count; i++)
                    {
                        metaFiles.Add(array[i].ToObject<MetaFileInfo>());
                    }
                }
            }
            catch (Exception err){
                throw new Exception("Une exception s'est produite durant la tentative de récupération des informations des fichiers. | Erreur: " + err.Message);
            }
            return metaFiles;
        }

        static private string downloadFile(MetaFileInfo metaFile)
        {
            string localAbsUrl = "";
            try{
                System.Net.WebClient Client = new System.Net.WebClient();
                Client.UseDefaultCredentials = true;
                localAbsUrl = @"C:\Users\gabriel.gagnon\Desktop\FichiersSharePoint\files\" + metaFile.getFileName();
                Client.DownloadFile(metaFile.EncodedAbsUrl, localAbsUrl);
            }
            catch (Exception err){
                throw new Exception("Une exception s'est produite durant la tentative de téléchargement du fichier: " + metaFile.EncodedAbsUrl + " | Erreur: " + err.Message);
            }
            return localAbsUrl;

        }

        static private MetaFileInfo getServicesWords(string document, MetaFileInfo metafile)
        {
            CustomBinding binding = new CustomBinding(new BindingElement[] {
                new CustomTextMessageBindingElement("Windows-1252"),
                new HttpTransportBindingElement() });
            EndpointAddress ep = new EndpointAddress("http://api.coginov.com:8282/coginovapiservice.soap");
            coginovapiClient cli = new coginovapiClient(binding, ep);
            var reponse = cli.coginovApi(new coginovApiRequest()
            {
                authKey = "123456",
                document = new coginovApiRequestDocument()
                {
                    source = document,
                    type = coginovApiRequestDocumentType.DOCUMENT
                },
                serviceCall = new coginovApiRequestServiceCall[] { new coginovApiRequestServiceCall() { serviceName = "ExtConcept" },
                                                                new coginovApiRequestServiceCall() { serviceName = "ExtCategory" },
                                                                new coginovApiRequestServiceCall() { serviceName = "ExtEntity" }
                                                                }
            });

            Dictionary<string, List<string>> services = new Dictionary<string, List<string>>();

            var concepts = reponse.serviceResult.Where(r => r.Any.Any(n => n.Name == "ExtConcepts")).Single().Any.SingleOrDefault();
            var category = reponse.serviceResult.Where(r => r.Any.Any(n => n.Name == "extcategory")).Single().Any.SingleOrDefault();
            var entities = reponse.serviceResult.Where(r => r.Any.Any(n => n.Name == "ExtEntities")).Single().Any.SingleOrDefault();

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(concepts.OwnerDocument.NameTable);
            nsmgr.AddNamespace("co", concepts.NamespaceURI);

            XmlNodeList conceptsnodes = concepts.SelectNodes("/co:ExtConcept/co:XConcept", nsmgr);
            XmlNodeList categorynodes = category.SelectNodes("/co:category/co:categoryname", nsmgr);
            XmlNodeList entitiesnodes = entities.SelectNodes("/co:ExtEntity/co:Entity", nsmgr);
            metafile.Concepts = new List<string>();
            metafile.Category = new List<string>();
            metafile.Entities = new List<string>();
            foreach (var node in conceptsnodes) metafile.Concepts.Add(((System.Xml.XmlElement)node).InnerText);
            foreach (var node in categorynodes) metafile.Category.Add(((System.Xml.XmlElement)node).InnerText);
            foreach (var node in entitiesnodes) metafile.Entities.Add(((System.Xml.XmlElement)node).InnerText);

            return metafile;
        }
    }
}