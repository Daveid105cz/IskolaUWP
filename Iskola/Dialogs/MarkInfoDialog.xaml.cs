using Iskola.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Iskola.Dialogs
{
    public sealed partial class MarkInfoDialog : ContentDialog
    {


        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            private set { SetValue(IsLoadingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsLoading.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register("IsLoading", typeof(bool), typeof(MarkInfoDialog), new PropertyMetadata(true));

        private long _ID;
        public MarkInfoDialog(long MarkID)
        {
            this.InitializeComponent();
            _ID = MarkID;
            this.Opened += MarkInfoDialog_Opened;
        }

        private async void MarkInfoDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            DataContext = await App.Client.GetMarkInfo(_ID);
            IsLoading = false;
        }
    }
}
