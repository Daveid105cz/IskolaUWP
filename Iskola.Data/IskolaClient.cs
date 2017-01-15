using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Iskola.Data
{
    public class IskolaClient
    {
        private String _SID;
        private String _BID;

        private String _username;

        #region Public User info
        //New marks list
        ObservableCollection<NewMark> _newMarks = new ObservableCollection<NewMark>();
        public ObservableCollection<NewMark> NewestMarks { get { return _newMarks; } }

        //Actual day table
        Table _actualTable;
        public Table ActualTable { get { return _actualTable; } }

        //News
        ObservableCollection<NewsMessage> _news = new ObservableCollection<NewsMessage>();
        public ObservableCollection<NewsMessage> News { get { return _news; } }
        
        //All Marks table
        MarksTable _marksTable;
        public MarksTable MarksTable { get { return _marksTable; } }
        #endregion
        public async Task<ConnectionResult> Login(String Username,String Password,String School)
        {
            ConnectionResult connectionResult = await LoginState(Username, Password,School);
            if (connectionResult==ConnectionResult.Success)
            {
                String mainPageContent = await LoadRequest("https://www.iskola.cz/?akce=hlavni");
                HtmlDocument mainPageDocument = new HtmlDocument();
                mainPageDocument.LoadHtml(mainPageContent);
                //Get marks from server and put them into collection
                _newMarks.Clear();
                foreach (NewMark m in DataParser.GetNewMarks(mainPageDocument))
                {
                    _newMarks.Add(m);
                }
                _actualTable = DataParser.GetTable(mainPageDocument);

                 foreach (NewsMessage n in DataParser.GetNews(mainPageDocument))
                 {
                     _news.Add(n);
                 }
                 await LoadMarking();
            }
            return connectionResult;
        }

        public async Task LoadMarking()
        {
            String response = await LoadRequest("https://www.iskola.cz/?cast=Hodnoceni&akce=zak");
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(response);
            _marksTable = DataParser.GetMarks(doc);
        }

        private Dictionary<long, MarkInfo> _markInfoBuffer = new Dictionary<long, MarkInfo>();
        public async Task<MarkInfo> GetMarkInfo(long ID)
        {
            if (_markInfoBuffer.ContainsKey(ID))
                return _markInfoBuffer[ID];
            String response = await LoadRequest("https://www.iskola.cz/?cast=Hodnoceni&akce=znamka&id=" + ID.ToString());
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(response);
            MarkInfo loadedMarkInfo = DataParser.GetMarkInfo(doc);
            _markInfoBuffer.Add(ID, loadedMarkInfo);
            return loadedMarkInfo;
        }
        private async Task<ConnectionResult> LoginState(String Usr, String Psd,String Sch)
        {
            HttpClientHandler handler = new HttpClientHandler
            {
                AllowAutoRedirect = false,
                UseCookies = true,
                CookieContainer = new CookieContainer()
            };
            using (var client = new HttpClient(handler))
            {
                var loginCredentials = new Dictionary<string, string>
                    {
                        { "mPrihlasJmeno", Usr },
                        { "mPrihlasHeslo", Psd },
                        { "mPrihlasSkola", Sch }
                    };

                var loginFormContent = new FormUrlEncodedContent(loginCredentials);

                try
                {
                    var response = await client.PostAsync("https://www.iskola.cz/?cast=Uzivatel&akce=prihlas", loginFormContent);
                    CookieCollection cookieColection = handler.CookieContainer.GetCookies(new Uri("https://www.iskola.cz"));
                    if (cookieColection.Count >= 2)
                    {
                        foreach (object cookie in cookieColection)
                        {
                            String cookieValue = cookie.ToString();
                            if (cookieValue.StartsWith("sid="))
                                _SID = cookieValue.Split('=')[1];
                            if (cookieValue.StartsWith("bid="))
                                _BID = cookieValue.Split('=')[1];
                        }
                        return ConnectionResult.Success;
                    }
                }
                catch { return ConnectionResult.ConnectionProblems; }
                return ConnectionResult.IncorrectCredentials;
            }
        }

        private HttpClientHandler GetHandler()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = true,
                UseCookies = true,
                CookieContainer = new CookieContainer()
            };
            handler.CookieContainer.SetCookies(new Uri("https://www.iskola.cz"), "sid=" + _SID + ";bid=" + _BID);
            return handler;
        }
        private async Task<String> LoadRequest(String Uri)
        {
            using (HttpClient client = new HttpClient(GetHandler()))
            {
                HttpResponseMessage responseMessage = await client.GetAsync(Uri);
                String responsedHTML = await responseMessage.Content.ReadAsStringAsync();
                //TODO: Add check for timeout and then raise OnTimoutRaised to ask user to Re-Login
                return responsedHTML;
            }
        }

        //Now completely useless!!
        public delegate bool TimeoutRaisedHandler();
        public event TimeoutRaisedHandler OnTimeoutRaised;
    }
    public enum ConnectionResult
    {
        Success,
        IncorrectCredentials,
        ConnectionProblems
    }
}
