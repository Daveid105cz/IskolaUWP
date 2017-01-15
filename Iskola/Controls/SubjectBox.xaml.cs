﻿using System;
using Iskola.Data;
using Iskola.Dialogs;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Iskola.Controls
{
    public sealed partial class SubjectBox : UserControl
    {
        public SubjectBox()
        {
            this.InitializeComponent();
        }

        private async void UserControl_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            SubjectInfoDialog subjectInfoDialog = new SubjectInfoDialog((DataContext as Subject));
            await subjectInfoDialog.ShowAsync();
        }
    }
}
