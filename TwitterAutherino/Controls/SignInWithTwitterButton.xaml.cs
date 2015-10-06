using System;
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
using TwitterAutherino.Model.EventArgs;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace TwitterAutherino.Controls
{
    public sealed partial class SignInWithTwitterButton : UserControl
    {
        public string ConsumerPublicKey{ get; set; }
        public string ConsumerSecretKey { get; set; }
        public string CallbackUri { get; set; }
        public FrameworkElement WebDialogPlacementTarget { get; set; }

        public event EventHandler AuthenticationFinished;
        public event EventHandler CredentialsVerified;

        //public delegate void OnAuthenticationFinishedEventHandler(object sender, EventArgs e);
        //public delegate void OnCredentialsVerifiedEventHandler(object sender, EventArgs e);
        //public delegate void OnAuthenticationFinishedEventHandler(object sender, EventArgs e);

        private TwitterAuth _twitter;
        private Uri requestUri;

        public SignInWithTwitterButton()
        {
            this.InitializeComponent();
        }

        private async void Image_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _twitter = new TwitterAuth(ConsumerPublicKey, ConsumerSecretKey);
            await _twitter.GetRequestTokenAsync(CallbackUri);
            _twitter.GotRequestResponseKeypair += _twitter_GotRequestResponseKeypair;
            _twitter.ShowWebDialogFlyout(WebDialogPlacementTarget);

        }

        private void _twitter_GotRequestResponseKeypair(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnAuthenticationFinished()
        {
            AuthenticationFinished?.Invoke(this, EventArgs.Empty);
        }

        private void OnCredentialsVerified()
        {
            CredentialsVerified?.Invoke(this, EventArgs.Empty);
        }
    }
}
