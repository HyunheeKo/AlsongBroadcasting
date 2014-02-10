using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using Android.Webkit;

namespace AlsongBroadcasting
{
	[Activity (Label = "AlsongBroadcasting")]
	public class MainActivity : Activity
	{
		int count = 1;
		HttpControl m_HttpCtrl;
		List<BroadcastingInfo> listBroadcasting = new List<BroadcastingInfo>();

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.myButton);
			
			button.Click += delegate {
				button.Text = string.Format ("{0} clicks!", count++);

				// Create your application here
				m_HttpCtrl = new HttpControl ();

				listBroadcasting = m_HttpCtrl.GetBroadcastingList ();

				WebView wv = (WebView)FindViewById(Resource.Id.webView1);
				wv.LoadData("<center>aaaa</center>", "text/html", "EUC-KR");

			};
		}
	}
}


