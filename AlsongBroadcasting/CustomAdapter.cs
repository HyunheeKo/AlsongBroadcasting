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
using Android.Webkit;

namespace AlsongBroadcasting
{
	public class CustomAdapter: BaseAdapter<BroadcastingInfo>
	{
		List<BroadcastingInfo> items;
		Activity context;
		public CustomAdapter(Activity context, List<BroadcastingInfo> items)
			: base()
		{
			this.context = context;
			this.items = items;
		}
		public override long GetItemId(int position)
		{
			return position;
		}
		public override BroadcastingInfo this[int position]
		{
			get { return items[position]; }
		}
		public override int Count
		{
			get { return items.Count; }
		}
		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			var item = items[position];
			View view = convertView;
			if (view == null) { // no view to re-use, create new
				//view = context.LayoutInflater.Inflate (Resource.Layout.CustomRow_WebViewBase, null);
				view = context.LayoutInflater.Inflate (Resource.Layout.CustomRow, null);
			}
			view.FindViewById<TextView>(Resource.Id.Text_Title).Text = item.Title;
			view.FindViewById<TextView>(Resource.Id.Text_Title).Selected = true;
			view.FindViewById<TextView>(Resource.Id.Text_URL).Text = item.URL;
			view.FindViewById<ImageView>(Resource.Id.Image_logo).SetImageResource(Resource.Drawable.alban_52);
			//WebView wv = view.FindViewById<WebView>(Resource.Id.webView1);
			//wv.LoadData(item.Title, "text/html; charset=utf-8", "utf-8");
			//wv.
			return view;
		}
	}

}

