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
namespace L6
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ChartSetup();  
        }
        //Списки точек для построения графиков
        PointPairList fp1 = new PointPairList();
        PointPairList fp2 = new PointPairList();
        PointPairList fp3 = new PointPairList();
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
        double dy1(double y1, double y2, double y3)
        { return 3 * y1 + y2 + y3; }
        double dy2(double y1, double y2, double y3)
        { return - y2 + 3 * y3; }
        double dy3(double y1, double y2, double y3)
        { return y1 + 2 * y2 + y3; }
        //Собственные числа матрицы A
        double a1 = -1.88, a2 = 4.1, a3 = 0.77564;
        //Матрицы М1, М2, М3
        double[,] M2 = new double[3, 3] { { 1.69257, 0.57951, 0.82485 }, { 0.24535, 0.43769 , 0.75715 }, { 0.57951, 0.58655, 1.025 } };
        double[,] M3 = new double[3, 3] { { 0.08714, -0.39334 , 0.11509 }, { -0.16948, 0.76505, -0.16356 }, { 0.11193, -0.50527, 0.14784 } };
        double[,] M1 = new double[3, 3] { { -0.11842, 0.30452, 0.55324 }, { -0.08291, 0.21321, 0.38742 }, { -0.19371, 0.49823, 0.90515 } };
        //Решение методом Лагранжа-Сильвестра
        double Fu1(double x, double y10, double y20, double y30)
        {
            return (M1[0, 0] * y10 + M1[0, 1] * y20 + M1[0, 2] * y30) * Math.Pow(Math.E, (a1 * x)) +
                   (M2[0, 0] * y10 + M2[0, 1] * y20 + M2[0, 2] * y30) * Math.Pow(Math.E, (a2 * x)) +
                   (M3[0, 0] * y10 + M3[0, 1] * y20 + M3[0, 2] * y30) * Math.Pow(Math.E, (a3 * x));
        }
        double Fu2(double x, double y10, double y20, double y30)
        {
            return (M1[1, 0] * y10 + M1[1, 1] * y20 + M1[1, 2] * y30) * Math.Pow(Math.E, (a1 * x)) +
                   (M2[1, 0] * y10 + M2[1, 1] * y20 + M2[1, 2] * y30) * Math.Pow(Math.E, (a2 * x)) +
                   (M3[1, 0] * y10 + M3[1, 1] * y20 + M3[1, 2] * y30) * Math.Pow(Math.E, (a3 * x));
        }
        double Fu3(double x, double y10, double y20, double y30)
        {
            return (M1[2, 0] * y10 + M1[2, 1] * y20 + M1[2, 2] * y30) * Math.Pow(Math.E, (a1 * x)) +
                   (M2[2, 0] * y10 + M2[2, 1] * y20 + M2[2, 2] * y30) * Math.Pow(Math.E, (a2 * x)) +
                   (M3[2, 0] * y10 + M3[2, 1] * y20 + M3[2, 2] * y30) * Math.Pow(Math.E, (a3 * x));
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {

                //Проверка введенных значений на корректность
                if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "" || textBox5.Text == "")
                { throw new Exception("И сколько ни кричи - пустота в пустоту ни о чём."); }
                double y10 = Convert.ToDouble(textBox1.Text);
                double y20 = Convert.ToDouble(textBox2.Text);
                double y30 = Convert.ToDouble(textBox3.Text);
                double leftx = 0;
                double rightx = Convert.ToDouble(textBox5.Text);
                if (leftx > rightx || leftx == rightx) throw new Exception("Я требую не адвоката, но нечто большее чем ноль.");
                if ((rightx > 170)) { throw new Exception("В правильном месте правильная лужа способна вознести тебя либо уничтожить."); }
                if ((y10 == 0) && (y20 == 0) && (y30 == 0)) { throw new Exception("Эта дверь ведет в пустую комнату."); }
                double step = (rightx - leftx) / 100;   //Задается шаг
                //Очистка списков точек
                fp1.Clear();
                fp2.Clear();
                fp3.Clear();
                double y11 = 0.0, y21 = 0.0, y31 = 0.0;
                int i = 0;
                for (double x = leftx; x <= rightx; x += step)
                {
                    //Вычисление значений функций
                    y11 = Fu1(x, y10, y20, y30);
                    y21 = Fu2(x, y10, y20, y30);
                    y31 = Fu3(x, y10, y20, y30);
                    //Добавление точек в списки
                    fp1.Add(x, y11);
                    fp2.Add(x, y21);
                    fp3.Add(x, y31);
                    i++;
                }
                //Настройка графика
                pane = zedGraphControl1.GraphPane;
                pane.CurveList.Clear();
                zedGraphControl1.AxisChange();
                zedGraphControl1.Invalidate();
                //Настройка панели для построения графика
                pane = zedGraphControl1.GraphPane;
                zedGraphControl1.AutoSize = false;
                pane.XAxis.Scale.MaxAuto = true;
                pane.XAxis.Scale.MinAuto = true;
                pane.YAxis.Scale.MaxAuto = true;
                pane.YAxis.Scale.MinAuto = true;
                //Установка масштаба 
                pane.XAxis.Scale.Min = leftx;
                pane.XAxis.Scale.Max = rightx;
                pane.XAxis.Scale.Format = "F2";
                pane.XAxis.Scale.FontSpec.Size = 12;
                pane.YAxis.Scale.FontSpec.Size = 12;
                pane.CurveList.Clear(); //Очистка списка кривых
                pane.Title.Text = "Графики функций";
                pane.XAxis.Title.Text = pane.XAxis.Title.Text = "Ось X";
                pane.YAxis.Title.Text = pane.YAxis.Title.Text = "Ось Y";
                //Построение графиков функций
                Draw1(fp1);
                Draw2(fp2);
                Draw3(fp3);
            }
            catch (FormatException) { MessageBox.Show("Наверное, это эльфийский"); }
            catch (ArgumentOutOfRangeException) { MessageBox.Show("Император Рима Римом ограничен."); }
            catch (OverflowException) { MessageBox.Show("Император Рима Римом ограничен."); }
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
        //Построение графика второй функции
        private void Draw3(PointPairList list)
        {
            //Сортировка массива точек
            list.TrimExcess();
            list.Sort();
            try
            {
                //Построение графика функции
                curve1 = pane.AddCurve("функция 3", list, Color.DeepPink, SymbolType.None);
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
            { if ((textBox1.Text.IndexOf('.') != (-1)))
              e.Handled = true; }
            if (textBox1.Text.Length == 0)
            { if (e.KeyChar == '.')
              e.Handled = true; }
        }
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!((e.KeyChar >= '0') && (e.KeyChar <= '9') || (e.KeyChar == (char)8) || (e.KeyChar == '-') || (e.KeyChar == '.')))
            { e.Handled = true; }
            if ((e.KeyChar == '.'))
            { if ((textBox2.Text.IndexOf('.') != (-1)))
              e.Handled = true; }
            if (textBox2.Text.Length == 0)
            { if (e.KeyChar == '.')
              e.Handled = true; }
        }
        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!((e.KeyChar >= '0') && (e.KeyChar <= '9') || (e.KeyChar == (char)8) || (e.KeyChar == '-') || (e.KeyChar == '.')))
            { e.Handled = true; }
            if ((e.KeyChar == '.'))
            { if ((textBox3.Text.IndexOf('.') != (-1)))
              e.Handled = true; }
            if (textBox3.Text.Length == 0)
            { if (e.KeyChar == '.')
              e.Handled = true; }
        }
        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!((e.KeyChar >= '0') && (e.KeyChar <= '9') || (e.KeyChar == (char)8) || (e.KeyChar == '.')))
            { e.Handled = true; }
            if ((e.KeyChar == '.'))
            { if ((textBox5.Text.IndexOf('.') != (-1)))
              e.Handled = true; }
            if (textBox5.Text.Length == 0)
            { if (e.KeyChar == '.')
              e.Handled = true; }
        }
    }
}