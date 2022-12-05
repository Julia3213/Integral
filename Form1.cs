using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using AngouriMath.Extensions;
//22
namespace Integral
{
    public partial class Form1 : Form
    {
        double a, b;
        public Form1()
        {
            InitializeComponent();
            table[0, 0] = "x";
            table[0, 1] = "x^2/2";

            table[1, 0] = "x^a";
            table[1, 1] = "x^(a+1)/(a+1)";

            table[2, 0] = "1/x";
            table[2, 1] = "ln(x)";

            table[3, 0] = "a^x";
            table[3, 1] = "a^x/ln(x)";
        }
        string func;
        string method;
        string[,] table = new string[20,2];
        double integral;

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            func="";
            func += formula.Text;
        }

        
        private double rectangle_integral(AngouriMath.Core.FastExpression f, double a, double b, int n) {
            
            double dx = 1.0 * (b - a) / n;
            double sum = 0.0;
            double xstart = a + dx;
            for (int i = 0; i < n; i++) {
                sum += f.Call(xstart + i * dx).Real;
            }
            integral = sum * dx;
            return (sum * dx);
        }

        private double simpson_integral(AngouriMath.Core.FastExpression f, double a, double b, int n)
        {
            double h = (b - a) / (2 * n);
            double[] x = new double[2 * n + 1];
            x[0] = a;
            for (int i = 1; i <= 2 * n; i++) {
                x[i] = x[i - 1] + h;
            }
            integral = f.Call(a).Real + f.Call(b).Real;
            double interSum = 0;
            for (int i = 1; i <= n; i++) {
                interSum += f.Call(x[2 * i - 1]).Real;
            }
            integral += 4 * interSum;
            interSum = 0;
            for (int i = 1; i <= n; i++)
            {
                interSum += f.Call(x[2 * i]).Real;
            }
            integral += 2 * interSum;
            integral *= (h / 3);
            integral *= 1.0;
            return integral;
        }
        private void OKButton_Click(object sender, EventArgs e)
        {
            method = "rectangle method";
            if (protection()) {
                var compiled = func.Compile("x");
                answerTextBox.Text = rectangle_integral(compiled, int.Parse(textBoxA.Text), int.Parse(textBoxB.Text), 100) + "";
            }
        }

        private void simpsonButton_Click(object sender, EventArgs e)
        {
            method = "Simpson method";
            if (protection()) {
                var compiled = func.Compile("x");
                answerTextBox.Text = simpson_integral(compiled, int.Parse(textBoxA.Text), int.Parse(textBoxB.Text), 100) + "";
            }

        }

        private void textBoxA_TextChanged(object sender, EventArgs e)
        {
            string sa = textBoxA.Text;
            for (int i = 0; i < sa.Length; i++) {
                if ((sa[i] < '0' || sa[i] > '9') && sa[i] != '-')
                {
                    textBoxA.Clear();
                    return;
                }
            }
            if (sa != "" && sa != "-") { 
                a = int.Parse(textBoxA.Text);
            }
        }

        private void textBoxB_TextChanged(object sender, EventArgs e)
        {
            string sb = textBoxB.Text;
            for (int i = 0; i < sb.Length; i++)
            {
                if ((sb[i] < '0' || sb[i] > '9') && sb[i] != '-')
                {
                    textBoxB.Clear();
                    return;
                }
            }
            if (sb != "" && sb != "-")
            {
                b = int.Parse(textBoxB.Text);
            }
        }
        private bool protection() {
            if (textBoxA.Text == "" || textBoxB.Text == "") {
                MessageBox.Show("Please, enter the interval");
                return false;
            }
            if (a >= b) {
                MessageBox.Show("a should be less then b");
                return false;
            }
            if (formula.Text == "") {
                MessageBox.Show("Please, enter the function");
                return false;
            }
            try
            {
                func.Compile("x");
            }
            catch (Exception) {
                MessageBox.Show("Error function. Please enter again");
                return false;
            }

            return true;
        }

        private void openFile_Click(object sender, EventArgs e) {

            openFileDialog1.Filter = "TXT(*.TXT)|*.txt";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string s = openFileDialog1.FileName;
                StreamReader reader = new StreamReader(s);
                func = reader.ReadToEnd();
                try
                {
                    var compiled = func.Compile("x");
                }
                catch (Exception) {
                    MessageBox.Show("Error function.\n Please, edit file or choose another one");
                    return;
                }
                formula.Text = func;
                saveFileDialog1.FileName = Path.ChangeExtension(s, "png");
                openFileDialog1.FileName = "";
            }
        }

        private void saveFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void saveFile_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "TXT(*.TXT)|*.txt";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string s = saveFileDialog1.FileName;
                StreamWriter writer = new StreamWriter(s, true);
                string result = "Integral of " + func + " on the interval from " + a + " to " + b + " = " + integral + " (" + method + ")\n";
                writer.WriteLine(result);
                writer.Close();
                MessageBox.Show("Results were recorded to the file");
            }
            
        }

        private void helpFile_Click(object sender, EventArgs e) {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"help.txt");

            StreamReader reader = new StreamReader(path);
            string helpInf = reader.ReadToEnd();
            reader.Close();
            MessageBox.Show(helpInf);
        }

        private void chart_Click(object sender, EventArgs e)
        {
            if (protection()) {
                var compiled = func.Compile("x");
                Chart chart = new Chart(this, compiled, a, b);
                chart.Show();
            }
        }
    }
}
