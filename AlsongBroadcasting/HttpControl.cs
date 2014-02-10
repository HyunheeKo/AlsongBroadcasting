using System;

using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Text;


namespace AlsongBroadcasting
{
	public class HttpControl
	{
		List<BroadcastingInfo> listBroadcasting = new List<BroadcastingInfo>();


		public HttpControl ()
		{
		}

		public string GetWeb(string strURL, Encoding encoding)
		{
			string responseFromServer = "";

			try{
				// Create a request for the URL. 		
				WebRequest request = WebRequest.Create (strURL);
				// If required by the server, set the credentials.
				request.Credentials = CredentialCache.DefaultCredentials;
				// Get the response.
				HttpWebResponse response = (HttpWebResponse)request.GetResponse ();
				// Display the status.
				Console.WriteLine (response.StatusDescription);
				// Get the stream containing content returned by the server.
				System.IO.Stream dataStream = response.GetResponseStream ();
				// Open the stream using a StreamReader for easy access.
				StreamReader reader = new StreamReader (dataStream, encoding);
				// Read the content.
				responseFromServer = reader.ReadToEnd ();
				// Display the content.
				//Console.WriteLine (responseFromServer);

				// Cleanup the streams and the response.
				reader.Close ();
				dataStream.Close ();
				response.Close ();

			} catch (Exception ex) {
				Console.WriteLine ("GetWeb Error : " + ex.Message);
			}

			return responseFromServer;
		}


		public List<BroadcastingInfo> GetBroadcastingList()
		{
			listBroadcasting.Clear ();

			//"http://onlinemusic.alsong.co.kr/ALSongBroadcastOnAir.aspx"

			// Parse Broadcasting List
			ParseBroadcastingList (GetWeb("http://onlinemusic.alsong.co.kr/ALSongBroadcastOnAir.aspx", Encoding.UTF8));
	
			//return listBroadcasting;

//			// Get the pls file
//			foreach (BroadcastingInfo bi in listBroadcasting) {
//				if (bi.IsPls) {
//					//
//					ParsePls( GetWeb (bi.URL), bi);
//				}
//			}

			return listBroadcasting;
		}

		private void ParseBroadcastingList(string strHtml)
		{
			int nOffset = 0;
			try
			{
				for (int i = 1; i < 100; i++) {
					string strFindString = "<td>" + i + "</td>";

					// Step 1. Find number for Html string
					nOffset = strHtml.IndexOf (strFindString);

					// 
					if (nOffset > 0) {
						// Step 2. Find URL
						strFindString = "javascript:linkFunctionByValue";
						nOffset = strHtml.IndexOf (strFindString, nOffset);

						if (nOffset > 0) {
							// Step 2-1. Move Offset Because ...
							nOffset += 60;
							//Step 2-2. Find Url start index
							strFindString = "','";
							int nTrimCount = strFindString.Length;

							int nStart = strHtml.IndexOf (strFindString, nOffset);
							strFindString = "');";


							int nEnd = strHtml.IndexOf (strFindString, nStart);

							Console.WriteLine("nStart 1 : " + nStart + " nEnd : " + nEnd + " nTrimCount : " + nTrimCount );
							if(nEnd < 0)
							{
								Console.WriteLine(strHtml);
							}
							//Step 2-3. Take URL
							string strUrl = strHtml.Substring(nStart + nTrimCount, nEnd - (nStart + nTrimCount));
							Console.WriteLine("URL : " + strUrl);

							nOffset = nEnd;

							//Step 3. Get the Title
							strFindString = ">";
							nStart = strHtml.IndexOf (strFindString, nOffset);
							strFindString = "</a>";
							nEnd = strHtml.IndexOf (strFindString, nOffset);
							//Step 3-3. Take URL
							string strTitle = strHtml.Substring(nStart + 1, nEnd - (nStart + 1));
							Console.WriteLine("Title : " + strTitle);

							BroadcastingInfo bi = new BroadcastingInfo();

							if(!strUrl.StartsWith("http://") )
							{
								strUrl = "http://" + strUrl;
							}

							bi.URL = strUrl;
							bi.Title = strTitle;
							if(strUrl.EndsWith( "pls"))
							{
								bi.IsPls = true;
								Console.WriteLine("This Url pls : " + strUrl);
							}


							listBroadcasting.Add(bi);

						} else {
							break;
						}
					} else {
						break;
					}
				}


			} catch (Exception ex) {
				Console.WriteLine("GetBroadcastingList Error : " + ex.Message);
			}
		}
		

