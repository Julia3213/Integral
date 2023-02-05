using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Integral
{
    public partial class Chart : Form
    {
        Form1 mainForm;
        AngouriMath.Core.FastExpression fu;
        double a;
        double b;
        double br;
        Pen pen;
        int minVX, maxVX;
        double moveX, moveY;
        double min = 0, max = 0, propX,propY;
        double p;
        const int chartWidth=1500, chartHeight=900;
        Graphics g;
        int v = 100;
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

        //отрисовка осей графика
        public void drawAxes(Graphics g) {
            g.DrawLine(pen, 0, (int)(chartHeight / 2 - moveY), chartWidth, (int)(chartHeight / 2 - moveY));
            g.DrawLine(pen, (int)(chartWidth / 2 + moveX), 0, (int)(chartWidth / 2 + moveX), chartHeight);

            SolidBrush br = new SolidBrush(Color.Black);
            double k = 1;
            if (max >= 20)
                k = 5;
            if (max >= 50)
                k = 10;
            if (max >= 100)
                k = 25;
            if (max >= 1000)
                k = 100;
            if (max >= 5000)
                k = 200;
            if (max >= 50000)
                k = 1000;
            if (max >= 100000)
                k = 2500;
            if (max <= 2)
                k = 0.5;
            if (max < 1)
                k = 0.025;
            if (max < 0.5)
                k = 0.01;
            for (double i = min; i < max; i += k)
            {
                g.DrawLine(new Pen(Color.FromArgb(15, Color.Black)), new Point(0, (int)(chartHeight / 2 - (i * propY) - moveY)), new Point(chartWidth, (int)(chartHeight / 2 - i * propY - moveY)));
                g.DrawLine(new Pen(Color.FromArgb(15, Color.Black)), new Point((int)(chartWidth / 2 + i * propX + moveX), 0), new Point((int)(chartWidth / 2 + i * propX + moveX), chartHeight));

                g.DrawString("|", new Font("Courier New", 6), br, new Point((int)(chartWidth / 2 + i * propX - 5 + moveX), (int)(chartHeight / 2 - 10 - moveY)));
                g.DrawString(Math.Round(i, 2) + "", new Font("Courier New", 6), br, new Point((int)(chartWidth / 2 + i * propX+moveX), (int)(chartHeight / 2 - moveY)));

                g.DrawString("-", new Font("Courier New", 6), br, new Point((int)(chartWidth / 2 - 5 + moveX), (int)(chartHeight / 2 - i * propY - 7 - moveY)));
                g.DrawString(Math.Round(i, 2) + "", new Font("Courier New", 6), br, new Point((int)(chartWidth / 2 + moveX), (int)(chartHeight / 2 - i * propY - moveY)));
            }
        }

        //вычиление точек графика
        public Point[] countChart() {
            List<Point> points = new List<Point>();
            double k=1;
            if (max >= 200)
                k = 5;
            if (max >= 10000)
                k = 10;
            if (max <= 50)
                k = 0.5;
            if (max <= 30)
                k = 0.01;
            bool isBr = false;
            for (double i = min; i < max; i += k)
            {
                if (double.IsNaN(fu.Call(Math.Round(i, 3)).Real) || double.IsInfinity(fu.Call(Math.Round(i, 3)).Real) || Math.Round(fu.Call(Math.Round(i, 3)).Imaginary, 3) != 0)
                {
                    isBr = true;
                    br = (int)(((i)*propX)+moveX+pictureBox1.Width/2);
                    continue;
                }
                if (!isBr) {
                    if (i > 0 && i - k < 0) {
                        br = (int)((i) * propX + moveX + pictureBox1.Width / 2);
                    }
                }
                points.Add(new Point((int)(pictureBox1.Width / 2 + (i * propX) + moveX), (int)(pictureBox1.Height / 2 - (fu.Call(i).Real * propY) - moveY)));
            }

            return points.ToArray();

        }
        
        //отрисовка графика
        public void drawChart() {
            g = Graphics.FromImage(pictureBox1.Image);
            g.Clear(Color.White);
            drawAxes(g);
            double k = 1;
            if (max >= 200)
                k = 3;
            if (max >= 10000)
                k = 10;
            if (max <= 50)
                k = 0.5;
            if (max <= 30)
                k = 0.01;
            if (max <= 10)
                k = 0.001;
            SolidBrush sb = new SolidBrush(Color.FromArgb(20, Color.Blue));
            for (double i = a; i < b; i+=k) {
                Point p1 = new Point((int)(i * propX + moveX + chartWidth / 2), (int)(chartHeight / 2 - (fu.Call(i).Real * propY) - moveY));
                Point p2 = new Point((int)(i * propX + moveX + chartWidth / 2), (int)(chartHeight / 2 - moveY));
                try
                {
                    g.DrawLine(new Pen(Color.FromArgb(40, Color.Blue)), p1, p2);
                }
                catch (OverflowException) {
                    continue;
                }
            }
            Point[] points = countChart();
            progressBar1.Maximum = points.Length;
            for (int i = 1; i < points.Length; i++) {
                if (points[i].X!=br)
                {
                    progressBar1.Value = i;
                    try
                    {
                        g.DrawLine(pen, points[i - 1], points[i]);
                    }
                    catch (OverflowException) {
                        trackBarZoom.Value = v;
                        MessageBox.Show("The interval is too long. The graph is not informative");
                        progressBar1.Value = points.Length;
                        return;
                    }
                }
            }
            g.Dispose();
            pictureBox1.Invalidate();
            pictureBox1.Update();
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
        }

        //кнопка "Построить график"
        private void createChartButton_Click(object sender, EventArgs e)
        {
            if (min == 0 && max == 0)
            {
                double dif = Math.Abs(Math.Abs(b) - Math.Abs(a));
                min = 0 - Math.Abs(a) - dif;
                max = -min;
            }

            propX = chartWidth / (max - min);
            propY = chartHeight / (max - min);

            drawChart();
        }


        private void Chart_Load(object sender, EventArgs e)
        {

        }


        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            p = trackBarZoom.Value * 1.00 / 100 * 1.00;
            min = minVX / p;
            max = maxVX / p;
            propX = pictureBox1.Width / (max - min);
            propY = pictureBox1.Height / (max - min);
            moveX = -((pictureBox1.Width / 2) / 5) * hScrollBar1.Value;

            drawChart();
        }
        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            p = trackBarZoom.Value * 1.00 / 100 * 1.00;
            min = minVX / p;
            max = maxVX / p;
            moveY = ((pictureBox1.Height / 2) / 5) * vScrollBar1.Value; 
            propX = pictureBox1.Width / (max - min);
            propY = pictureBox1.Height / (max - min);
            drawChart();
        }

        private void trackBarZoom_Scroll(object sender, EventArgs e)
        {
            p = trackBarZoom.Value*1.00 / 100*1.00;
            min = minVX / p;
            max = maxVX / p;
            label2.Text = trackBarZoom.Value + "%";
            propX = pictureBox1.Width / (max - min);
            propY = pictureBox1.Height / (max - min);
            drawChart();
            v = trackBarZoom.Value;
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