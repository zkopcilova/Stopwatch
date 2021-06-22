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
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            timer1.Start();
            startButton.Text = "Start";

            if (stopped) //pokud bylo stisknuto tlačítko "Stop", spustí se stopky znova od nuly
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
            timeDisplay.Text = string.Format("{0:mm\\:ss\\.ff}", stopky.Elapsed);
        }

        private void pauseButton_Click(object sender, EventArgs e)
        {
            stopky.Stop(); //stopky se pozastaví
            startButton.Text = "Resume";
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            stopky.Stop(); //stopky se pozastaví
            stopped = true; //při stisknutí "Start" nepokračují od poslední hodnoty, ale od nuly
            startButton.Text = "Start";
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            stopky.Reset();
        }

        private void saveButton_Click(object sender, EventArgs e )
        {
            //funkce uloží do souboru naměřený pojmenovaný čas
            
            if (timeName.Text != "")
            {
                FileStream fileStream = new FileStream(".\\savedTimes.Xml", FileMode.Create, FileAccess.Write);
                savedTime st = new savedTime();
                st.name = timeName.Text;
                st.time = timeDisplay.Text;
                timeName.Text = "";
                list.Add(st);
                xs.Serialize(fileStream, list);
                fileStream.Close();
                viewSaved();
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Enter a name of the measured value.");
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
        }

        private void deleteSaved_Click(object sender, EventArgs e)
        {
            if (File.Exists(".\\savedTimes.Xml"))
            {
                list.Clear(); //vymaže všechny uložené časy
                File.Delete(".\\savedTimes.Xml");
                dataView.DataSource = null;
                dataView.Refresh();
            }
        }
    }
}
