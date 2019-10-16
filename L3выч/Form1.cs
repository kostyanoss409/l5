using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ZedGraph;
namespace L3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            //
            InitializeComponent();
            ChartSetup();  
        }
        PointPairList FunctionList1 = new PointPairList();
        PointPairList FunctionList2 = new PointPairList();
        LineItem curve1;
        GraphPane pane;
        private void ChartSetup()
        {
            GraphPane pane = zedGraphControl1.GraphPane;
            pane.XAxis.Scale.MinAuto = true;
            pane.XAxis.Scale.MaxAuto = true;
            pane.YAxis.Scale.MinAuto = true;
            pane.YAxis.Scale.MaxAuto = true;
            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
            pane.XAxis.Title.Text = "Ось X";
            pane.YAxis.Title.Text = "Ось Y";
            pane.Title.Text = "Графики функций";
            pane.XAxis.MajorGrid.IsVisible = true;
            pane.YAxis.MajorGrid.IsVisible = false;
            pane.XAxis.MajorGrid.IsZeroLine = false;
            pane.YAxis.MajorGrid.IsZeroLine = false;
            pane.XAxis.Scale.FontSpec.Size = 12;
            pane.YAxis.Scale.FontSpec.Size = 12;
        }
        //Исходные функции
        public static double F2(double x, double y)
        { return ((x * x) / 4 + (y * y) / 9 - 1); }
        public static double F1(double x, double y)
        { return (2 * x + 3 * y - 1); }
        //Вычисление Якобиана
        public static double Det(double[,] W)
        {
            double a = W[0, 0] * W[1, 1] - W[0, 1] *  W[1, 0] ;
            return a;
        }
        //Исходные функции в явном виде
        double Fu1(double x) { return (Math.Sqrt(9 * (1 - (x * x) / 4))); }
        double Fu2(double x) { return (1 - 2 * x) / 3; }
        //Функция обработки события нажатия на кнопку "Решить систему и постоить график"
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //Проверка введенных значений на корректность
                if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "")
                { throw new Exception("Три слона зиждят Землю"); }
                double[] X = new double[2];
                double x0 = Convert.ToDouble(textBox1.Text);
                double y0 = Convert.ToDouble(textBox2.Text);
                double accuracy = Convert.ToDouble(textBox3.Text);
                if (accuracy < 1E-105 || Math.Abs(accuracy) > Math.Pow(10, 10))
                { throw new Exception("Такую точность не обеспечит и ворошиловский стрелок"); }
                double[,] J = new double[2, 2]; //Матрица Якоби 
                double[,] A = new double[2, 2]; //Матрица алгебраических дополнений
                double[] B = new double[2]; //Погрешность
                double[] F = new double[2]; //Столбец значений функций
                //Начальное приближение
                X[0] = x0;
                X[1] = y0;
                int step = 0;    //Итерационный шаг                
                double j;   //Определитель Якоби
                do
                {
                    //Матрица Якоби
                    J[1, 0] = 2 * X[0] / 4;
                    J[1, 1] = 2 * X[1] / 9;
                    J[0, 0] = 2;
                    J[0, 1] = 3;
                    j = Det(J); //Вычисление Якобиана
                    if (j == 0) //Проверка на ненулевость
                    { throw new Exception("Здесь нулевой Якобиан. Войди в другую дверь."); }
                    //Матрица алгебраических дополнений
                    A[0, 0] = 2 * X[1] / 9;
                    A[0, 1] = -3;
                    A[1, 0] = -2 * X[0] / 4;
                    A[1, 1] = 2;
                    //Цикл строки матрицы
                    for (int i = 0; i < 2; i++)
                    {
                        //Цикл столбцы матрицы
                        for (int k = 0; k < 2; k++)
                        { A[i, k] = A[i, k] / j; }
                    }
                    //Столбец значений функций
                    F[1] = (Math.Pow(X[0], 2)) / 4 + (Math.Pow(X[1], 2)) / 9 - 1;
                    F[0] = 2 * X[0] + 3 * X[1] - 1;
                    //Погрешность
                    for (int h = 0; h < 2; h++)
                    {
                        B[h] = 0.0;
                        for (int b = 0; b < 2; b++)
                        { B[h] += A[h, b] * F[b]; }
                    }
                    //Приближение
                    for (int d = 0; d < 2; d++)
                    { X[d] = X[d] - B[d]; }
                    step++;
                    if (step > 150) //Ограничение на итерации
                    { throw new Exception("Итерации ушли туда, где пахнет нефтью"); }
                }
                //Прекращаем действия, когда будет достигнута желаемая точность
                while ((Math.Abs(F1(X[0], X[1])) > accuracy) || (Math.Abs(F2(X[0], X[1])) > accuracy));
                //Выводим найденное решение на экран
                textBox5.Text = X[0].ToString();
                textBox4.Text = X[1].ToString();
                //Вычисление значений функций при найденных значениях переменных и их вывод на экран
                textBox7.Text = ((X[0] * X[0]) / 4 + (X[1] * X[1]) / 9).ToString();
                textBox6.Text = (2 * X[0] + 3 * X[1]).ToString();
                //Очищаем массивы точек
                FunctionList1.Clear();
                FunctionList2.Clear();
                //Настраиваем график
                pane = zedGraphControl1.GraphPane;
                pane.CurveList.Clear();
                zedGraphControl1.AxisChange();
                zedGraphControl1.Invalidate();
                //Настройка панели для построения графика
                pane = zedGraphControl1.GraphPane;
                zedGraphControl1.AutoSize = true;
                //Установка масштаба 
                pane.XAxis.Scale.Min = -4;
                pane.XAxis.Scale.Max = 4;
                pane.XAxis.Scale.Format = "F2";
                pane.XAxis.Scale.FontSpec.Size = 12;
                pane.YAxis.Scale.FontSpec.Size = 12;
                //Очищение списка кривых
                pane.CurveList.Clear();
                pane.Title.Text = "Графики функций";
                pane.XAxis.Title.Text = pane.XAxis.Title.Text = "Ось X";
                pane.YAxis.Title.Text = pane.YAxis.Title.Text = "Ось Y";
                double x;
                for (x = -2; x < 2; x += 0.001)
                { FunctionList1.Add(x, Fu1(x)); }
                for (x = -2.001; x < 2; x += 0.001)
                { FunctionList1.Add(x, -Fu1(x)); }
                for (x = -4; x < 4; x += 0.001)
                { FunctionList2.Add(x, Fu2(x)); }
                Draw1(FunctionList1);
                Draw2(FunctionList2);
            }
            catch (ArgumentOutOfRangeException) { MessageBox.Show("Император Рима Римом ограничен."); }
            catch (Exception ea) { MessageBox.Show(ea.Message); }
        }
        //Построение графика первой функции
        private void Draw1(PointPairList list)
        {
            try
            {
                //Очищение панели
                pane.CurveList.Clear();
                //Построение графика функции
                curve1 = pane.AddCurve("функция 1", list, Color.Green, SymbolType.None);                
                curve1.Line.Width = 2;  //ширина линии
                //Обновление данных
                zedGraphControl1.AxisChange();
                zedGraphControl1.Invalidate();
            }
            //Обработка исключений
            catch (Exception m) { MessageBox.Show(m.Message); }
        }
        //Построение графика второй функции
        private void Draw2(PointPairList list)
        {
            //Сортировка массива точек
            list.TrimExcess();
            list.Sort();
            try
            {
                //Построение графика функции
                curve1 = pane.AddCurve("функция 2", list, Color.Violet, SymbolType.None);                
                curve1.Line.Width = 2;  //ширина линии
                //Обновление данных
                zedGraphControl1.AxisChange();
                zedGraphControl1.Invalidate();
            }
            //Обработка исключений
            catch (Exception m) { MessageBox.Show(m.Message); }
        }
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!((e.KeyChar >= '0') && (e.KeyChar <= '9') || (e.KeyChar == (char)8) || (e.KeyChar == '-') || (e.KeyChar == '.')))
            { e.Handled = true; }
            if ((e.KeyChar == '.'))
            {
                if ((textBox1.Text.IndexOf('.') != (-1)))
                    e.Handled = true;
            }
            if (textBox1.Text.Length == 0)
            {
                if (e.KeyChar == '.')
                    e.Handled = true;
            }
        }
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!((e.KeyChar >= '0') && (e.KeyChar <= '9') || (e.KeyChar == (char)8) || (e.KeyChar == '-') || (e.KeyChar == '.')))
            { e.Handled = true; }
            if ((e.KeyChar == '.'))
            {
                if ((textBox2.Text.IndexOf('.') != (-1)))
                    e.Handled = true;
            }
            if (textBox2.Text.Length == 0)
            {
                if (e.KeyChar == '.')
                    e.Handled = true;
            }
        }
        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!((e.KeyChar >= '0') && (e.KeyChar <= '9') || (e.KeyChar == (char)8) || (e.KeyChar == '.')))
            { e.Handled = true; }
            if ((e.KeyChar == '.'))
            {
                if ((textBox3.Text.IndexOf('.') != (-1)))
                    e.Handled = true;
            }
            if (textBox3.Text.Length == 0)
            {
                if (e.KeyChar == '.')
                    e.Handled = true;
            }
        }      
    }
}