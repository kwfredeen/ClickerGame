using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        string[] savelines = { "CLICKS = 0", "DTAP = 0", "DTAPU = 50", "ACLICK = 0", "ACLICKU = 1000", "ASPEED = 0" };

        Int64 clicks = 0;

        int DTap = 0;
        int DTapUpCost = 50; //Change as balance requires
        int DTapUpInit = 50;
        int DTapUpMult = 2; //Change as balance requires

        int AClick = 0;
        int AClickUpCost = 1000; //Change as balance requires
        int AclickUpInit = 1000;
        int AClickUpMult = 10; //Change as balance requires

        public Form1()
        {
            InitializeComponent();
        }

        private Int64 processLine(int lineNum)
        {
            string line = savelines[lineNum];
            int beginPos = line.LastIndexOf(' ');

            return Convert.ToInt64(line.Substring(beginPos)) ;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //debug console
            //AllocConsole();
            Console.WriteLine("Console initialized.");

            //Check if a savefile exists
            if (!System.IO.File.Exists(@"..\save.txt"))
            {
                //if not, create one
                System.IO.File.WriteAllLines(@"..\save.txt", savelines);
                Console.WriteLine("Created new save file.");
            }

            savelines = System.IO.File.ReadAllLines(@"..\save.txt");

            //load clicks
            clicks = processLine(0);

            //load DTap
            DTap = (int) processLine(1);
            DTapUpCost = (int) processLine(2);

            //Load AClick
            AClick = (int) processLine(3);
            AClickUpCost = (int) processLine(4);

            Console.WriteLine("Loaded from save file");
            
            //Begin AClick proces
            backgroundWorker1.RunWorkerAsync();

            //Render text objects
            label1.Text = "Count: " + clicks.ToString();

            button2.Text = "Double Click: " + DTap.ToString();
            label2.Text = "Next Upgrade: " + DTapUpCost.ToString();
            if (DTap > 0 | clicks > DTapUpInit)
            {
                button2.Visible = true;
                label2.Visible = true;
            }

            button4.Text = "Auto Click: " + AClick.ToString();
            label3.Text = "Next Upgrade: " + AClickUpCost.ToString();
            if (AClick > 0 | clicks > AclickUpInit)
            {
                button4.Visible = true;
                label3.Visible = true;
            }

        }

        private void Button1_Click(object sender, EventArgs e) //Click button
        {
            clicks  = clicks + (int) Math.Pow(2, DTap);

            if (clicks == 666)
            {
                label1.Text = "Count: s8n";
            }
            if (clicks >= DTapUpInit)
            {
                button2.Visible = true;
                label2.Visible = true;
            }
            if (clicks >= AclickUpInit)
            {
                button4.Visible = true;
                label3.Visible = true;
            }

            label1.Text = "Count: " + clicks.ToString();
        }

        private void DTapClick(object sender, EventArgs e) //Double Click Button
        {
            //Dtap stuff here
            if (clicks < DTapUpCost)
            {
                //Maybe display a message in the future using a blank label next to it?
                return;
            }
            clicks = clicks - DTapUpCost;
            DTapUpCost = DTapUpCost * DTapUpMult; //change with balance
            DTap++;

            label1.Text = "Count: " + clicks.ToString();
            button2.Text = "Double Click: " + DTap.ToString();
            label2.Text = "Next Upgrade: " + DTapUpCost.ToString();
        }

        private void SaveClick(object sender, EventArgs e)
        {
            savelines[0] = "CLICKS = " + clicks.ToString();
            savelines[1] = "DTAP = " + DTap.ToString();
            savelines[2] = "DTAPU = " + DTapUpCost.ToString();
            savelines[3] = "ACLICK = " + AClick.ToString();
            savelines[4] = "ACLICKU = " + AClickUpCost.ToString();

            System.IO.File.WriteAllLines(@"..\save.txt", savelines);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void AClick_Click(object sender, EventArgs e)
        {
            if (clicks < AClickUpCost)
            {
                //Maybe display a message in the future using a blank label next to it?
                return;
            }

            clicks = clicks - AClickUpCost;
            AClickUpCost = AClickUpCost * AClickUpMult;
            AClick++;

            label1.Text = "Count: " + clicks.ToString();
            button4.Text = "Auto Click: " + AClick.ToString();
            label3.Text = "Next Uprade: " + AClickUpCost.ToString();
        }
            
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            
            while (true)
            {
                System.Threading.Thread.Sleep(1000);
                if (AClick == 0)
                {
                    //do nothing
                }
                else
                {
                    clicks = clicks + (int)Math.Pow(2, AClick - 1);
                }
                worker.ReportProgress(0);
            }
            
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            label1.Text = "Count: " + clicks.ToString();
        }
    }
}
