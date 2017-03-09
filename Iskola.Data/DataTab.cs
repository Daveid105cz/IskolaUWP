using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iskola.Data
{
    public class DataTab:INotifyPropertyChanged
    {
        private IskolaClient _client;

        public IskolaClient Client
        {
            get { return _client; }
        }

        public DataTab(IskolaClient Client)
        {
            _client = Client;
        }
        private bool _isDownloading;
        public bool IsDownloading { get { return _isDownloading; }private set { _isDownloading = value;PropertyChanged_Invoke("IsDownloading"); } }

        public bool IsAnythingLoaded { get; private set; } = false;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void PropertyChanged_Invoke(String PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        protected async virtual Task DownloadData()
        {

        }
        public async Task DownloadDataAsync()
        {
            IsAnythingLoaded = true;
            IsDownloading = true;
            await DownloadData();
            IsDownloading = false;
        }
        internal virtual void LogoutClear()
        {

        }
    }
}
