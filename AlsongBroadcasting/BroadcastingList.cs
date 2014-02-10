using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Data.Common;
using Android.Webkit;
using Com.Google.Ads;

namespace AlsongBroadcasting
{
	[Activity (Label = "알송방송", MainLauncher = true)]			
	public class BroadcastingList : Activity
	{
		ListView m_ListView;

		HttpControl m_HttpCtrl;
		List<BroadcastingInfo> listBroadcasting = new List<BroadcastingInfo>();
		//DataAdapter m_adapter;

		AdView adView;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.BroadcastingList);

			//RequestWindowFeature (WindowFeatures.NoTitle);

			// Create an ad.
			adView = FindViewById<AdView> (Resource.Id.ad);

			// Create an ad request.
			AdRequest adRequest = new AdRequest ();
			adRequest.SetTesting (true);

			adRequest.AddTestDevice (AdRequest.TestEmulator);
			// If you're trying to show ads on device, use this.
			// The device ID to test will be shown on adb log.
			// adRequest.AddTestDevice (some_device_id);

			// Start loading the ad in the background.
			adView.LoadAd (adRequest);


			m_ListView = (ListView)FindViewById (Resource.Id.listView1);

			// Create your application here
			m_HttpCtrl = new HttpControl ();
			listBroadcasting = m_HttpCtrl.GetBroadcastingList ();
			m_ListView.Adapter = new CustomAdapter (this, listBroadcasting);
			m_ListView.ItemClick += OnListItemClick;
			//m_adapter = new DataAdapter ();


		}


		protected void OnListItemClick(object sender, Android.Widget.AdapterView.ItemClickEventArgs e)
		{
			var listView = sender as ListView;
			BroadcastingInfo item = listBroadcasting [e.Position];
			Console.WriteLine ("---------------------------------");
			//			Android.Widget.Toast.MakeText (this, item.Title, Android.Widget.ToastLength.Short).Show ();

			Intent intent = new Intent(this, typeof(Playing) );
	
			item.AddRealTitle ("test1");

			intent.PutExtra ("BroadcastingInfo", item);

			StartActivity (intent);
		}

		protected override void OnDestroy ()
		{
			// Destroy the AdView.
			adView.Destroy();

			base.OnDestroy ();
		}
	}
}