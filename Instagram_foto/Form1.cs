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
					
					string response = req.Get(url).ToString();

					string[] imgs = req.Get(url).ToString().Substrings("\"display_url\":\"", "\"");
					string[] vids = req.Get(url).ToString().Substrings("\"video_url\":\"", "\"");

					//Убираем фото превью видео
					if (vids.Length > 0 && !all)
					{
						List<string> temp = imgs.ToList();
						temp.RemoveRange(imgs.Length - vids.Length, vids.Length);
						imgs = temp.ToArray();
					}

					WebClient client = new WebClient();
					
					//Складываем два массива
					string[] arr = imgs.Concat(vids).ToArray();
					int start = (imgs.Length > 1) ? 1 : 0;

					//Все фотографии
					if (all)
					{
						for (int i = start; i < arr.Length; i++)
						{
							Uri uri = new Uri(arr[i]);
							client.DownloadFile(uri, uri.Segments[uri.Segments.Length - 1]);
						}
					}
					//Одна фотография
					if (!all)
					{
						Uri uri = new Uri(arr[0]);
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