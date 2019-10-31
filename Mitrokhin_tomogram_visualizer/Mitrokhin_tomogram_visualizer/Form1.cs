using System;
using System.Windows.Forms;

namespace Mitrokhin_tomogram_visualizer
{
    public partial class Form1 : Form
    {

        Bin bin = new Bin();
        View view = new View();
        bool loaded = false; //не запускать отрисовку пока нет данных
        bool needReload = false;// когда не изменяем трекбар
        int currentLayer = 0;
        int FrameCount;
        DateTime NextFPSUpdate = DateTime.Now.AddSeconds(1);

        public Form1()
        {
            InitializeComponent();
            trackBar1.Scroll += trackBar1_Scroll;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            currentLayer = trackBar1.Value;
            needReload = true;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string str = dialog.FileName;
                bin.readBIN(str);
                trackBar1.SetRange(0, Bin.Z - 1);
                trackBar2.SetRange(0, Bin.Z - 1);
                trackBar3.SetRange(0, Bin.Z - 1);
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
                if (radioButton2.Checked)
                {
                    if (needReload)
                    {
                        view.generateTextureImage(currentLayer);
                        view.Load2DTexture();
                        needReload = false;
                    }
                    view.DrawTexture();
                }
                glControl1.SwapBuffers();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Application.Idle += Application_Idle;
        }
        void Application_Idle(object sender, EventArgs e) //проявляет на занятость окно формы
        {
            while (glControl1.IsIdle)
            {
                displayFPS();
                glControl1.Invalidate(); //заставлет снова рендериться
            }
        }
        void displayFPS() //функция обновления fps
        {
            if (DateTime.Now >= NextFPSUpdate)
            {
                Text = String.Format("CT Visualizer (fps = {0})", FrameCount);
                NextFPSUpdate = DateTime.Now.AddSeconds(1);
                FrameCount = 0;
            }
            FrameCount++;
        }
        private void trackBar2_Scroll(object sender, EventArgs e)// min
        {
            view.SetMinMaxTransferFunction(trackBar2.Value, trackBar2.Value + trackBar3.Value);
            needReload = true;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            view.SetMinMaxTransferFunction(trackBar2.Value, trackBar2.Value + trackBar3.Value);
            needReload = true;
        }
    }
}
