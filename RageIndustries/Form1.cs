using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;

namespace RageIndustries
{
    public partial class Form1 : Form
    {
        [DllImport("User32.dll")]
        public static extern short GetAsyncKeyState(Keys vKey);

        [DllImport("User32.dll")]
        public static extern long SetCursorPos(int x, int y);

        [DllImport("User32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;

        public static int ScreenXReq = 110;
        public static int ScreenYReq = 110;

        Bitmap screenshot = new Bitmap(ScreenXReq, ScreenYReq);
        Bitmap screenshot2 = new Bitmap(ScreenXReq, ScreenYReq);

        Graphics graphics;

        public int MissX;
        public int MissY;

        public int[] CompareBitmapsLazy(Bitmap bmp1, Bitmap bmp2)
        {
            //Target X i Y
            int[] output = new int[2] { 0, 0 };

            for (int x = 2; x < bmp1.Width-2; x = x + 4)
            {
                for (int y = 2 ; y < bmp1.Height-2; y = y + 4)
                {
                    int RDif = Math.Abs(bmp1.GetPixel(x, y).R - bmp2.GetPixel(x, y).R);
                    int GDif = Math.Abs(bmp1.GetPixel(x, y).G - bmp2.GetPixel(x, y).G);
                    int BDif = Math.Abs(bmp1.GetPixel(x, y).B - bmp2.GetPixel(x, y).B);

                    int R1Dif = Math.Abs(bmp1.GetPixel(x + 1, y).R - bmp2.GetPixel(x, y).R);
                    int G1Dif = Math.Abs(bmp1.GetPixel(x + 1, y).G - bmp2.GetPixel(x, y).G);
                    int B1Dif = Math.Abs(bmp1.GetPixel(x + 1, y).B - bmp2.GetPixel(x, y).B);

                    int R2Dif = Math.Abs(bmp1.GetPixel(x, y + 1).R - bmp2.GetPixel(x, y).R);
                    int G2Dif = Math.Abs(bmp1.GetPixel(x, y + 1).G - bmp2.GetPixel(x, y).G);
                    int B2Dif = Math.Abs(bmp1.GetPixel(x, y + 1).B - bmp2.GetPixel(x, y).B);

                    int R3Dif = Math.Abs(bmp1.GetPixel(x, y - 1).R - bmp2.GetPixel(x, y).R);
                    int G3Dif = Math.Abs(bmp1.GetPixel(x, y - 1).G - bmp2.GetPixel(x, y).G);
                    int B3Dif = Math.Abs(bmp1.GetPixel(x, y - 1).B - bmp2.GetPixel(x, y).B);

                    int R4Dif = Math.Abs(bmp1.GetPixel(x - 1, y).R - bmp2.GetPixel(x, y).R);
                    int G4Dif = Math.Abs(bmp1.GetPixel(x - 1, y).G - bmp2.GetPixel(x, y).G);
                    int B4Dif = Math.Abs(bmp1.GetPixel(x - 1, y).B - bmp2.GetPixel(x, y).B);


                    int Dif = Math.Abs((RDif + GDif + BDif + R1Dif + R2Dif +R3Dif + R4Dif + G1Dif + G2Dif + G3Dif + G4Dif + B1Dif + B2Dif + B3Dif + B4Dif) / 15);

                    int Sens = Int32.Parse(textBox3.Text);

                    if (Dif > Sens)
                    {
                        output[0] = x;
                        output[1] = y;
                    }
                }
            }
            return output;
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Thread Start = new Thread(Main);
            Start.IsBackground = true;
            Start.Start();
        }

        private void Main()
        {
            while (true)
            {
                if (GetAsyncKeyState(Keys.XButton1) < 0)
                {
                    graphics = Graphics.FromImage(screenshot as Image);
                    int SourceX = (Screen.PrimaryScreen.Bounds.Width / 2) - 55;
                    int SourceY = (Screen.PrimaryScreen.Bounds.Height / 2) - 55;
                    int DestX = 0;
                    int DestY = 0;
                    System.Drawing.Size ImgSize = new System.Drawing.Size(ScreenXReq, ScreenYReq);
                    graphics.CopyFromScreen(SourceX, SourceY, DestX, DestY, ImgSize);
                    Bitmap PicBefore = screenshot;
                    Thread.Sleep(10);
                    graphics = Graphics.FromImage(screenshot2 as Image);
                    graphics.CopyFromScreen(SourceX, SourceY, DestX, DestY, ImgSize);
                    Bitmap PicAfter = screenshot2;

                    int[] Target = CompareBitmapsLazy(PicBefore, PicAfter);

                    if (Target[0] != 0 & Target[1] != 0)
                    {
                        Target[1] = Target[1] - 55;
                        Target[0] = Target[0] - 55;
                        mouse_event(0x0001, Target[0]*Int32.Parse(textBox1.Text), Target[1]*Int32.Parse(textBox2.Text), 0, 0);
                        Thread.Sleep(10);
                        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                        Thread.Sleep(5);
                        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                        Thread.Sleep(200);
                    }
                }
            }
        }
        private bool mouseDown;
        private System.Drawing.Point lastLocation;

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = e.Location;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.Location = new System.Drawing.Point(
                    (this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);

                this.Update();
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void label2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.youtube.com/channel/UCiddpR_hZ92qqpioZn4qUhw");
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            return;
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
