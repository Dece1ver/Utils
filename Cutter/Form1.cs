using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cutter
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        string fileName;
        string lastXY;
        int lastXYIndex;
        int parts;
        int headEndPoint = 0;
        int endingStartPoint = 0;
        int partStrings;
        int startLine;
        int cutLine;
        public List<string> body = new List<string> { };
        public List<string> head = new List<string> { };
        public List<string> ending = new List<string> { };
        string[] lines;
        string feed;

        private void OpenFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            body.Clear();
            head.Clear();
            ending.Clear();
            feed = "";
            label7.Visible = false;
            textBox5.Visible = false;
            button5.Visible = false;
            headEndPoint = 0;
            endingStartPoint = 0;
            fileName = openFileDialog1.FileName;
            textBox2.Text = openFileDialog1.SafeFileName;
            long fileSize = new FileInfo(fileName).Length;
            textBox1.Text = "";
            textBox3.Text = "";
            textBox1.Text += $"Открыт файл:...........{fileName}\r\n";
            textBox1.Text += $"Общий размер:..........{fileSize / 1024} Кб\r\n";
            textBox1.Text += $"Допустимый размер:.....480 Кб\r\n";
            parts = (Convert.ToInt32(fileSize / 1024)) / 480 + 1;
            textBox1.Text += $"Частей:................{parts}\r\n";
            lines = File.ReadAllLines(fileName);
            textBox1.Text += $"Строк всего:...........{lines.Length}\r\n";
            numericUpDown1.Maximum = lines.Length;
            numericUpDown2.Maximum = lines.Length;
            partStrings = lines.Length / parts;
            textBox1.Text += $"Строк в каждой части:..≈ {partStrings.ToString().Remove(partStrings.ToString().Length - 3, 3)}k\r\n";
            headEndPoint = 0;
            foreach (string line in lines)
            {
                if (line.Contains("F") && line.Contains("G1"))
                {
                    break;
                }
                else
                {
                    head.Add(line);
                    headEndPoint++;
                }
            }
            numericUpDown1.Value = Convert.ToDecimal(headEndPoint);
            foreach (string line in lines)
            {
                if (line.Contains("F") && line.Contains("G1"))
                {
                    feed = line.Remove(0, line.IndexOf('F'));
                    textBox1.Text += $"Подача:................{feed}\r\n\r\n";
                    break;
                }
            }
            if (feed == null || feed == "")
            {
                label7.Visible = true;
                textBox5.Visible = true;
                button5.Visible = true;
                textBox1.AppendText("Подача не обнаружена. Можно установить подачу в появившейся форме." + Environment.NewLine);
            }

            endingStartPoint = 0;
            Array.Reverse(lines);
            foreach (string line in lines)
            {
                if (line.Contains("G0"))
                {
                    endingStartPoint++;
                    break;
                }
                else
                {
                    endingStartPoint++;
                }
            }
            Array.Reverse(lines);
            endingStartPoint = lines.Length - endingStartPoint;
            numericUpDown2.Value = Convert.ToDecimal(endingStartPoint) + 1;
            string[] endLines = new string[lines.Length - endingStartPoint];
            Array.Copy(lines, endingStartPoint, endLines, 0, (lines.Length - endingStartPoint));

            if ((fileSize / 1024) < 480)
            {
                textBox1.Text = "Файл нормального размера, ничего разделять не надо.";
                button4.Enabled = false;
                numericUpDown1.Enabled = false;
                numericUpDown2.Enabled = false;
                button2.Enabled = false;
                button3.Enabled = false;
                textBox3.Enabled = false;
                textBox3.Text = "";
                textBox4.Text = "";
                textBox4.Enabled = false;
            }
            else
            {
                button4.Enabled = true;
                numericUpDown1.Enabled = true;
                numericUpDown2.Enabled = true;
                button2.Enabled = true;
                button3.Enabled = true;
                textBox3.Enabled = true;
                textBox4.Enabled = true;
            }

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            PrintHead();
            PrintEnding();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            head.Clear();
            head.AddRange(textBox3.Text.Split((string[])null, StringSplitOptions.RemoveEmptyEntries));
            textBox3.Clear();
            head.RemoveAt(head.Count - 1);
            PrintHead();
            textBox1.AppendText("Измененная шапка сохранена." + Environment.NewLine);
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            ending.Clear();
            ending.AddRange(textBox4.Text.Split((string[])null, StringSplitOptions.RemoveEmptyEntries));
            textBox4.Clear();
            ending.RemoveAt(ending.Count - 1);
            PrintEnding();
            textBox1.AppendText("Измененная концовка сохранена." + Environment.NewLine);
        }


        private void PrintHead()
        {
            textBox3.Text = "";
            foreach (string line in head)
            {
                textBox3.AppendText(line + Environment.NewLine);
            }
        }
        private void PrintEnding()
        {
            textBox4.Text = "";
            foreach (string line in ending)
            {
                textBox4.AppendText(line + Environment.NewLine);
            }
        }

        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            int count = 0;
            headEndPoint = Convert.ToInt32(numericUpDown1.Value);
            head.Clear();
            foreach (string line in lines)
            {
                if (count < headEndPoint)
                {
                    head.Add(line);
                    count++;
                }
                else
                {
                    break;
                }
            }
            PrintHead();
        }

        private void NumericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            endingStartPoint = Convert.ToInt32(numericUpDown2.Value) - 1;
            ending.Clear();
            string[] endLines = new string[lines.Length - endingStartPoint];
            Array.Copy(lines, endingStartPoint, endLines, 0, (lines.Length - endingStartPoint));
            ending.AddRange(endLines);
            PrintEnding();
        }

        private void SaveBody()
        {
            string[] tempBody = new string[lines.Length - head.Count - ending.Count];
            Array.Copy(lines, headEndPoint, tempBody, 0, lines.Length - headEndPoint - (lines.Length - endingStartPoint));
            body.AddRange(tempBody);
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            SaveBody();
            cutLine = 0;
            textBox1.AppendText($"Название детали определено: {FindName(head)}" + Environment.NewLine);
            textBox1.AppendText($"Тело программы определено строками {numericUpDown1.Value} - {numericUpDown2.Value}. Всего {body.Count} строк." + Environment.NewLine);
            for (int i = 1; i < parts + 1; i++)
            {
                CutPart(i);
            }
        }

        private void CutPart(int part)
        {
            using (StreamWriter stream = new StreamWriter($"{openFileDialog1.FileName}-{part}", false))
            {
                startLine = cutLine;
                if (part != parts)
                {
                    cutLine += partStrings;

                    textBox1.AppendText($"\r\nСоставление части: {part}" + Environment.NewLine);
                    textBox1.AppendText($"Создание файла:................{openFileDialog1.FileName}-{part}" + Environment.NewLine);
                    textBox1.AppendText($"Изменение названия на:.........{FindName(head)}-{part}" + Environment.NewLine);

                    //
                    // запись шапки
                    //
                    string newName = FindName(head);
                    foreach (string line in head)
                    {
                        // замена имени
                        if (line.Contains("(") || line.Contains(")"))
                        {
                            if (part == 1)
                            {
                                stream.WriteLine(line.Substring(0, line.Length - 1) + $"-{part})");
                            }
                            else if (part >= 10)
                            {
                                stream.WriteLine(line.Substring(0, line.Length - 4) + $"-{part})");
                            }
                            else
                            {
                                stream.WriteLine(line.Substring(0, line.Length - 3) + $"-{part})");
                            }
                        }
                        // замена XY
                        else if (line.Contains("X") && line.Contains("Y") && part > 1)
                        {
                            if (!lastXY.Contains("G0"))
                            {
                                lastXY = $"{lastXY.Substring(0, lastXY.IndexOf("X"))}G0{lastXY.Substring(lastXY.IndexOf("X"), lastXY.Length - lastXY.IndexOf("X"))}";
                            }
                            stream.WriteLine(lastXY.Replace("G1", "G0").Replace("G2", "G0").Replace("G3", "G0"));
                            textBox1.AppendText($"Заменена координат X, Y :......{lastXY.Replace("G1", "G0").Replace("G2", "G0").Replace("G3", "G0")}" + Environment.NewLine);
                        }
                        else
                        {
                            stream.WriteLine(line);
                        }
                    }

                    textBox1.AppendText($"Смещение для поиска на:........[{cutLine}] {body[cutLine]}" + Environment.NewLine);
                    int count = 0;
                    foreach (string line in body)
                    {

                        if (count >= cutLine && line.Contains("X") && line.Contains("Y"))
                        {
                            lastXYIndex = count;
                            lastXY = line;
                        }
                        else if (count >= cutLine && line.Contains("G0"))
                        {
                            cutLine = count;
                            break;
                        }
                        count++;
                    }
                    textBox1.AppendText($"Последние координаты X, Y:.....[{lastXYIndex}] {lastXY}" + Environment.NewLine);
                    textBox1.AppendText($"Место разделения:..............[{cutLine}] {body[cutLine]}" + Environment.NewLine);

                    //
                    // запись тела
                    //
                    string[] tempBody = body.GetRange(startLine, cutLine - startLine).ToArray();
                    bool feedIsWrited = false;
                    foreach (string line in tempBody)
                    {
                        // добавление подачи
                        if (!feedIsWrited && line.Contains("G1") && part != 1)
                        {
                            stream.WriteLine(line + feed);
                            feedIsWrited = true;
                            textBox1.AppendText($"Добавлена подача:..............{line + feed}" + Environment.NewLine);
                        }
                        else
                        {
                            stream.WriteLine(line);
                        }
                    }

                    //
                    // запись концовки
                    //
                    foreach (string line in ending)
                    {
                        if (line != "\r\n")
                        {
                            stream.WriteLine(line);
                        }
                    }
                    textBox1.AppendText("Часть составлена." + Environment.NewLine);
                }
                else
                {
                    textBox1.AppendText($"\r\nСоставление части: {part}" + Environment.NewLine);
                    textBox1.AppendText("Смещение для поиска не нужно, последняя часть." + Environment.NewLine);
                    textBox1.AppendText($"Создание файла:................{openFileDialog1.FileName}-{part}" + Environment.NewLine);
                    textBox1.AppendText($"Изменение названия на:.........{FindName(head)}-{part}" + Environment.NewLine);
                    string newName = FindName(head);
                    //
                    // запись шапки
                    //
                    foreach (string line in head)
                    {
                        // замена имени
                        if (line.Contains("(") || line.Contains(")"))
                        {
                            if (part == 1)
                            {
                                stream.WriteLine(line.Substring(0, line.Length - 1) + $"-{part})");
                            }
                            else if (part >= 10)
                            {
                                stream.WriteLine(line.Substring(0, line.Length - 4) + $"-{part})");
                            }
                            else
                            {
                                stream.WriteLine(line.Substring(0, line.Length - 3) + $"-{part})");
                            }
                        }
                        // замена XY
                        else if (line.Contains("X") && line.Contains("Y") && part > 1)
                        {
                            if (!lastXY.Contains("G0"))
                            {
                                lastXY = $"{lastXY.Substring(0, lastXY.IndexOf("X"))}G0{lastXY.Substring(lastXY.IndexOf("X"), lastXY.Length - lastXY.IndexOf("X"))}";
                            }
                            stream.WriteLine(lastXY.Replace("G1", "G0").Replace("G2", "G0").Replace("G3", "G0"));
                            textBox1.AppendText($"Заменена координат X, Y:.......{lastXY.Replace("G1", "G0").Replace("G2", "G0").Replace("G3", "G0")}" + Environment.NewLine);
                        }
                        else
                        {
                            stream.WriteLine(line);
                        }
                    }

                    //
                    // запись тела
                    //
                    string[] tempBody = body.GetRange(startLine, body.Count - startLine).ToArray();
                    bool feedIsWrited = false;
                    foreach (string line in tempBody)
                    {
                        // добавление подачи
                        if (!feedIsWrited && line.Contains("G1"))
                        {
                            stream.WriteLine(line + feed);
                            feedIsWrited = true;
                            textBox1.AppendText($"Добавлена подача:..............{line + feed}" + Environment.NewLine);
                        }
                        else
                        {
                            stream.WriteLine(line);
                        }
                    }

                    //
                    // запись концовки
                    //
                    foreach (string line in ending)
                    {
                        stream.WriteLine(line);
                    }
                    textBox1.AppendText("Программа разделена." + Environment.NewLine);
                }
            }
        }

        private string FindName(List<string> head)
        {
            string name = "PART NAME";
            foreach (string line in head)
            {
                if (line.Contains("(") || line.Contains(")"))
                {
                    name = line.Substring(line.IndexOf("(") + 1, line.Length - line.IndexOf("(") - 2);
                    break;
                }
            }

            return name;
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            feed = textBox5.Text.Contains("F") ? textBox5.Text : $"F{textBox5.Text}";
            textBox1.AppendText($"Установлена подача:....{feed}" + Environment.NewLine);
        }
    }
}
