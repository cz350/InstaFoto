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
				MessageBox.Show("Проверьте ссылку на пост в Instagram", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}

			try
			{
				button1.Enabled = false;
				bool all = checkBox1.Checked;

				using (var req = new HttpRequest())
				{
					req.UserAgent = Http.ChromeUserAgent();
					CookieDictionary cookies = new CookieDictionary(false);
					req.Cookies = cookies;

					string[] imgs = req.Get(url).ToString().Substrings("\"display_url\": \"", "\"");									

					WebClient client = new WebClient();
					
					int start = (imgs.Length > 1 && all) ? 1 : 0;

					//Все фотографии
					if (start == 1)
					{
						for (int i = start; i < imgs.Length; i++)
						{
							Uri uri = uri = new Uri(imgs[i]);
							client.DownloadFile(uri, uri.Segments[uri.Segments.Length - 1]);
						}
					}
					//Одна фотография
					if (start == 0)
					{
						Uri uri = uri = new Uri(imgs[0]);
						client.DownloadFile(uri, uri.Segments[uri.Segments.Length - 1]);
					}
				}

				//Очищаем текстовое поле после успешного выполнения
				urlBox.Text = "";
			}
			catch (Exception ex)
			{
				MessageBox.Show("Возникла ошибка.\nПроверьте правильность ссылки...\n" + ex.Message.ToString(), "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				urlBox.Focus();
				button1.Enabled = true;
			}
		}

		private bool TestUrl(string url)
		{
			return (url.Contains("instagram.com/p/") && url != String.Empty) ? true : false;
		}
				
		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			string title = "Instagram - загрузка фотографий";
			Form1.ActiveForm.Text = (checkBox1.Checked) ? title + " (режим all)" : title;
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			ToolTip toolTip = new ToolTip();
			toolTip.SetToolTip(checkBox1, "Скачать все фотографии");
		}
	}
}