		public BroadcastingInfo ParsePls(string strPlsFile, BroadcastingInfo  bi)
		{
			//int nOffset = 0;
			Console.WriteLine ("^^ Source : " + strPlsFile);
			try
			{
				string strFindString = "NumberOfEntries=";
				string strLineFeed1 ="\n";
				string strLineFeed2 ="\r\n";

				strPlsFile = strPlsFile.ToLower();
				strFindString = strFindString.ToLower();

				int nStart = strPlsFile.IndexOf (strFindString);
				int nEnd = strPlsFile.IndexOf (strLineFeed1, nStart);
				int nCount = 0;

				if (nStart > 0 && nEnd > 0) {
					string tmp = strPlsFile.Substring(nStart + strFindString.Length, nEnd - (nStart + strFindString.Length));
					nCount = int.Parse(tmp);
				}

				Console.WriteLine("- Count : " + nCount);

				nStart = 0;

				// Url Extraction
				for (int i = 1; i <= nCount; i++) {
					int nStart2 = 0;
					int nEnd2 = 0;
					strFindString = "File" + i;
					strFindString = strFindString.ToLower();

					nStart2 = strPlsFile.IndexOf (strFindString, nStart2);
					nStart2 = strPlsFile.IndexOf ("=", nStart2);

					// 줄바꿈 문자를 찾는다. 2가지가 존하므로 2번 검사해준다.
					// 제목은 상관없고 URL부분만 해주면 된다.
					nEnd2 = strPlsFile.IndexOf (strLineFeed2, nStart2);
					if(nEnd2 == -1)
					{
						nEnd2 = strPlsFile.IndexOf (strLineFeed1, nStart2);
					}
					if(nStart2 > 0 && nEnd2 > 0)
					{
						string tmp = strPlsFile.Substring(nStart2 + 1, nEnd2-(nStart2 + 1));
						bi.RealURL.Add(tmp);
						Console.WriteLine("- URL : " + tmp);
						nStart2 = nEnd2;
					}
				}


				// Title Extraction
				for (int i = 1; i <= nCount; i++) {
					int nStart3 = 0;
					int nEnd3 = 0;
					strFindString = "Title" + i ;
					strFindString = strFindString.ToLower();

					nStart3 = strPlsFile.IndexOf (strFindString, nStart3);
					if ( nStart3 >= 0)
					{
						//Console.WriteLine("- Next1 : " + i + " index : " + nStart3);
						nStart3 = strPlsFile.IndexOf ("=", nStart3);
						//Console.WriteLine("- Next2 : " + i + " index : " + nStart3);
						nEnd3 = strPlsFile.IndexOf (strLineFeed1, nStart3);
						if(nStart3 > 0 && nEnd3 > 0)
						{
							string tmp = strPlsFile.Substring(nStart3 + 1, nEnd3-(nStart3 + 1));

							bi.RealTitle.Add(tmp);
							Console.WriteLine("- Title : "+tmp);
							nStart3 = nEnd3;
						}
					} else {
						Console.WriteLine("Not found title  : " + strFindString);
						nStart3 = 0;
					}
				}

			} catch (Exception ex) {
				Console.WriteLine("ParsePls Error : " + ex.Message);
			}
			return bi;

		}


	}
}
