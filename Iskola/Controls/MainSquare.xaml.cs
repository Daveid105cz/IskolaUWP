using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Iskola.Controls
{
    public sealed partial class MainSquare : UserControl
    {


        public String SquareHeader
        {
            get { return (String)GetValue(SquareHeaderProperty); }
            set { SetValue(SquareHeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SquareHeader.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SquareHeaderProperty =
            DependencyProperty.Register("SquareHeader", typeof(String), typeof(MainSquare), new PropertyMetadata(String.Empty));



        public new object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        public static readonly new DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(MainSquare), new PropertyMetadata(0));



        public MainSquare()
        {
            this.InitializeComponent();
        }
    }
}
