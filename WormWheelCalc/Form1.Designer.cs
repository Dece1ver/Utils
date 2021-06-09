
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace WormWheelCalc
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(Form1));
            this.groupBox1 = new GroupBox();
            this.label18 = new Label();
            this.label17 = new Label();
            this.label6 = new Label();
            this.secTB = new TextBox();
            this.minTB = new TextBox();
            this.degTB = new TextBox();
            this.simpleCalcCB = new CheckBox();
            this.roundCB = new CheckBox();
            this.z1TB = new TextBox();
            this.label16 = new Label();
            this.button1 = new Button();
            this.qTB = new TextBox();
            this.label3 = new Label();
            this.z2TB = new TextBox();
            this.label4 = new Label();
            this.mTB = new TextBox();
            this.label2 = new Label();
            this.awTB = new TextBox();
            this.label1 = new Label();
            this.groupBox2 = new GroupBox();
            this.ha2TB = new TextBox();
            this.label15 = new Label();
            this.sna2TB = new TextBox();
            this.label13 = new Label();
            this.angleTB = new TextBox();
            this.label14 = new Label();
            this.dw1TB = new TextBox();
            this.label9 = new Label();
            this.st2TB = new TextBox();
            this.label10 = new Label();
            this.d2TB = new TextBox();
            this.label11 = new Label();
            this.da2TB = new TextBox();
            this.label12 = new Label();
            this.haTB = new TextBox();
            this.label5 = new Label();
            this.saw2TB = new TextBox();
            this.label7 = new Label();
            this.xTB = new TextBox();
            this.label8 = new Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label18);
            this.groupBox1.Controls.Add(this.label17);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.secTB);
            this.groupBox1.Controls.Add(this.minTB);
            this.groupBox1.Controls.Add(this.degTB);
            this.groupBox1.Controls.Add(this.simpleCalcCB);
            this.groupBox1.Controls.Add(this.roundCB);
            this.groupBox1.Controls.Add(this.z1TB);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.qTB);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.z2TB);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.mTB);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.awTB);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new Size(190, 426);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Ввод";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(204)));
            this.label18.Location = new Point(176, 286);
            this.label18.Name = "label18";
            this.label18.Size = new Size(13, 16);
            this.label18.TabIndex = 17;
            this.label18.Text = "″";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(204)));
            this.label17.Location = new Point(115, 286);
            this.label17.Name = "label17";
            this.label17.Size = new Size(12, 16);
            this.label17.TabIndex = 16;
            this.label17.Text = "′";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(204)));
            this.label6.Location = new Point(54, 286);
            this.label6.Name = "label6";
            this.label6.Size = new Size(12, 16);
            this.label6.TabIndex = 15;
            this.label6.Text = "⁰";
            // 
            // secTB
            // 
            this.secTB.Location = new Point(132, 283);
            this.secTB.Name = "secTB";
            this.secTB.Size = new Size(42, 20);
            this.secTB.TabIndex = 11;
            // 
            // minTB
            // 
            this.minTB.Location = new Point(71, 283);
            this.minTB.Name = "minTB";
            this.minTB.Size = new Size(42, 20);
            this.minTB.TabIndex = 10;
            // 
            // degTB
            // 
            this.degTB.Location = new Point(10, 283);
            this.degTB.Name = "degTB";
            this.degTB.Size = new Size(42, 20);
            this.degTB.TabIndex = 9;
            // 
            // simpleCalcCB
            // 
            this.simpleCalcCB.AutoSize = true;
            this.simpleCalcCB.Checked = true;
            this.simpleCalcCB.CheckState = CheckState.Checked;
            this.simpleCalcCB.Location = new Point(10, 260);
            this.simpleCalcCB.Name = "simpleCalcCB";
            this.simpleCalcCB.Size = new Size(130, 17);
            this.simpleCalcCB.TabIndex = 11;
            this.simpleCalcCB.TabStop = false;
            this.simpleCalcCB.Text = "Упрощенный расчет";
            this.simpleCalcCB.UseVisualStyleBackColor = true;
            this.simpleCalcCB.CheckedChanged += new EventHandler(this.simpleCalcCB_CheckedChanged);
            // 
            // roundCB
            // 
            this.roundCB.AutoSize = true;
            this.roundCB.Checked = true;
            this.roundCB.CheckState = CheckState.Checked;
            this.roundCB.Location = new Point(10, 237);
            this.roundCB.Name = "roundCB";
            this.roundCB.Size = new Size(147, 17);
            this.roundCB.TabIndex = 9;
            this.roundCB.TabStop = false;
            this.roundCB.Text = "Визуальное округление";
            this.roundCB.UseVisualStyleBackColor = true;
            this.roundCB.CheckedChanged += new EventHandler(this.button1_Click);
            // 
            // z1TB
            // 
            this.z1TB.Location = new Point(10, 157);
            this.z1TB.Name = "z1TB";
            this.z1TB.Size = new Size(174, 20);
            this.z1TB.TabIndex = 7;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new Point(7, 140);
            this.label16.Name = "label16";
            this.label16.Size = new Size(154, 13);
            this.label16.TabIndex = 9;
            this.label16.Text = "Количество заходов червяка";
            // 
            // button1
            // 
            this.button1.Location = new Point(6, 382);
            this.button1.Name = "button1";
            this.button1.Size = new Size(178, 38);
            this.button1.TabIndex = 12;
            this.button1.Text = "Расчет";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new EventHandler(this.button1_Click);
            // 
            // qTB
            // 
            this.qTB.Location = new Point(10, 197);
            this.qTB.Name = "qTB";
            this.qTB.Size = new Size(174, 20);
            this.qTB.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new Point(7, 180);
            this.label3.Name = "label3";
            this.label3.Size = new Size(173, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Коэффициент диаметра червяка";
            // 
            // z2TB
            // 
            this.z2TB.Location = new Point(10, 117);
            this.z2TB.Name = "z2TB";
            this.z2TB.Size = new Size(174, 20);
            this.z2TB.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new Point(7, 100);
            this.label4.Name = "label4";
            this.label4.Size = new Size(143, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Количество зубьев колеса";
            // 
            // mTB
            // 
            this.mTB.Location = new Point(10, 77);
            this.mTB.Name = "mTB";
            this.mTB.Size = new Size(174, 20);
            this.mTB.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new Point(7, 60);
            this.label2.Name = "label2";
            this.label2.Size = new Size(45, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Модуль";
            // 
            // awTB
            // 
            this.awTB.Location = new Point(10, 37);
            this.awTB.Name = "awTB";
            this.awTB.Size = new Size(174, 20);
            this.awTB.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new Point(7, 20);
            this.label1.Name = "label1";
            this.label1.Size = new Size(128, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Межосевое расстояние";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.ha2TB);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.sna2TB);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.angleTB);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.dw1TB);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.st2TB);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.d2TB);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.da2TB);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.haTB);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.saw2TB);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.xTB);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Location = new Point(209, 13);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new Size(270, 426);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Расчет";
            // 
            // ha2TB
            // 
            this.ha2TB.Location = new Point(9, 397);
            this.ha2TB.Name = "ha2TB";
            this.ha2TB.ReadOnly = true;
            this.ha2TB.Size = new Size(254, 20);
            this.ha2TB.TabIndex = 29;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(204)));
            this.label15.Location = new Point(6, 380);
            this.label15.Name = "label15";
            this.label15.Size = new Size(120, 13);
            this.label15.TabIndex = 28;
            this.label15.Text = "Высота до хорды зуба";
            // 
            // sna2TB
            // 
            this.sna2TB.Location = new Point(9, 357);
            this.sna2TB.Name = "sna2TB";
            this.sna2TB.ReadOnly = true;
            this.sna2TB.Size = new Size(254, 20);
            this.sna2TB.TabIndex = 27;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(204)));
            this.label13.Location = new Point(6, 340);
            this.label13.Name = "label13";
            this.label13.Size = new Size(257, 13);
            this.label13.TabIndex = 26;
            this.label13.Text = "Толщина зуба по хорде делительной окружности";
            // 
            // angleTB
            // 
            this.angleTB.Location = new Point(9, 317);
            this.angleTB.Name = "angleTB";
            this.angleTB.ReadOnly = true;
            this.angleTB.Size = new Size(254, 20);
            this.angleTB.TabIndex = 25;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new Point(6, 300);
            this.label14.Name = "label14";
            this.label14.Size = new Size(137, 13);
            this.label14.TabIndex = 24;
            this.label14.Text = "Начальный угол подъема";
            // 
            // dw1TB
            // 
            this.dw1TB.Location = new Point(9, 277);
            this.dw1TB.Name = "dw1TB";
            this.dw1TB.ReadOnly = true;
            this.dw1TB.Size = new Size(254, 20);
            this.dw1TB.TabIndex = 23;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new Point(6, 260);
            this.label9.Name = "label9";
            this.label9.Size = new Size(154, 13);
            this.label9.TabIndex = 22;
            this.label9.Text = "Начальный диаметр червяка";
            // 
            // st2TB
            // 
            this.st2TB.Location = new Point(9, 237);
            this.st2TB.Name = "st2TB";
            this.st2TB.ReadOnly = true;
            this.st2TB.Size = new Size(254, 20);
            this.st2TB.TabIndex = 21;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new Point(6, 220);
            this.label10.Name = "label10";
            this.label10.Size = new Size(257, 13);
            this.label10.TabIndex = 20;
            this.label10.Text = "Толщина зуба по хорде делительной окружности";
            // 
            // d2TB
            // 
            this.d2TB.Location = new Point(9, 197);
            this.d2TB.Name = "d2TB";
            this.d2TB.ReadOnly = true;
            this.d2TB.Size = new Size(254, 20);
            this.d2TB.TabIndex = 19;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new Point(6, 180);
            this.label11.Name = "label11";
            this.label11.Size = new Size(123, 13);
            this.label11.TabIndex = 18;
            this.label11.Text = "Делительный диаметр";
            // 
            // da2TB
            // 
            this.da2TB.Location = new Point(9, 157);
            this.da2TB.Name = "da2TB";
            this.da2TB.ReadOnly = true;
            this.da2TB.Size = new Size(254, 20);
            this.da2TB.TabIndex = 17;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new Point(6, 140);
            this.label12.Name = "label12";
            this.label12.Size = new Size(132, 13);
            this.label12.TabIndex = 16;
            this.label12.Text = "Диаметр вершин зубьев";
            // 
            // haTB
            // 
            this.haTB.Location = new Point(9, 117);
            this.haTB.Name = "haTB";
            this.haTB.ReadOnly = true;
            this.haTB.Size = new Size(254, 20);
            this.haTB.TabIndex = 15;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new Point(6, 100);
            this.label5.Name = "label5";
            this.label5.Size = new Size(175, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Половина угловой толщины зуба";
            // 
            // saw2TB
            // 
            this.saw2TB.Location = new Point(9, 77);
            this.saw2TB.Name = "saw2TB";
            this.saw2TB.ReadOnly = true;
            this.saw2TB.Size = new Size(254, 20);
            this.saw2TB.TabIndex = 14;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new Point(6, 60);
            this.label7.Name = "label7";
            this.label7.Size = new Size(250, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Толщина зуба по дуге делительной окружности";
            // 
            // xTB
            // 
            this.xTB.Location = new Point(9, 37);
            this.xTB.Name = "xTB";
            this.xTB.ReadOnly = true;
            this.xTB.Size = new Size(254, 20);
            this.xTB.TabIndex = 13;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new Point(6, 20);
            this.label8.Name = "label8";
            this.label8.Size = new Size(133, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "Коэффициент смещения";
            // 
            // Form1
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(489, 449);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.Icon = ((Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Расчет зуба";
            this.Load += new EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private GroupBox groupBox1;
        private TextBox qTB;
        private Label label3;
        private TextBox z2TB;
        private Label label4;
        private TextBox mTB;
        private Label label2;
        private TextBox awTB;
        private Label label1;
        private GroupBox groupBox2;
        private TextBox ha2TB;
        private Label label15;
        private TextBox sna2TB;
        private Label label13;
        private TextBox angleTB;
        private Label label14;
        private TextBox dw1TB;
        private Label label9;
        private TextBox st2TB;
        private Label label10;
        private TextBox d2TB;
        private Label label11;
        private TextBox da2TB;
        private Label label12;
        private TextBox haTB;
        private Label label5;
        private TextBox saw2TB;
        private Label label7;
        private TextBox xTB;
        private Label label8;
        private Button button1;
        private TextBox z1TB;
        private Label label16;
        private CheckBox roundCB;
        private CheckBox simpleCalcCB;
        private Label label18;
        private Label label17;
        private Label label6;
        private TextBox secTB;
        private TextBox minTB;
        private TextBox degTB;
    }
}

