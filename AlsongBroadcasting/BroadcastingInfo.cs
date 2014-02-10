using System;
using System.Collections.Generic;
using Android.Runtime;

using Java.Interop;
using Java.IO;


namespace AlsongBroadcasting
{

	public class BroadcastingInfo : Java.Lang.Object, ISerializable
	{
		private string m_strTitle;
		private string m_strURL;

		private List<string> m_listRealTitle = new List<string>();
		private List<string> m_listRealURL = new List<string> ();
		//public List<string> list_URL;
		//public List<string> list_Title;
		public bool IsPls;

		public string Title {
			get { return m_strTitle; }
			set { m_strTitle = value; }
		}

		public string URL {
			get { return m_strURL; }
			set { m_strURL = value; }
		}

		public List<string> RealTitle {
			get { return m_listRealTitle; }
		}

		public List<string> RealURL {
			get { return m_listRealURL; }
		}

		public void AddRealTitle(string strTitle)
		{
			m_listRealTitle.Add (strTitle);
		}

		public void AddRealURL(string strURL)
		{
			m_listRealURL.Add (strURL);
		}

		public void Clear()
		{
			m_listRealTitle.Clear ();
			m_listRealURL.Clear ();
		}
		List<string> list1 = null;
		//------------------------------------------------------------------------------------------------------

		public BroadcastingInfo()
		{
		}

		public BroadcastingInfo(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
		{
		}

		[Export("readObject", Throws = new[] 
			{
				typeof(Java.IO.IOException),
				typeof(Java.Lang.ClassNotFoundException)
			}
		)]
		private void ReadObjectDummy(Java.IO.ObjectInputStream source)
		{
			System.Console.WriteLine("I'm in ReadObject");
			IsPls = source.ReadBoolean ();
			Title = source.ReadUTF();
			URL = source.ReadUTF();
			RealTitle.CopyTo ((string[])source.ReadObject ());
			RealURL.CopyTo ((string[])source.ReadObject ());
		}

		[Export("writeObject", Throws = new[] 
			{
				typeof(Java.IO.IOException),
				typeof(Java.Lang.ClassNotFoundException)
			}
		)]
		private void WriteObjectDummy(Java.IO.ObjectOutputStream destination)
		{
			System.Console.WriteLine("I'm in WriteObject");
			//destination.WriteObject (this);
			destination.WriteBoolean (IsPls);
			destination.WriteUTF (Title);
			destination.WriteUTF (URL);
			destination.WriteObject (RealTitle.ToArray());
			destination.WriteObject (RealURL.ToArray());


		}

	}
}

;