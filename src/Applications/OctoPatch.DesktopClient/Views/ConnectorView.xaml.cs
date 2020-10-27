using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OctoPatch.ContentTypes;

namespace OctoPatch.DesktopClient.Views
{
    /// <summary>
    /// Interaction logic for ConnectorView.xaml
    /// </summary>
    public partial class ConnectorView : UserControl
    {
        // Using a DependencyProperty as the backing store for ContentType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentTypeProperty =
            DependencyProperty.Register("ContentType", typeof(ContentType), typeof(ConnectorView));

        [Bindable(true)]
        public ContentType ContentType
        {
            get => (ContentType)GetValue(ContentTypeProperty);
            set => SetValue(ContentTypeProperty, value);
        }

        public ConnectorView()
        {
            InitializeComponent();
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (ContentType is ComplexContentType complexContentType)
            {
                var x = complexContentType.Type;
            }

            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
        }
    }
}
