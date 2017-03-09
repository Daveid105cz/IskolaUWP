using Iskola.Data;
using Iskola.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Iskola.Controls
{
    public partial class AllMarksList : UserControl
    {
        public MarksTable Marks
        {
            get { return (MarksTable)GetValue(MarksProperty); }
            set { SetValue(MarksProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Marks.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarksProperty =
            DependencyProperty.Register("Marks", typeof(MarksTable), typeof(AllMarksList), new PropertyMetadata(null));


        public AllMarksList()
        {
            this.InitializeComponent();
        }
    }
    public static class TextBlockExtension
    {
        public static List<Mark> GetMarks(DependencyObject obj)
        { return (List<Mark>)obj.GetValue(MarksProperty); }

        public static void SetMarks(DependencyObject obj, List<Mark> value)
        { obj.SetValue(MarksProperty, value); }

        public static readonly DependencyProperty MarksProperty =
            DependencyProperty.Register("Marks", typeof(List<Mark>), typeof(TextBlockExtension),
            new PropertyMetadata(new List<Mark>(), (sender, e) =>
            {
                List<Mark> marks = e.NewValue as List<Mark>; ;
                var localTextBlock = sender as TextBlock;
                if (localTextBlock != null)
                {
                    localTextBlock.Inlines.Clear();
                    foreach(Mark actualMark in marks)
                    {
                        Hyperlink hyperlink = new Hyperlink();
                        hyperlink.Click += async (lSender, args) => 
                        {
                            MarkInfoDialog markInfoDialog = new MarkInfoDialog(actualMark.ID);
                            await markInfoDialog.ShowDialogAsync();
                        };
                        hyperlink.Inlines.Add(new Run { Text = actualMark.Value });
                        localTextBlock.Inlines.Add(hyperlink);
                        if (marks.Last() != actualMark)
                        {
                            localTextBlock.Inlines.Add(new Run() { Text = ", " });
                        }
                    }
                }
            }));
    }
}
