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
using Android.Media;
using Java.Lang;
using Android.Support.V4.App;
using Com.Google.Ads;

namespace AlsongBroadcasting
{
	[Activity (Label = "Playing")]			
	public class Playing : Activity
	{
		//string TAG = "ASB";

		private Button buttonPlayAndStop;
		private Button buttonBack;

		private MediaPlayer player;

		static BroadcastingInfo bi = null;

		private List<string> m_listRealTitle = new List<string>();
		private List<string> m_listRealURL = new List<string> ();

		private int _count = 0;
		private int _id = 132;
		private int m_nIndex = 0;

		AdView adView;


		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			// Set our view from the "main" layout resource4F4
			SetContentView (Resource.Layout.Playing);
			// Create your application here

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


			if (bi == null) {
				bi = (BroadcastingInfo)Intent.GetSerializableExtra ("BroadcastingInfo");

				if (bi.IsPls) {
					//
					HttpControl httpCtrl = new HttpControl ();
					bi = httpCtrl.ParsePls( httpCtrl.GetWeb (bi.URL, System.Text.Encoding.GetEncoding("cp949")), bi);
				}		
			}

			initializeUIElements ();
			initializeMediaPlayer ();
		}



		private void initializeUIElements() {
			string strTitle = null;

			if (bi.RealTitle.Count > 0) {
				foreach (string tmp in bi.RealTitle) {
					strTitle += tmp + "  ";
				}
			} else {
				strTitle = bi.Title;
			}

			Console.WriteLine ("---------------------" + strTitle);

			FindViewById<TextView>(Resource.Id.textView_Broadcasting).Text = strTitle;
			FindViewById<TextView>(Resource.Id.textView_Broadcasting).Selected = true;
			FindViewById<TextView>(Resource.Id.textView_Title).Text = bi.URL;
			FindViewById<TextView>(Resource.Id.textView_Title).Selected = true;

			buttonPlayAndStop = FindViewById<Button>(Resource.Id.button_PlayAndStop);
			buttonBack = FindViewById<Button> (Resource.Id.button_Back);

			//buttonPlayAndStop.SetsetEnabled(false);
			buttonPlayAndStop.Click += (sender, e) => {
				Button btn = sender as Button;

				if(btn.Text == "재생")
				{
					StartPlaying();
					btn.Text = "로딩중";
				} else {
					StopPlaying();
					btn.Text = "재생";
				}
			};

			buttonBack.Click += (object sender, EventArgs e) => {
				Intent intent = new Intent(this, typeof(BroadcastingList));
				//Finish();
				intent.PutExtra ("BroadcastingInfo", bi);
				StartActivity (intent);
			};
		}


		private void StartPlaying() {

			player.PrepareAsync();

			player.Prepared += (object sender, EventArgs e) => {
				Console.Write ("StartPlaying : ");

				player.Start();
				buttonPlayAndStop.Text = "정지";
			};
		}

		private void StopPlaying() {
			if (player.IsPlaying) {
				player.Stop();
				player.Release();
				initializeMediaPlayer();
			}
		}

		private void initializeMediaPlayer() {
			player = new MediaPlayer();
			try {
				if (bi.IsPls) {
					//Console.WriteLine("SetDataSource URL : " + bi.RealURL[0]);
					player.SetDataSource(   bi.RealURL[m_nIndex]  );
				} else {
					player.SetDataSource(   bi.URL );
				}

			} catch (Java.Lang.Exception e) {
				Console.Write ("Error initializeMediaPlayer : ");
				e.PrintStackTrace();
			} 

			player.BufferingUpdate += (object sender, MediaPlayer.BufferingUpdateEventArgs e) => {
				Console.WriteLine("Buffering : ", e.Percent);
			};

			player.Completion += (object sender, EventArgs e) => {
				Console.WriteLine("Completion : " + e.ToString());
			};

			player.Error += (object sender, MediaPlayer.ErrorEventArgs e) => {
				Console.WriteLine(e.What);
				if (e.Extra == -1004)
				{
					Console.WriteLine("SetDataSource URL : " + bi.URL);

				}
				if(e.What == MediaError.Unknown)
				{
					Console.WriteLine("SetDataSource URL : " + bi.RealURL[++m_nIndex]);
					player.SetDataSource(   bi.RealURL[m_nIndex]  );
					StartPlaying();
				}
			};
		}

		private void SetNotification()
		{

			Intent intent = new Intent(this, typeof(Playing) );
			intent.PutExtra ("BroadcastingInfo", bi);

			TaskStackBuilder stackBuilder = TaskStackBuilder.Create(this);
			stackBuilder.AddParentStack(Class.FromType(typeof(Playing)));
			stackBuilder.AddNextIntent(intent);

			PendingIntent resultPendingIntent = stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);

			// Build the notification
			NotificationCompat.Builder builder = new NotificationCompat.Builder(this)
				.SetAutoCancel(true) // dismiss the notification from the notification area when the user clicks on it
				.SetContentIntent(resultPendingIntent) // start up this activity when the user clicks the intent.
				.SetContentTitle("알송 방송") // Set the title
				.SetNumber(_count) // Display the count in the Content Info
				.SetSmallIcon(Resource.Drawable.Headphones) // This is the icon to display
				.SetContentText(string.Format(bi.Title)); // the message to display.

			// Finally publish the notification
			NotificationManager notificationManager = (NotificationManager)GetSystemService(NotificationService);
			notificationManager.Notify(_id, builder.Build());
		}

		protected override void OnPause() {
			base.OnPause ();

			if (player.IsPlaying) {
				//player.Stop();
				SetNotification ();
			}
		}

		protected override void OnDestroy ()
		{
			// Destroy the AdView.
			adView.Destroy();

			bi = null;

			base.OnDestroy ();
		}
	}
}

