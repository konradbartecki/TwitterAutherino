# TwitterAutherino ![Nuget Version](https://img.shields.io/nuget/v/TwitterAutherino.svg) ![BSD License](https://img.shields.io/badge/license-BSD-blue.svg)

Sign in with Twitter button implementation for Windows Phone 8.1 released as Nuget library
You can use this NuGet for Windows Universal 8.1, but you should use Web Authentication Broker for Windows 8.1

### How to use

#### Method 1. Making your own Twitter button
##### Step 1. Download TwitterAutherino using NuGet

Open the Package Manager Console and type in `Install-Package TwitterAutherino`

##### Step 2. Add code

Add `using TwitterAutherino;` and some button to your page. Subscribe to the button's Click event

```C#
   TwitterAuth twitterAuth;

        private async void TwitterButton_Click(object sender, RoutedEventArgs e)
        {
            //Create new TwitterAuth object with your consumer id and consumer secret
            twitterAuth = new TwitterAuth("YOUR_CONSUMER_ID", "YOUR_CONSUMER_SECRET");
            //Call GetRequestTokenAsync with the callback url you have set in your Twitter dashboard
            //The url below could be anything but make sure it's the same as in your Twitter dashboard
            await twitterAuth.GetRequestTokenAsync("http://twitter.com/mobile");
            //Subscribe to the GotRequestResponseKeypair event
            twitterAuth.GotRequestResponseKeypair += TwitterAuth_GotRequestResponseKeypair;
            //Where should the WebDialog flyout be placed? The good practice is to point it to the parent grid (often with the name "LayoutRoot"
            twitterAuth.ShowWebDialogFlyout(LayoutGrid);
        }
        
        private async void TwitterAuth_GotRequestResponseKeypair(object sender, System.EventArgs e)
        {
            var user = await twitterAuth.GetAccessTokenAsync();
            var response = await twitterAuth.GetUserDetailsAsync();
            await new MessageDialog(String.Format("Welcome {0}", user.ScreenName), "Registered with Twitter").ShowAsync();
        }
```

#####

#### Method 2. Using Sign-in-with-Twitter UserControl
This user control is not working yet. Scheduled for v.0.13


### Troubleshooting
##### Help! I've got exception 401 Unauthorized
Double check your date and time.
