using HtmlAgilityPack;
using Iskola.Data.DataTabs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Iskola.Data
{
    public partial class IskolaClient : INotifyPropertyChanged
    {

        private String _SID;
        private String _BID;

        private String _username;
        private String _loginUsername;
        private String _password;
        private String _school;
        public String Username { get { return _username; } internal set { _username = value;InvokePropertyChanged("Username"); } }

        public ReadOnlyCollection<DataTab> DataTabs { get { return _dataTabs.AsReadOnly(); } }
        private List<DataTab> _dataTabs = new List<DataTab>();

        public DataTab MainDataTab { get { return _dataTabs[0]; } }
        public DataTab RatingDataTab { get { return _dataTabs[1]; } }

        public async Task<ConnectionResult> Login(String Username,String Password,String School)
        {
            _loginUsername = Username;
            _password = Password;
            _school = School;
            ConnectionResult connectionResult = await LoginState(Username, Password,School);
            if (connectionResult == ConnectionResult.Success)
            {
                InitDataTabs();
                await _dataTabs[0].DownloadDataAsync();
            }
                return connectionResult;
        }
        private void InitDataTabs()
        {
            _dataTabs.Add(new MainDataTab(this));
            _dataTabs.Add(new RatingDataTab(this));
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

        #region MainDataAccess
        private HttpClientHandler GetHandler(bool AllowAutoRedirect)
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = AllowAutoRedirect,
                UseCookies = true,
                CookieContainer = new CookieContainer()
            };
            handler.CookieContainer.SetCookies(new Uri("https://www.iskola.cz"), "sid=" + _SID + ";bid=" + _BID);
            return handler;
        }
        internal async Task<String> LoadRequest(String Uri,bool AllowAutoRedirect = true)
        {
            using (HttpClient client = new HttpClient(GetHandler(AllowAutoRedirect)))
            {
                HttpResponseMessage responseMessage = await client.GetAsync(Uri);
                String responsedHTML = await responseMessage.Content.ReadAsStringAsync();
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(responsedHTML);
                if (AllowAutoRedirect != false)
                {
                    bool isLogouted = CheckLogin(document);
                    if (isLogouted && OnTimeoutRaised != null && await OnTimeoutRaised.Invoke())
                    {
                        ConnectionResult connectionResult = await LoginState(_loginUsername, _password, _school);
                        if (connectionResult == ConnectionResult.Success)
                            return await LoadRequest(Uri, AllowAutoRedirect);
                        else
                            OnLoginFailed?.Invoke();
                    }
                }
                return responsedHTML;
            }
        }

        private static bool CheckLogin(HtmlDocument doc)
        {
            return (doc.DocumentNode.LastChild.LastChild.FirstChild.GetAttributeValue("class", "NIC") == "neprihlaseny");
        }

        public delegate Task<bool> TimeoutRaisedHandler();
        public event TimeoutRaisedHandler OnTimeoutRaised;

        public delegate void LoginFailedHandler();
        public event LoginFailedHandler OnLoginFailed;
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        private void InvokePropertyChanged(String Property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Property));
        }
        public void Logout()
        {
            SendLogoutRequest();
            foreach(DataTab proceededDataTable in _dataTabs)
            {
                proceededDataTable.LogoutClear();
            }
            _dataTabs.Clear();
        }
        private async void SendLogoutRequest()
        {
            await LoadRequest("https://www.iskola.cz/?cast=Uzivatel&akce=odhlas", false);
        }
    }   
    public enum ConnectionResult
    {
        Success,
        IncorrectCredentials,
        ConnectionProblems
    }
}
