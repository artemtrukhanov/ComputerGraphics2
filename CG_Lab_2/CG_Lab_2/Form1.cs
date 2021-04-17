using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CG_Lab_2
{
    public partial class Form1 : Form
    {
        Bin bin;
        View view;
        bool loaded = false;
        int currentLayer = 0;
        int FrameCount;
        DateTime NextFPSUpdate = DateTime.Now.AddSeconds(1);
        bool needReload = false;

        public Form1()
        {
            InitializeComponent();
            bin = new Bin();
            view = new View();
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            while (glControl1.IsIdle)
            {
                DisplayFPS();
                glControl1.Invalidate();
            }
        }

        void DisplayFPS()
        {
            if (DateTime.Now >= NextFPSUpdate)
            {
                this.Text = String.Format("CT Visualizer (fps={0})", FrameCount);
                NextFPSUpdate = DateTime.Now.AddSeconds(1);
                FrameCount = 0;
            }
            FrameCount++;
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string str = dialog.FileName;
                bin.readBIN(str);
                trackBar1.Maximum = Bin.Z - 1;
                view.SetupView(glControl1.Width, glControl1.Height);
                loaded = true;
                glControl1.Invalidate();
            }
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (loaded)
            {
                if (radioButton1.Checked)
                {
                    view.DrawQuads(currentLayer);
                }
                else if (radioButton2.Checked)
                {
                    if (needReload)
                    {
                        view.generateTextureImage(currentLayer);
                        view.Load2Dexture();
                        needReload = false;
                    }
                    view.DrawTexture();
                }
                else if (radioButton3.Checked)
                {
                    view.DrawQuadStrips(currentLayer);
                }
                glControl1.SwapBuffers();
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            currentLayer = trackBar1.Value;
            needReload = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Application.Idle += Application_Idle;
            ToolTip toolTip1 = new ToolTip();
            toolTip1.AutoPopDelay = 5000;
            toolTip1.InitialDelay = 1000;
            toolTip1.ReshowDelay = 500;
            toolTip1.ShowAlways = true;
            toolTip1.SetToolTip(this.trackBar2, "Transfer function minimum");
            toolTip1.SetToolTip(this.trackBar3, "Transfer function width");
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            view.SetTFMin(trackBar2.Value);
            needReload = true;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            view.SetTFWidth(trackBar3.Value * 20);
            needReload = true;
        }
    }
}
