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
    public sealed partial class AllMarksList : UserControl
    {

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
                var textBl = sender as TextBlock;
                if (textBl != null)
                {
                    textBl.Unloaded -= TextBl_Unloaded;
                    textBl.Unloaded += TextBl_Unloaded;
                    textBl.Inlines.Clear();
                    foreach(Mark m in marks)
                    {
                        Hyperlink link = new Hyperlink();
                        _hyperlinksDatabase.Add(link, m);
                        link.Click += Link_Click;
                        link.Inlines.Add(new Run { Text = m.Value });
                        textBl.Inlines.Add(link);
                        if (marks.Last() != m)
                        {
                            textBl.Inlines.Add(new Run() { Text = ", " });
                        }
                    }
                }
            }));

        private static void TextBl_Unloaded(object sender, RoutedEventArgs e)
        {
            TextBlock tb = sender as TextBlock;
            tb.Unloaded -= TextBl_Unloaded;
            foreach(var t in tb.Inlines)
            {
                if(t is Hyperlink)
                {
                    _hyperlinksDatabase.Remove(t as Hyperlink);
                    Debug.WriteLine("removing item from dictionary");
                }
            }
        }

        private static Dictionary<Hyperlink, Mark> _hyperlinksDatabase = new Dictionary<Hyperlink, Mark>();
        private async static void Link_Click(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            //TODO: Open dialog with info about mark
            Mark selectedMark = _hyperlinksDatabase[sender];
            MarkInfoDialog mid = new MarkInfoDialog(selectedMark.ID);
            await mid.ShowAsync();
        }
    }
}
