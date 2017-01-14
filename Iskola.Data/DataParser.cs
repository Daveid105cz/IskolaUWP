using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace Iskola.Data
{
    internal static  class DataParser
    {
        internal static List<NewMark> GetNewMarks(HtmlDocument doc)
        {
            List<NewMark> marks = new List<NewMark>();
            var nodes = doc.DocumentNode.Descendants().Where(node => node.Id == "dlazdiceZnamka");
            if (nodes.Count() > 0)
            {
                HtmlNode marksNode = nodes.ElementAt(0);

                foreach (var node in marksNode.ChildNodes)
                {
                    if (node.Name == "div")
                    {
                        NewMark newMark = new NewMark();
                        String date = node.ChildNodes[0].InnerText;
                        String mark = node.ChildNodes[1].InnerText;
                        newMark.Date = date.Split(',')[0];
                        newMark.Teacher = date.Split(',')[1].Trim().Substring(6).Trim();
                        newMark.Subject = mark.Split(',')[0];
                        newMark.Value = mark.Split(',')[1].Trim();
                        marks.Add(newMark);
                    }
                }
            }
            return marks;
        }
        internal static Table GetTable(HtmlDocument doc)
        {
            IEnumerable<HtmlNode> nodes = doc.DocumentNode.Descendants().Where(node => { return node.GetAttributeValue("class", "/NULL/") == "tableRozvrh prohlizeni"; });
            if (nodes.Count() > 0)
            {
                HtmlNode tableNode = nodes.ElementAt(0);
                Table table = new Table();
                ushort HourID = 0;
                foreach(var node in tableNode.FirstChild.FirstChild.ChildNodes)
                {
                    if(node.GetAttributeValue("class", "N").Contains("zahlaviHodina"))
                    {
                        HourDefinition hd = new HourDefinition();
                        hd.FromTo = node.LastChild.InnerText;
                        hd.HourNumber = Convert.ToUInt16(Regex.Replace(node.FirstChild.InnerText, "[^0-9]", ""));
                        HourID++;
                        table.HourDefinitions.Add(hd);
                    }
                }
                foreach (HtmlNode actualNode in tableNode.LastChild.ChildNodes)
                {
                    Day newDay = new Day();
                    HourID = 0;
                    var dayNodes = actualNode.Descendants().Where(nd => nd.GetAttributeValue("class", "N") == "tdVolno");
                    if (dayNodes.Count() > 0)
                    {
                        newDay.IsFreeDay = true;
                        newDay.FreeDayName = dayNodes.ElementAt(0).InnerText;
                        SetDayInfo(actualNode, newDay);
                    }
                    else
                    {
                        foreach (HtmlNode actualHourNode in actualNode.ChildNodes)
                        {
                            if (actualHourNode.Name == "td")
                            {
                                Hour newHour = new Hour();
                                newHour.HourNumber = HourID;
                                if (actualHourNode.HasChildNodes)
                                {
                                    foreach (HtmlNode actualSubject in actualHourNode.ChildNodes)
                                    {
                                        Subject newSubject = new Subject();
                                        String className = actualSubject.GetAttributeValue("class", "NULL");
                                        if (className == "skolniAkce" && actualSubject.HasChildNodes)
                                        {
                                            newSubject.IsSchoolAction = true;
                                            newSubject.SchoolActionName = actualSubject.InnerText;
                                            String styleWidthValue = actualSubject.FirstChild.GetAttributeValue("style", "width: 120px");
                                            int width = Convert.ToInt32(styleWidthValue.Remove(styleWidthValue.Length - 3).Split(' ')[1]) + 1;
                                            newSubject.SchoolActionLenght = width / 120;
                                            newHour.Subjects.Add(newSubject);
                                        }
                                        else if(className!="skolniAkce")
                                        {
                                            newSubject.Title = actualSubject.GetAttributeValue("title", "Title Not Found");
                                            newSubject.Title = newSubject.Title.Replace("&#10;", "\n").Replace("&#13;", "");
                                            if (className.Contains("zrusena"))
                                                newSubject.State = SubjectState.Canceled;
                                            foreach (HtmlNode actualPropertyNode in actualSubject.Descendants())
                                            {
                                                String propertyClassName = (actualPropertyNode.GetAttributeValue("class", "badClass") + " -A-A-A-A").Split(' ')[1].Substring(6);
                                                switch (propertyClassName)
                                                {
                                                    case "Predmet": newSubject.SubjectName = actualPropertyNode.InnerText; break;
                                                    case "Ucitel": newSubject.Teacher = actualPropertyNode.InnerText; break;
                                                    case "Ucebna": newSubject.Placement = actualPropertyNode.InnerText; break;
                                                    case "Umisteni": newSubject.Placement = actualPropertyNode.InnerText; break;
                                                }
                                            }
                                            newHour.Subjects.Add(newSubject);
                                        }
                                        
                                    }
                                }
                                HourID++;
                                newDay.Hours.Add(newHour);
                            }
                            else if (actualHourNode.Name == "th")
                            {
                                newDay.DayInWeek = actualHourNode.ChildNodes[0].InnerText;
                                newDay.Date = actualHourNode.ChildNodes[1].InnerText.Replace("(", "").Replace(")", "");
                            }

                        }
                    }
                    table.Days.Add(newDay);
                }
                return table;
            }
            return null;
        }
        internal static List<NewsMessage> GetNews(HtmlDocument doc)
        {
            List<NewsMessage> news = new List<NewsMessage>();
            IEnumerable<HtmlNode> nodes = doc.DocumentNode.Descendants().Where(node => node.Id == "dlazdiceNovinky");
            if (nodes.Count() > 0)
            {
                HtmlNode newsNode = nodes.ElementAt(0);

                foreach (var node in newsNode.ChildNodes)
                {
                    if (node.Name == "div" && node.GetAttributeValue("class","NULL")=="novinkaDlazdice")
                    {
                        NewsMessage newMessage = new NewsMessage();
                        newMessage.Date = node.ChildNodes[0].InnerText;
                        newMessage.Title = node.ChildNodes[1].InnerText;
                        newMessage.Content = node.ChildNodes[2].InnerHtml;
                        news.Add(newMessage);
                    }
                }
            }
            return news;
        }

        internal static MarksTable GetMarks(HtmlDocument doc)
        { 
            IEnumerable<HtmlNode> nodes = doc.DocumentNode.Descendants().Where(node => node.Id=="vypis");
            if(nodes.Count()>0)
            {
                HtmlNode mainNode = nodes.ElementAt(0).LastChild;
                MarksTable marksTable = new MarksTable();
                foreach(var node in mainNode.ChildNodes)
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

        internal static MarkInfo GetMarkInfo(HtmlDocument doc)
        {
            var nodes = doc.DocumentNode.Descendants().Where(noud => noud.GetAttributeValue("class","NULL") == "znamkaInfo uprostred barevny");
            if (nodes.Count()>0)
            {
                MarkInfo markInfo = new MarkInfo();
                HtmlNode mainNode = nodes.ElementAt(0).ParentNode;
                foreach (HtmlNode actualPropertyNode in mainNode.Descendants("div"))
                {
                    if (actualPropertyNode.FirstChild.Name=="span")
                    {
                        String propertyName = actualPropertyNode.FirstChild.InnerText;
                        switch(propertyName)
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
                                markInfo.StructComment = actualPropertyNode.LastChild.InnerText;
                                break;
                        }
                    }
                }
                    return markInfo;
            }
            return null;
        }

        private static void SetDayInfo(HtmlNode usedNode,Day day)
        {
            IEnumerable<HtmlNode> nodes = usedNode.Descendants().Where(node => node.Name == "th");
            if(nodes.Count()>0)
            {
                HtmlNode dayNode = nodes.ElementAt(0);
                day.DayInWeek = dayNode.ChildNodes[0].InnerText;
                day.Date = dayNode.ChildNodes[1].InnerText.Replace("(", "").Replace(")", "");
            }
        }
    }
}
