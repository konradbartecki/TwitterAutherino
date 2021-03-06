﻿using System;
using System.Collections.Generic;
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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace TwitterAutherino.Controls
{
    public sealed partial class TwitterWebView : UserControl
    {
        public TwitterWebView()
        {
            this.InitializeComponent();
            dialogWebBrowser.Navigate(new Uri("http://google.com"));
        }

        private void CloseDialogButton_OnClick(object sender, RoutedEventArgs e)
        {
            dialogWebBrowser.Stop();
            LayoutRoot.Visibility = Visibility.Collapsed;
        }
    }
}
