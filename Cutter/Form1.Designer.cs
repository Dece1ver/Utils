using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Cutter
{
    partial class MainWindow
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
            this.textBox1 = new TextBox();
            this.textBox2 = new TextBox();
            this.label2 = new Label();
            this.groupBox1 = new GroupBox();
            this.button2 = new Button();
            this.label3 = new Label();
            this.numericUpDown1 = new NumericUpDown();
            this.textBox3 = new TextBox();
            this.groupBox2 = new GroupBox();
            this.button3 = new Button();
            this.label4 = new Label();
            this.numericUpDown2 = new NumericUpDown();
            this.textBox4 = new TextBox();
            this.button4 = new Button();
            this.label5 = new Label();
            this.label1 = new Label();
            this.label6 = new Label();
            this.openFileDialog1 = new OpenFileDialog();
            this.button1 = new Button();
            this.label7 = new Label();
            this.textBox5 = new TextBox();
            this.button5 = new Button();
            this.groupBox1.SuspendLayout();
            ((ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((ISupportInitialize)(this.numericUpDown2)).BeginInit();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.BackColor = SystemColors.Window;
            this.textBox1.Font = new Font("Consolas", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            this.textBox1.Location = new Point(15, 511);
            this.textBox1.Margin = new Padding(4, 3, 4, 3);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = ScrollBars.Vertical;
            this.textBox1.Size = new Size(903, 209);
            this.textBox1.TabIndex = 0;
            this.textBox1.TabStop = false;
            // 
            // textBox2
            // 
            this.textBox2.Font = new Font("Consolas", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            this.textBox2.Location = new Point(76, 14);
            this.textBox2.Margin = new Padding(4, 3, 4, 3);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Size(748, 23);
            this.textBox2.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            this.label2.Location = new Point(14, 17);
            this.label2.Margin = new Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new Size(45, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "Файл:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.numericUpDown1);
            this.groupBox1.Controls.Add(this.textBox3);
            this.groupBox1.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            this.groupBox1.Location = new Point(14, 48);
            this.groupBox1.Margin = new Padding(4, 3, 4, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new Padding(4, 3, 4, 3);
            this.groupBox1.Size = new Size(449, 408);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Шапка";
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Location = new Point(6, 20);
            this.button2.Margin = new Padding(4, 3, 4, 3);
            this.button2.Name = "button2";
            this.button2.Size = new Size(133, 28);
            this.button2.TabIndex = 3;
            this.button2.Text = "Запомнить";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new EventHandler(this.Button2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new Point(146, 23);
            this.label3.Margin = new Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new Size(121, 16);
            this.label3.TabIndex = 2;
            this.label3.Text = "Конечная строка:";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Enabled = false;
            this.numericUpDown1.Location = new Point(299, 21);
            this.numericUpDown1.Margin = new Padding(4, 3, 4, 3);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new Size(144, 22);
            this.numericUpDown1.TabIndex = 1;
            this.numericUpDown1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.ValueChanged += new EventHandler(this.NumericUpDown1_ValueChanged);
            // 
            // textBox3
            // 
            this.textBox3.AcceptsReturn = true;
            this.textBox3.Enabled = false;
            this.textBox3.Font = new Font("Consolas", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            this.textBox3.Location = new Point(7, 54);
            this.textBox3.Margin = new Padding(4, 3, 4, 3);
            this.textBox3.Multiline = true;
            this.textBox3.Name = "textBox3";
            this.textBox3.ScrollBars = ScrollBars.Vertical;
            this.textBox3.Size = new Size(434, 346);
            this.textBox3.TabIndex = 0;
            this.textBox3.WordWrap = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button3);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.numericUpDown2);
            this.groupBox2.Controls.Add(this.textBox4);
            this.groupBox2.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            this.groupBox2.Location = new Point(470, 48);
            this.groupBox2.Margin = new Padding(4, 3, 4, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new Padding(4, 3, 4, 3);
            this.groupBox2.Size = new Size(449, 408);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Концовка";
            // 
            // button3
            // 
            this.button3.Enabled = false;
            this.button3.Location = new Point(6, 20);
            this.button3.Margin = new Padding(4, 3, 4, 3);
            this.button3.Name = "button3";
            this.button3.Size = new Size(133, 28);
            this.button3.TabIndex = 4;
            this.button3.Text = "Запомнить";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new EventHandler(this.Button3_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new Point(146, 24);
            this.label4.Margin = new Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new Size(130, 16);
            this.label4.TabIndex = 4;
            this.label4.Text = "Начальная строка:";
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.Enabled = false;
            this.numericUpDown2.Location = new Point(307, 21);
            this.numericUpDown2.Margin = new Padding(4, 3, 4, 3);
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new Size(135, 22);
            this.numericUpDown2.TabIndex = 3;
            this.numericUpDown2.ValueChanged += new EventHandler(this.NumericUpDown2_ValueChanged);
            // 
            // textBox4
            // 
            this.textBox4.AcceptsReturn = true;
            this.textBox4.Enabled = false;
            this.textBox4.Font = new Font("Consolas", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            this.textBox4.Location = new Point(7, 54);
            this.textBox4.Margin = new Padding(4, 3, 4, 3);
            this.textBox4.Multiline = true;
            this.textBox4.Name = "textBox4";
            this.textBox4.ScrollBars = ScrollBars.Vertical;
            this.textBox4.Size = new Size(434, 346);
            this.textBox4.TabIndex = 1;
            this.textBox4.WordWrap = false;
            // 
            // button4
            // 
            this.button4.Enabled = false;
            this.button4.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            this.button4.Location = new Point(330, 464);
            this.button4.Margin = new Padding(4, 3, 4, 3);
            this.button4.Name = "button4";
            this.button4.Size = new Size(279, 40);
            this.button4.TabIndex = 5;
            this.button4.Text = "Разделить";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new EventHandler(this.Button4_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Enabled = false;
            this.label5.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            this.label5.ForeColor = Color.Gray;
            this.label5.Location = new Point(840, 726);
            this.label5.Margin = new Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new Size(64, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "© dece1ver";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            this.label1.Location = new Point(10, 485);
            this.label1.Margin = new Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new Size(95, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Инорфмация:";
            // 
            // label6
            // 
            this.label6.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            this.label6.ImageAlign = ContentAlignment.BottomCenter;
            this.label6.Location = new Point(14, 726);
            this.label6.Margin = new Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new Size(819, 18);
            this.label6.TabIndex = 8;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileOk += new CancelEventHandler(this.OpenFileDialog1_FileOk);
            // 
            // button1
            // 
            this.button1.Font = new Font("Consolas", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            this.button1.Location = new Point(832, 13);
            this.button1.Margin = new Padding(4, 3, 4, 3);
            this.button1.Name = "button1";
            this.button1.Size = new Size(88, 25);
            this.button1.TabIndex = 4;
            this.button1.Text = "Обзор";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new EventHandler(this.Button1_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            this.label7.Location = new Point(638, 477);
            this.label7.Margin = new Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new Size(60, 16);
            this.label7.TabIndex = 5;
            this.label7.Text = "Подача:";
            this.label7.Visible = false;
            // 
            // textBox5
            // 
            this.textBox5.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            this.textBox5.Location = new Point(718, 473);
            this.textBox5.Margin = new Padding(4, 3, 4, 3);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new Size(86, 22);
            this.textBox5.TabIndex = 9;
            this.textBox5.Visible = false;
            // 
            // button5
            // 
            this.button5.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            this.button5.Location = new Point(811, 472);
            this.button5.Margin = new Padding(4, 3, 4, 3);
            this.button5.Name = "button5";
            this.button5.Size = new Size(108, 28);
            this.button5.TabIndex = 5;
            this.button5.Text = "Запомнить";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Visible = false;
            this.button5.Click += new EventHandler(this.Button5_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(933, 750);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.textBox5);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.Margin = new Padding(4, 3, 4, 3);
            this.MaximumSize = new Size(949, 789);
            this.MinimumSize = new Size(949, 789);
            this.Name = "MainWindow";
            this.Text = "Разделятель";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((ISupportInitialize)(this.numericUpDown2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox textBox1;
        private TextBox textBox2;
        private Label label2;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private TextBox textBox3;
        private TextBox textBox4;
        private Label label3;
        private NumericUpDown numericUpDown1;
        private Button button2;
        private Label label4;
        private NumericUpDown numericUpDown2;
        private Button button3;
        private Button button4;
        private Label label5;
        private Label label1;
        private Label label6;
        private OpenFileDialog openFileDialog1;
        private Button button1;
        private Label label7;
        private TextBox textBox5;
        private Button button5;
    }
}

