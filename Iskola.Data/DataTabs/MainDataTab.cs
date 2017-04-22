using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace Iskola.Data.DataTabs
{
    public class MainDataTab:DataTab
    {
        ObservableCollection<NewMark> _newMarks = new ObservableCollection<NewMark>();
        public ObservableCollection<NewMark> NewestMarks { get { return _newMarks; } }

        Table _actualTable;
        public Table ActualTable { get { return _actualTable; } internal set { _actualTable = value; PropertyChanged_Invoke("ActualTable"); } }

        private int _newMailsCount;

        public int NewMailsCount
        {
            get { return _newMailsCount; }
            internal set { _newMailsCount = value; PropertyChanged_Invoke("NewMailsCount"); }
        }

        private Dictionary<DateTime, Table> _tables = new Dictionary<DateTime, Table>();

        public Dictionary<DateTime,Table> Tables
        {
            get { return _tables; }
            set { _tables = value; }
        }

        private DateTime _selectedDate = DateTime.MinValue;

        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            internal set { _selectedDate = value; }
        }
        

        ObservableCollection<NewsMessage> _news = new ObservableCollection<NewsMessage>();
        public ObservableCollection<NewsMessage> News { get { return _news; } }

        public MainDataTab(IskolaClient Client) : base(Client) { }

        public async Task NextWeek()
        {
            if (ActualTable.IsNextWeekAvailable)
            {
                SelectedDate = ActualTable.TableNextDate;
                WeekChange = true;
                await DownloadDataAsync();
            }
        }
        public async Task PreviousWeek()
        {
            if (ActualTable.IsPreviousWeekAvailable)
            {
                SelectedDate = ActualTable.TablePreviousDate;
                WeekChange = true;
                await DownloadDataAsync();
            }
        }
        private bool WeekChange;
        protected async override Task DownloadData()
        {
            String downloadString = "https://www.iskola.cz/?akce=hlavni";
            if (_selectedDate != DateTime.MinValue)
            {
                downloadString = "https://www.iskola.cz/?datum="+_selectedDate.Day+"."+_selectedDate.Month+"."+_selectedDate.Year+"&cast=Vychozi&akce=hlavni";
            }
            if (WeekChange && _tables.ContainsKey(SelectedDate))
            { WeekChange = false; ActualTable = _tables[SelectedDate]; }
            else
            {
                String mainPageContent = await Client.LoadRequest(downloadString);
                HtmlDocument mainPageDocument = new HtmlDocument();
                mainPageDocument.LoadHtml(mainPageContent);
                Client.Username = GetUserName(mainPageDocument);
                NewMailsCount = GetNewMailsCount(mainPageDocument);
                _newMarks.Clear();
                foreach (NewMark m in GetNewMarks(mainPageDocument))
                {
                    _newMarks.Add(m);
                }
                Table loadedTable = GetTable(mainPageDocument);
                SelectedDate = loadedTable.TableDate;
                if (_tables.ContainsKey(SelectedDate))
                    _tables.Remove(SelectedDate);
                _tables.Add(SelectedDate, loadedTable);

                ActualTable = loadedTable;
                _news.Clear();
                foreach (NewsMessage n in GetNews(mainPageDocument))
                {
                    _news.Add(n);
                }
            }
        }
        internal override void LogoutClear()
        {
            _newMarks.Clear();
            _news.Clear();
        }
        private static int GetNewMailsCount(HtmlDocument doc)
        {
            var nodes = doc.DocumentNode.Descendants().Where(node => node.Id == "mPololetiZprav");
            if (nodes.Count() > 0)
            {
                return Convert.ToInt32(nodes.ElementAt(0).InnerText);
            }
            return 0;
        }

        private static List<NewMark> GetNewMarks(HtmlDocument doc)
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
        private static Table GetTable(HtmlDocument doc)
        {
            IEnumerable<HtmlNode> nodes = doc.DocumentNode.Descendants().Where(node => { return node.GetAttributeValue("class", "/NULL/") == "tableRozvrh prohlizeni"; });
            if (nodes.Count() > 0)
            {
                HtmlNode tableNode = nodes.ElementAt(0);
                Table table = new Table();
                IEnumerable<HtmlNode> timeDateNodes = doc.DocumentNode.Descendants().Where(node => { return node.Id == "kalendar_datum"; });
                if(timeDateNodes.Count()>0)
                {
                    HtmlNode timeDateNode = timeDateNodes.ElementAt(0);
                    DateTime currentDate = DateTime.Parse(timeDateNode.InnerText);
                    DateTime previousDate = currentDate.AddDays(-7);
                    int daysToAdd = ((int)DayOfWeek.Monday - (int)currentDate.AddDays(1).DayOfWeek + 7) % 7;
                    DateTime nextDate = currentDate.AddDays(daysToAdd+1);
                    table.TableDate = currentDate;
                    table.TablePreviousDate = previousDate;
                    table.TableNextDate = nextDate;
                    table.IsPreviousWeekAvailable = true;
                    table.IsNextWeekAvailable = true;
                    IEnumerable<HtmlNode> variablesNodes = doc.DocumentNode.Descendants().Where(node => { return node.Id == "promenne"; });
                    if(variablesNodes.Count()>0)
                    {
                        HtmlNode textsNode = variablesNodes.ElementAt(0).ChildNodes[0];
                        String JSONContent = textsNode.InnerText.Replace("&quot;","\"");
                        JObject jsonObject = JObject.Parse(JSONContent);
                        DateTime dateFrom = DateTime.Parse(jsonObject["kalendar_datumOd"].Value<String>());
                        DateTime dateTo = DateTime.Parse(jsonObject["kalendar_datumDo"].Value<String>());
                        if (table.TablePreviousDate < dateFrom)
                            table.TablePreviousDate = dateFrom;
                        if (currentDate == dateFrom)
                            table.IsPreviousWeekAvailable = false;
                        if (table.TableNextDate > dateTo)
                            table.IsNextWeekAvailable = false;
                        
                    }
                }
                ushort HourID = 0;
                foreach (var node in tableNode.FirstChild.FirstChild.ChildNodes)
                {
                    if (node.GetAttributeValue("class", "N").Contains("zahlaviHodina"))
                    {
                        HourDefinition hd = new HourDefinition()
                        {
                            FromTo = node.LastChild.InnerText,
                            HourNumber = Convert.ToUInt16(Regex.Replace(node.FirstChild.InnerText, "[^0-9]", ""))
                        };
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
                                            SchoolAction newSchoolAction = new SchoolAction();
                                            newSchoolAction.ActionName = actualSubject.InnerText;
                                            String styleWidthValue = actualSubject.FirstChild.GetAttributeValue("style", "width: 120px");
                                            int width = Convert.ToInt32(styleWidthValue.Remove(styleWidthValue.Length - 3).Split(' ')[1]) + 1;
                                            newSchoolAction.ActionLenght = (width + 10) / 123;
                                            newSchoolAction.ActionPosition = newHour.HourNumber;
                                            newDay.SchoolActions.Add(newSchoolAction);
                                        }
                                        else if (className != "skolniAkce")
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
        private static void SetDayInfo(HtmlNode usedNode, Day day)
        {
            IEnumerable<HtmlNode> nodes = usedNode.Descendants().Where(node => node.Name == "th");
            if (nodes.Count() > 0)
            {
                HtmlNode dayNode = nodes.ElementAt(0);
                day.DayInWeek = dayNode.ChildNodes[0].InnerText;
                day.Date = dayNode.ChildNodes[1].InnerText.Replace("(", "").Replace(")", "");
            }
        }
        private static List<NewsMessage> GetNews(HtmlDocument doc)
        {
            List<NewsMessage> news = new List<NewsMessage>();
            IEnumerable<HtmlNode> nodes = doc.DocumentNode.Descendants().Where(node => node.Id == "dlazdiceNovinky");
            if (nodes.Count() > 0)
            {
                HtmlNode newsNode = nodes.ElementAt(0);

                foreach (var node in newsNode.ChildNodes)
                {
                    if (node.Name == "div" && node.GetAttributeValue("class", "NULL") == "novinkaDlazdice")
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
        private static String GetUserName(HtmlDocument doc)
        {
            var nodes = doc.DocumentNode.Descendants().Where(node => node.Id == "zobraz_mPrihlas_nabidka");
            if (nodes.Count() > 0)
            {
                return nodes.ElementAt(0).InnerText;
            }
            return "JménoNenalezeno";
        }
    }
}
