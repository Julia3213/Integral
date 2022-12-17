using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using AngouriMath.Extensions;

namespace Integral
{
    public partial class Form1 : Form
    {
        double a, b;
        public Form1()
        {
            InitializeComponent();
        }
        string func;
        string method;
        double integral;
        int acc = 1000;

        //ввод функции
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            func="";
            func += formula.Text;
        }

        //функция вычисления функции методом прямоугольников
        private double rectangle_integral(AngouriMath.Core.FastExpression f, double a, double b, int n) {
            for (int i = (int)a; i < (int)b; i += 1) {
                if (double.IsNaN(f.Call(i).Real)|| double.IsInfinity(f.Call(i).Real))
                {
                    return double.NaN;
                }
            }
            double dx = 1.0 * (b - a) / n;
            double sum = 0.0;
            double xstart = a + dx;
            for (int i = 0; i < n; i++) {
                sum += f.Call(a + dx/2 + i * dx).Real;
            }
            integral = sum * dx;
            if (Math.Abs(integral) < 0.000000001)
                integral = 0;
            return integral;
        }

        //функция вычисления интеграла методом Симпсона(метод парабол)
        private double simpson_integral(AngouriMath.Core.FastExpression f, double a, double b, int n)
        {
            for (int i = (int)a; i < (int)b; i += 1)
            {
                if (double.IsNaN(f.Call(i).Real) || double.IsInfinity(f.Call(i).Real))
                {
                    return double.NaN;
                }
            }
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
            for (int i = 1; i < n; i++)
            {
                interSum += f.Call(x[2 * i]).Real;
            }
            integral += 2 * interSum;
            integral *= (h / 3);

            if (Math.Abs(integral) < 0.000000001)
                integral = 0;
            return integral;
        }

        //обработчик нажатия на кнопку "Метод прямоугольников"
        private void OKButton_Click(object sender, EventArgs e)
        {
            method = "rectangle method";
            if (protection()) {
                var compiled = func.Compile("x");

                if (double.IsInfinity((double)acc))
                {
                    MessageBox.Show("Accuracy could not be 0");
                }
                acc = (acc == 0) ? 1000 : acc;
                double answer = rectangle_integral(compiled, a, b, acc);
                if(double.IsNaN(answer))
                    answerTextBox.Text = "интеграл не определен";
                else
                    answerTextBox.Text = answer + "";
            }
        }

        //обработчик нажатия на кнопку "Метод Симпсона"
        private void simpsonButton_Click(object sender, EventArgs e)
        {
            method = "Simpson method";
            if (protection()) {
                var compiled = func.Compile("x");

                if (double.IsInfinity((double)acc))
                {
                    MessageBox.Show("Accuracy could not be 0");
                }
                acc = (acc == 0) ? 1000 : acc;
                double answer = simpson_integral(compiled, a, b, acc);
                if (double.IsNaN(answer))
                    answerTextBox.Text = "интеграл не определен";
                else
                    answerTextBox.Text = answer + "";
            }

        }

        //ввод значения а
        private void textBoxA_TextChanged(object sender, EventArgs e)
        {
            string sa = textBoxA.Text;
            for (int i = 0; i < sa.Length; i++) {
                if ((sa[i] < '0' || sa[i] > '9') && sa[i] != '-' && sa[i]!=',')
                {
                    textBoxA.Clear();
                    return;
                }
            }
            if (sa != "" && sa != "-" && sa[sa.Length-1]!=',') { 
                a = double.Parse(textBoxA.Text);
            }
        }

        //ввод значения b
        private void textBoxB_TextChanged(object sender, EventArgs e)
        {
            string sb = textBoxB.Text;
            for (int i = 0; i < sb.Length; i++)
            {
                if ((sb[i] < '0' || sb[i] > '9') && sb[i] != '-' && sb[i]!=',')
                {
                    textBoxB.Clear();
                    return;
                }
            }
            if (sb != "" && sb != "-"&&sb[sb.Length-1]!=',')
            {
                b = double.Parse(textBoxB.Text);
            }
        }

        //защита от неквалифицированных действий пользователя
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

        //Открытие файла для считывания функции
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


        //Сохранение файла
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

        //обработчик нажатия на кнопку "Помощь"
        private void helpFile_Click(object sender, EventArgs e) {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"help.txt");

            StreamReader reader = new StreamReader(path);
            string helpInf = reader.ReadToEnd();
            reader.Close();
            MessageBox.Show(helpInf);
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {
            string sb = textBox1.Text;
            for (int i = 0; i < sb.Length; i++)
            {
                if ((sb[i] < '0' || sb[i] > '9') && sb[i] != '-' && sb[i] != ',')
                {
                    textBox1.Clear();
                    return;
                }
            }
            if (sb != "" && sb != "-" && sb[sb.Length - 1] != ',')
            {
                double fraction = double.Parse(textBox1.Text);
                if (fraction < 0.00001 && fraction!=0) { 
                    fraction = 0.00001;
                    textBox1.Text="0,00001";
                    MessageBox.Show("The maximum accuracy is 0.00001.\nThis is enough to calculate the integral with an accuracy of more than 10 decimal places.");
                }
                if (fraction > 1) {
                    fraction = 1;
                    textBox1.Text = "1";
                    MessageBox.Show("The minimum accuracy is 1");
                }
                acc = (int)((b-a)/fraction);
            }
        }

        //обработчик нажатия на кнопку "Построить график"
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