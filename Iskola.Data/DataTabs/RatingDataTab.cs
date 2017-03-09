using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iskola.Data.DataTabs
{
    public class RatingDataTab : DataTab
    {
        private MarksTable _marksTable;

        public MarksTable Marks
        {
            get { return _marksTable; }
            set { _marksTable = value; PropertyChanged_Invoke("Marks"); }
        }


        public RatingDataTab(IskolaClient Client) : base(Client) { }

        protected async override Task DownloadData()
        {
            String response = await Client.LoadRequest("https://www.iskola.cz/?cast=Hodnoceni&akce=zak");
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(response);
            Marks = GetMarks(document);
        }

        private Dictionary<long, MarkInfo> _markInfoBuffer = new Dictionary<long, MarkInfo>();
        public async Task<MarkInfo> GetMarkInfo(long ID)
        {
            if (_markInfoBuffer.ContainsKey(ID))
                return _markInfoBuffer[ID];
            String response = await Client.LoadRequest("https://www.iskola.cz/?cast=Hodnoceni&akce=znamka&id=" + ID.ToString());
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(response);
            MarkInfo loadedMarkInfo = GetMarkInfo(document);
            if(loadedMarkInfo!=null)
                _markInfoBuffer.Add(ID, loadedMarkInfo);
            return loadedMarkInfo;
        }
        internal override void LogoutClear()
        {
            if (IsAnythingLoaded)
            {
                _markInfoBuffer.Clear();
                _marksTable.ReleaseAll();
            }
        }
        private static MarksTable GetMarks(HtmlDocument doc)
        {
            IEnumerable<HtmlNode> nodes = doc.DocumentNode.Descendants().Where(node => node.Id == "vypis");
            if (nodes.Count() > 0)
            {
                HtmlNode mainNode = nodes.ElementAt(0).LastChild;
                MarksTable marksTable = new MarksTable();
                foreach (var node in mainNode.ChildNodes)
                {
                    RatedSubject newRatedSubject = new RatedSubject();
                    newRatedSubject.SubjectName = node.FirstChild.InnerText;
                    newRatedSubject.Qualification = node.ChildNodes[2].InnerText;
                    newRatedSubject.Average = node.LastChild.InnerText;
                    HtmlNode marksNode = node.ChildNodes[1];
                    foreach (HtmlNode actualMarkNode in marksNode.Descendants().Where(nd => nd.Name == "a"))
                    {
                        Mark newMark = new Mark();
                        newMark.Value = actualMarkNode.InnerText;
                        String idValue = actualMarkNode.GetAttributeValue("href", "NULL");
                        newMark.ID = Convert.ToInt64(idValue.Split(new String[] { ";id=" }, StringSplitOptions.None)[1]);
                        String titleText = actualMarkNode.GetAttributeValue("title", "NULL").Replace("&#10;", "");
                        String[] properties = titleText.Split(new String[] { "&#13;" }, StringSplitOptions.None);
                        foreach (String property in properties)
                        {
                            String[] splittedProperty = property.Split(':');
                            switch (splittedProperty[0].Trim())
                            {
                                case "Datum zkoušky": newMark.Date = splittedProperty[1].Trim(); break;
                                case "Zadal": newMark.Teacher = splittedProperty[1].Trim(); break;
                                case "Okruh učiva": newMark.TeachingOkruh = splittedProperty[1].Trim(); break;
                                case "Za co hodnocení": newMark.ForWhat = splittedProperty[1].Trim(); break;
                                case "Stručný komentář": newMark.StructComment = splittedProperty[1].Trim(); break;
                            }
                        }
                        newRatedSubject.Marks.Add(newMark);
                    }
                    marksTable.RatedSubjects.Add(newRatedSubject);
                }
                return marksTable;
            }
            return null;
        }

        private static MarkInfo GetMarkInfo(HtmlDocument doc)
        {
            var nodes = doc.DocumentNode.Descendants().Where(noud => noud.GetAttributeValue("class", "NULL") == "znamkaInfo uprostred barevny");
            if (nodes.Count() > 0)
            {
                MarkInfo markInfo = new MarkInfo();
                HtmlNode mainNode = nodes.ElementAt(0).ParentNode;
                foreach (HtmlNode actualPropertyNode in mainNode.Descendants("div"))
                {
                    if (actualPropertyNode.FirstChild.Name == "span")
                    {
                        String propertyName = actualPropertyNode.FirstChild.InnerText;
                        switch (propertyName)
                        {
                            case "Předmět":
                                markInfo.Subject = actualPropertyNode.LastChild.InnerText;
                                break;
                            case "Datum zkoušky":
                                markInfo.Date = actualPropertyNode.LastChild.InnerText;
                                break;
                            case "Datum zadání hodnocení":
                                markInfo.DateOfEnter = actualPropertyNode.LastChild.InnerText;
                                break;
                            case "Hodnocení":
                                markInfo.Value = actualPropertyNode.LastChild.InnerText;
                                break;
                            case "Váha hodnocení":
                                markInfo.MarkValuability = actualPropertyNode.LastChild.InnerText;
                                break;
                            case "Zadal":
                                markInfo.Teacher = actualPropertyNode.LastChild.InnerText;
                                break;
                            case "Za co hodnocení":
                                markInfo.ForWhat = actualPropertyNode.LastChild.InnerText;
                                break;
                            case "Okruh učiva":
                                markInfo.TeachingOkruh = actualPropertyNode.LastChild.InnerText;
                                break;
                            case "Stručný komentář":
                                if (actualPropertyNode.ChildNodes.Count > 1)
                                    markInfo.StructComment = actualPropertyNode.LastChild.InnerText;
                                break;
                        }
                    }
                }
                return markInfo;
            }
            return null;
        }
    }
}
