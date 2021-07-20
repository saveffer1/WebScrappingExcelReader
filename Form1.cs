using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Windows;

namespace SFFScrapWeb
{
    public partial class Form1 : Form
    {
        

        string keepcool;
        public Form1()
        {
            InitializeComponent();
            rtb_debugDisplay.HideSelection = false;
        }

        public async Task GetDataFromWebPage()
        {
            //string fullUrl = "https://coinmarketcap.com/";
            string fullUrl = string.Format("http://{0}/old/table/stationtable.php", inputbox.Text);
            rtb_debugDisplay.AppendText("Scraping from: " + fullUrl);
            rtb_debugDisplay.AppendText("\n");
            HttpClient client = new HttpClient();
            var response = await client.GetStringAsync(fullUrl);

            ParseHtml(response);
        }
        public void ParseHtml(string htmlData)
        {
            keepcool = "";
            HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml(htmlData);

            var table = htmlDocument.DocumentNode.Descendants("tr");
            //var data = new Dictionary<string, string>();
            //var data2 = new Dictionary<string, string>();
            //var data3 = new Dictionary<string, string>();
            var csvBuilder = new StringBuilder();
            int i = 0;
            csvBuilder.AppendLine("Names,CPU Workload %,CPU Temp C,RAM Usage %,Last Update Date,Last Update Time");
            foreach (var row in table)
            {
                i = i + 1;
                string[] words = row.InnerText.Split(' ');
                if (i != 1)
                {
                    csvBuilder.AppendLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\"", words[0], words[1], words[3], words[5], words[7], words[8]));
                    rtb_debugDisplay.AppendText("\r" + row.InnerText);
                }
            }
            string fullUrl = string.Format("http://{0}/old/table/stationtable.php", inputbox.Text);
            rtb_debugDisplay.AppendText("Scraped from: " + fullUrl);
            keepcool = csvBuilder.ToString().TrimStart();
        }

        private void savefile()
        {
            string dummyFileName = "Report_" + DateTime.Now.ToString("dd_MM_yyyy");

            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "CSV|*.csv";
            sf.Title = "Report";
            sf.FileName = dummyFileName;
            if (sf.ShowDialog() == DialogResult.OK)
            {
                string savePath = Path.GetDirectoryName(sf.FileName);
                rtb_debugDisplay.AppendText("\nSaved to " + savePath);
                File.WriteAllText(sf.FileName, keepcool);
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            savefile();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            GetDataFromWebPage();
        }
        private void excelbtn_Click(object sender, EventArgs e)
        {
            //System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            Process.Start(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + @"\ExcelReader\Reader.exe");
        }
        private void button1_Click(object sender, EventArgs e)
        {
            rtb_debugDisplay.Text = "";
            keepcool = "";
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        /*
        private void btnExit_Click(object sender, EventArgs e)
        {
            foreach (var process in Process.GetProcessesByName("TestApp"))
            {
                process.Kill();
            }
        }
        */
    }
}
