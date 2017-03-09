using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace Iskola
{
    public static class DialogHelper
    {
        private static ContentDialog _contentDialog;
        public static async Task<ContentDialogResult> ShowDialogAsync(this ContentDialog contentDialog)
        {
            if(_contentDialog!=null)
            {
                _contentDialog.Hide();
                _contentDialog = null;
            }
             
            _contentDialog = contentDialog;
            _contentDialog.Closing += (conentDialog, contentDialogClosingEventArgs) => { if (DialogHelper._contentDialog == conentDialog) DialogHelper._contentDialog = null; };
            return await _contentDialog.ShowAsync();
        }
    }
}
