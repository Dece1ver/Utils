using System;
using System.Windows.Forms;
using WormWheelCalc.Properties;

namespace WormWheelCalc
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private double ToRad(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        private double ToDeg(double radians)
        {
            return radians * 180 / Math.PI;
        }

        private void SetBoxes()
        {
            if (simpleCalcCB.Checked)
            {
                awTB.Enabled = false;
                label4.Text = "Диаметр вершин колеса";
                label16.Text = "Делительный диаметр колеса";
                label3.Text = "Коэффициент смещения червяка";
                label6.Visible = true;
                label17.Visible = true;
                label18.Visible = true;
                degTB.Visible = true;
                minTB.Visible = true;
                secTB.Visible = true;

            }
            else
            {
                awTB.Enabled = true;
                label4.Text = "Количество зубьев колеса";
                label16.Text = "Количество заходов червяка";
                label3.Text = "Коэффициент диаметра червяка";
                label6.Visible = false;
                label17.Visible = false;
                label18.Visible = false;
                degTB.Visible = false;
                minTB.Visible = false;
                secTB.Visible = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string fString;
                double h = 1;
                if (roundCB.Checked)
                {
                    fString = "N3";
                }
                else
                {
                    fString = "";
                }

                if (simpleCalcCB.Checked)
                {

                    double m = Double.Parse(mTB.Text.Replace('.', ','));
                    double da2 = Double.Parse(z2TB.Text.Replace('.', ','));
                    double d2 = Double.Parse(z1TB.Text.Replace('.', ','));
                    double x = Double.Parse(qTB.Text.Replace('.', ','));
                    double deg = Double.Parse(degTB.Text.Replace('.', ','));
                    double min = Double.Parse(minTB.Text.Replace('.', ','));
                    double sec = Double.Parse(secTB.Text.Replace('.', ','));

                    xTB.Text = "";

                    double saw2 = m * (Math.PI / 2 + 2 * x * Math.Tan(ToRad(20)));
                    saw2TB.Text = saw2.ToString(fString);

                    d2TB.Text = d2.ToString(fString);

                    da2TB.Text = da2.ToString(fString);

                    double ha = saw2 / d2;
                    haTB.Text = ToDeg(ha).ToString(fString);

                    double st2 = d2 * Math.Sin(ha);
                    st2TB.Text = st2.ToString(fString);

                    double angle = 0;
                    if ((min >= 0 && min <= 60) && (sec >= 0 && sec <= 60))
                    {
                        angle = deg + (min / 60) + (sec / 3600);
                    }
                    angleTB.Text = angle.ToString(fString);

                    double sna2 = st2 * Math.Cos(ToRad(angle));
                    sna2TB.Text = sna2.ToString(fString);

                    double ha2 = ((da2 - d2) / 2) + (Math.Pow(saw2, 2) / (4 * d2));
                    ha2TB.Text = ha2.ToString(fString);
                }
                else
                {

                    double aw = Double.Parse(awTB.Text.Replace('.', ','));
                    double m = Double.Parse(mTB.Text.Replace('.', ','));
                    double z2 = Double.Parse(z2TB.Text.Replace('.', ','));
                    double z1 = Double.Parse(z1TB.Text.Replace('.', ','));
                    double q = Double.Parse(qTB.Text.Replace('.', ','));


                    double x = aw / m - 0.5 * (z2 + q);
                    xTB.Text = x.ToString(fString);

                    double saw2 = m * (Math.PI / 2 + 2 * x * Math.Tan(ToRad(20)));
                    saw2TB.Text = saw2.ToString(fString);

                    double d2 = z2 * m;
                    d2TB.Text = d2.ToString(fString);

                    double da2 = d2 + 2 * (h + x) * m;
                    da2TB.Text = da2.ToString(fString);

                    double ha = saw2 / d2;
                    haTB.Text = ToDeg(ha).ToString(fString);

                    double st2 = d2 * Math.Sin(ha);
                    st2TB.Text = st2.ToString(fString);

                    double dw1 = (q + 2 * x) * m;
                    dw1TB.Text = dw1.ToString(fString);

                    double angle = Math.Atan(z1 * m / dw1);
                    angleTB.Text = ToDeg(angle).ToString(fString);

                    double sna2 = st2 * Math.Cos(angle);
                    sna2TB.Text = sna2.ToString(fString);

                    double ha2 = ((da2 - d2) / 2) + (Math.Pow(saw2, 2) / (4 * d2));
                    ha2TB.Text = ha2.ToString(fString);
                }
            }
            catch
            {

            }
        }

        private void simpleCalcCB_CheckedChanged(object sender, EventArgs e)
        {
            SetBoxes();
            Settings.Default.simpleCalc = simpleCalcCB.Checked;
            Settings.Default.Save();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetBoxes();
        }
    }
}
