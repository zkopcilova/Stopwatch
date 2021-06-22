using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml.Serialization;
using System.IO;

namespace Stopky
{
    public partial class Form1 : Form
    {
        Stopwatch stopky;
        bool stopped = false;
        XmlSerializer xs;
        List<savedTime> list;


        public Form1()
        {
            InitializeComponent();

            list = new List<savedTime>();
            xs = new XmlSerializer(typeof(List<savedTime>));
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            stopky = new Stopwatch();
            viewSaved();

            //styl zobrazení uložených časů
            dataView.EnableHeadersVisualStyles = false;
            dataView.AutoResizeColumns();
            dataView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataView.DefaultCellStyle.Font = new Font("Arial", 8);
            dataView.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8);
            dataView.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;

        }


        private void startButton_Click(object sender, EventArgs e)
        {
            timer1.Start();
            startButton.Text = "Start";

            if (stopped) //bylo stisknuto tlačítko "Stop", stopky se spustí znova od nuly
            {
                stopky.Restart();
                stopped = false;
            }
            else //stopky se spustí od aktuální hodnoty
            {
                stopky.Start();
            }
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            /* 
                při změně časovače zobrazí aktuální naměřený čas
                hodiny nejsou zahrnuty, jelikož jsou stopky obvykle užívány na měření intervalů v řádu vteřin až minut,
                jejich zobrazení by tedy bylo zbytečně rušivé
            */
            timeDisplay.Text = string.Format("{0:mm\\:ss\\.ff}", stopky.Elapsed);
        }


        private void pauseButton_Click(object sender, EventArgs e)
        {
            stopky.Stop(); //stopky se pozastaví
            startButton.Text = "Resume"; //tlačítko "Start" přejmenuje na "Resume"
        }


        private void stopButton_Click(object sender, EventArgs e)
        {
            stopky.Stop(); //stopky se pozastaví
            stopped = true; //při stisknutí "Start" nepokračují od poslední hodnoty, ale od nuly
            startButton.Text = "Start";
        }


        private void resetButton_Click(object sender, EventArgs e)
        {
            stopky.Reset(); //vynuluje naměřený čas
        }


        private void saveButton_Click(object sender, EventArgs e )
        {
            //funkce uloží do souboru naměřený pojmenovaný čas
            
            if (timeName.Text != "")
            {
                FileStream fileStream = new FileStream(".\\savedTimes.Xml", FileMode.Create, FileAccess.Write);
                savedTime st = new savedTime();
                st.Name = timeName.Text;
                st.Time = timeDisplay.Text;
                timeName.Text = "";
                list.Add(st);
                xs.Serialize(fileStream, list);
                fileStream.Close();
                viewSaved();
            }
            else //pokud nebyl zadán název ukládaného času
            {
                System.Windows.Forms.MessageBox.Show("Entering a name is required.");
            }
   
        }


        private void viewSaved()
        {
            if (File.Exists(".\\savedTimes.Xml")) //zobrazí existující uložené časy
            {
                FileStream fileStream = new FileStream(".\\savedTimes.Xml", FileMode.Open, FileAccess.Read);
                list = (List<savedTime>)xs.Deserialize(fileStream);

                dataView.DataSource = list;
                fileStream.Close();
            }

            dataView.ClearSelection();
            dataView.CurrentCell = null;
        }


        private void deleteSaved_Click(object sender, EventArgs e)
        {
            if (File.Exists(".\\savedTimes.Xml"))
            {
                list.Clear(); //vymaže všechny uložené časy
                File.Delete(".\\savedTimes.Xml"); //vymaže soubor, do nějž jsou časy ukládány
                dataView.DataSource = null;
                dataView.Refresh();
            }
        }


        private void timeName_Click(object sender, EventArgs e)
        {
            //po kliknutí do pole "Name" odstraní placeholder, text změní na černý
            timeName.Text = "";
            timeName.ForeColor = Color.Black;
        }
    }
}
