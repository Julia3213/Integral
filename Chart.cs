using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Integral
{
    public partial class Chart : Form
    {
        Form1 mainForm;
        AngouriMath.Core.FastExpression fu;
        double a;
        double b;
        Pen pen;
        int minVX, maxVX;
        double moveX, moveY;
        double p;
        Graphics g;
        public Chart()
        {
            InitializeComponent();
        }

        public Chart(Form1 f, AngouriMath.Core.FastExpression fu,double a,double b)
        {

            int dif = (int)Math.Abs(Math.Abs(b) - Math.Abs(a));
            minVX = 0 - (int)Math.Abs(a) - dif;
            maxVX = -minVX;
            pen = new Pen(Color.Black);
            mainForm = f;
            this.fu = fu;
            this.a = a;
            this.b = b;
            InitializeComponent();
            pictureBox1.Width = 1500;
            pictureBox1.Height = 900;
            Bitmap bm = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = bm;
            g = pictureBox1.CreateGraphics();
        }

        public void drawAxes(double min, double max, double propX, double propY,Graphics g) {

            g.DrawLine(pen, 0, (int)(pictureBox1.Height / 2-moveY), pictureBox1.Width, (int)(pictureBox1.Height / 2-moveY));
            g.DrawLine(pen, (int)(pictureBox1.Width / 2+moveX), 0, (int)(pictureBox1.Width / 2+moveX), pictureBox1.Height);

            SolidBrush br = new SolidBrush(Color.Black);
            double k = 1;
            if (max >= 20)
                k = 5;
            if (max >= 50)
                k = 10;
            if (max >= 100)
                k = 25;
            if (max <= 2)
                k = 0.5;
            if (max < 1)
                k = 0.025;
            if (max < 0.5)
                k = 0.01;
            for (double i = min; i < max; i+=k)
            {
                g.DrawLine(new Pen(Color.FromArgb(15,Color.Black)), new Point(0, (int)(pictureBox1.Height / 2 - (i * propY)-moveY)),new Point(pictureBox1.Width, (int)(pictureBox1.Height / 2 - i * propY - moveY)));
                g.DrawLine(new Pen(Color.FromArgb(15, Color.Black)), new Point((int)(pictureBox1.Width / 2 + i * propX + moveX), 0), new Point((int)(pictureBox1.Width / 2 + i * propX + moveX), pictureBox1.Height));

                g.DrawString("|", new Font("Courier New", 6), br, new Point((int)(pictureBox1.Width / 2 + i * propX - 5), (int)(pictureBox1.Height / 2 - 10 - moveY)));
                g.DrawString(Math.Round(i,2) + "", new Font("Courier New", 6), br, new Point(pictureBox1.Width / 2 + (int)(i * propX), (int)(pictureBox1.Height / 2 - moveY)));

                g.DrawString("-", new Font("Courier New", 6), br, new Point((int)(pictureBox1.Width / 2 - 5 + moveX), (int)(pictureBox1.Height / 2 - i * propY - 7 - moveY)));
                g.DrawString(Math.Round(i,2) + "", new Font("Courier New", 6), br, new Point((int)(pictureBox1.Width / 2 + moveX), (int)(pictureBox1.Height / 2 - i * propY - moveY)));
            }
        }
        public void drawChart(AngouriMath.Core.FastExpression f,string function,double a,double b,double min,double max) {
            //min and max values 
            if (min == 0 && max == 0)
            {
                double dif = Math.Abs(Math.Abs(b) - Math.Abs(a));
                min = 0 - Math.Abs(a) - dif;
                max = -min;
            }

            double prop = pictureBox1.Width / (max - min);
            double propY = pictureBox1.Height / (max - min);

            g = Graphics.FromImage(pictureBox1.Image);
            g.Clear(Color.White);

            //axes
            drawAxes(min, max, prop, propY,g);

            //integration area
            SolidBrush sb = new SolidBrush(Color.FromArgb(20,Color.Blue));
            g.FillRectangle(sb, new Rectangle((int)(pictureBox1.Width / 2 + a * prop), 0, (int)((b - a)*prop), pictureBox1.Height));

            if (Math.Round(Math.Abs(f.Call(min).Imaginary),3) !=0)
            {
                MessageBox.Show(f.Call(min).Real +"+"+ f.Call(min).Imaginary + "");
                min = 0;
            }
            List<Point> points = new List<Point>();
            for (double i = min; i < max; i+=0.01) {//0.01
                if (i == 0) {
                    if (double.IsNaN(f.Call(i).Real))
                        continue;
                }
                points.Add(new Point((int)(pictureBox1.Width / 2 + ((float)(i) * prop) + moveX), (int)(pictureBox1.Height / 2 - ((float)f.Call(i).Real * propY) - moveY)));
            }
            g.DrawPolygon(pen, points.ToArray());
            g.Dispose();
            pictureBox1.Invalidate();
            pictureBox1.Update();
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
        }

        private void createChartButton_Click(object sender, EventArgs e)
        {
            drawChart(fu, "", a, b,0,0);
            
        }


        private void Chart_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            MessageBox.Show(e.X + " " + e.Y);
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            p = trackBarZoom.Value * 1.00 / 100 * 1.00;
            moveX = -((pictureBox1.Width / 2) / 5) * hScrollBar1.Value;
            drawChart(fu, "", a, b, minVX / p, maxVX / p);
        }
        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            p = trackBarZoom.Value * 1.00 / 100 * 1.00;
            moveY = ((pictureBox1.Height / 2) / 5) * vScrollBar1.Value;
            drawChart(fu, "", a, b, minVX/p, maxVX/p);
        }

        private void trackBarZoom_Scroll(object sender, EventArgs e)
        {
            p = trackBarZoom.Value*1.00 / 100*1.00;
            label2.Text = trackBarZoom.Value + "%";
            drawChart(fu, "", a, b, minVX/p, maxVX/p);
        }

        private void save_Click(object sender, EventArgs e) {
            saveFileDialog1.Filter = "PNG(*.PNG)|*.png";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {

                pictureBox1.Image.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Png);
            }
        }
    }
}


