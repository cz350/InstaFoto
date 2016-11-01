using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using xNet;
using System.Net;
using System.IO;

namespace Instagram_foto
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();			
		}

		private void button1_Click(object sender, EventArgs e)
		{
			string url = urlBox.Text;

			if (TestUrl(url) == false)
			{
				MessageBox.Show("Вставьте ссылку на Instagram");
				return;
			}

			try
			{
				button1.Enabled = false;

				using (var req = new HttpRequest())
				{
					req.UserAgent = Http.ChromeUserAgent();
					CookieDictionary cookies = new CookieDictionary(false);
					req.Cookies = cookies;

					string get_html = req.Get(url).ToString();

					//получем ссылку на изображение
					string src_img = get_html.Substring("<meta property=\"og:image\" content=\"", "?");

					WebClient client = new WebClient();
					Uri uri = new Uri(src_img);

					client.DownloadFileAsync(uri, uri.Segments[uri.Segments.Length - 1]);
				}				
			}
			catch (Exception ex)
			{				
				MessageBox.Show("Возникла ошибка.\nПроверьте правильность ссылки...\n" + ex.Message.ToString());				
			}
			finally
			{
				urlBox.Text = "";
				urlBox.Focus();
				button1.Enabled = true;
			}
		}

		private bool TestUrl(string url)
		{			
			return (url.Contains("instagram.com/p/") && url != String.Empty) ? true : false;				
		}
	}
}