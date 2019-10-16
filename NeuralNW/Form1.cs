using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeuralNW
{
    public partial class Form1 : Form
    {
        private bool Draw;

        Bitmap picture;
        private NeuralNetwork NW;

        public Form1()
        {
            InitializeComponent();
        }

        private void pictureBox1_MouseDown(object sender,MouseEventArgs e)
        {
            Draw = true;
        }

        private void pictureBox1_MouseUp(object sender,MouseEventArgs e)
        {
            Draw = false;
        }

        private void pictureBox1_MouseMove(object sender,MouseEventArgs e)
        {
            Graphics g = Graphics.FromImage(picture);
            if (Draw)
            {
                g.FillEllipse(Brushes.White,new Rectangle(e.Location,new Size(15,15)));
                pictureBox1.Image = picture;
            }
        }

        private void Form1_Load(object sender,EventArgs e)
        {
            picture = new Bitmap(pictureBox1.Width,pictureBox1.Height);
            Graphics g = Graphics.FromImage(picture);
            g.Clear(Color.Black);
            pictureBox1.Image = picture;
        }

        private void button3_Click(object sender,EventArgs e)
        {
            Bitmap inputPicture = new Bitmap(pictureBox1.Image);

            inputPicture = Crop(inputPicture);

            inputPicture = new Bitmap(inputPicture,new Size(28,28));

            //pictureBox1.Image = inputPicture;

            List<double> inputsL = new List<double>();

            for (int y = 0; y < 28; y++)
                for (int x = 0; x < 28; x++)
                    inputsL.Add(inputPicture.GetPixel(y,x).R >= 128 ? 1 : 0);

            double[] inputs = inputsL.ToArray();

            //DrawImage(inputs.Select(x => x.ToString()).ToArray());

            double[] output = NW.GetResult(inputs);

            label1.Text = $"B = {Math.Round(output[0],5)*100}";
            label2.Text = $"C = {Math.Round(output[1],5) * 100}";
            label3.Text = $"M = {Math.Round(output[2],5) * 100}";
        }

        private void button2_Click(object sender,EventArgs e)
        {
            NW = new NeuralNetwork(784,3,392);
        }

        int num = 0;

        private void button4_Click(object sender,EventArgs e)
        {
            int Size = 600;
            int Bs = 0;
            int Cs = 0;
            int Ms = 0;

            string path = @"C:\Users\mak36\OneDrive\Рабочий стол\MAIN\4 курс\ИС\emnist\emnist-letters-train.csv";

            string[] images = File.ReadAllLines(path);

            List<double[]> expected = new List<double[]>();

            List<double[]> inputs = new List<double[]>();

            for (int i = num + 1; i < images.Length - 1; i++)
            {
                string[] buf = images[i].Split(',');

                int symbolNum = Convert.ToInt32(buf[0]);

                double[] output = new double[3];

                switch (symbolNum)
                {
                    case 2:
                        if (Bs < Size / 3)
                        {
                            output[0] = 1;
                            expected.Add(output);
                            Bs++;
                        }
                        //DrawImage(buf);
                        //num = i;
                        break;
                    case 3:
                        if (Cs < Size / 3)
                        {
                            output[1] = 1;
                            expected.Add(output);
                            Cs++;
                        }
                        //DrawImage(buf);
                        //num = i;
                        break;
                    case 13:
                        if (Ms < Size / 3)
                        {
                            output[2] = 1;
                            expected.Add(output);
                            Ms++;
                        }
                        break;
                    default:
                        continue;
                }

                if (num == i)
                    break;

                double[] buf2 = buf.Skip(1).Select(x => Convert.ToDouble(x)).ToArray();

                inputs.Add(buf2);

                if (inputs.Count == Size)
                    break;
            }

            double[,] Inputs = ltoD(inputs,Size);

            double error = NW.Learn(expected.ToArray(),Inputs,Convert.ToInt32(textBox3.Text),0.1);

            label4.Text = $"Error = {Math.Round(error,7)}";
        }

        private void DrawImage(string[] buf)
        {
            Bitmap bmp = new Bitmap(28,28);

            string[] pixels = buf;//.Skip(1).ToArray();

            for (int i = 0; i < 28 * 28; i++)
            {
                bmp.SetPixel(i % 28,i / 28,Convert.ToDouble(pixels[i]) >= 128 ? Color.White : Color.Black);
            }

            pictureBox1.Image = bmp;
            pictureBox1.Invalidate();

            List<string> pixelsI = new List<string>();

            for (int i = 0; i < 28 * 28; i++)
            {
                //pixelsI.Add(Convert.ToDouble(pixels[i]) >= 128 ? "1" : "0");
                pixelsI.Add(pixels[i]);
            }

            List<string> lists = new List<string>();

            for (int i = 0; i < 28; i++)
            {
                string newstring = "";
                for (int j = 0; j < 28; j++)
                {
                    newstring += pixelsI.First();
                    pixelsI.RemoveAt(0);
                }
                lists.Add(newstring);
            }

            File.WriteAllLines("1.txt",lists.ToArray());
        }

        private double[,] ltoD(List<double[]> inputs,int size)
        {
            double[,] rezult = new double[size,inputs[0].Length];

            for (int i = 0; i < size; i++)
            {
                for (int k = 0; k < inputs[0].Length - 1; k++)
                {
                    rezult[i,k] = inputs[i][k] >= 128 ? 1 : 0;
                }

            }

            return rezult;
        }

        private void button6_Click(object sender,EventArgs e)
        {
            Graphics g = Graphics.FromImage(pictureBox1.Image);
            g.Clear(Color.Black);
            pictureBox1.Invalidate();
        }

        private Bitmap Crop(Bitmap bmp)
        {
            Rectangle rect = new Rectangle(Point.Empty,bmp.Size);

            int yUP = -1;
            int yDOWN = -1;
            int xLEFT = -1;
            int xRIGHT = -1;
            for (int i = 0; i < bmp.Height; i++)
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    if (bmp.GetPixel(j,i).R != 0)
                    {
                        if (yUP == -1) yUP = i;
                        if (yDOWN < i) yDOWN = i;
                    }
                }

            }

            for (int i = 0; i < bmp.Height; i++)
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    if (bmp.GetPixel(i,j).R != 0)
                    {
                        if (xLEFT == -1) xLEFT = i;
                        if (xRIGHT < i) xRIGHT = i;
                    }
                }
            }
            if (xLEFT != -1 || xRIGHT != -1 || yUP != -1 || yDOWN != -1)
                rect = new Rectangle(new Point(xLEFT - 1,yUP - 1),new Size(xRIGHT - xLEFT + 10,yDOWN - yUP + 10));
            //+- буффер?


            Bitmap crop = new Bitmap(rect.Width,rect.Height);

            Graphics g = Graphics.FromImage(crop);

            g.DrawImage(bmp,0,0,rect,GraphicsUnit.Pixel);

            return crop;
        }

        private void button7_Click(object sender,EventArgs e)
        {
            NW.Save();
        }

        private void button5_Click(object sender,EventArgs e)
        {
            NW = new NeuralNetwork("NW.bin");
        }
    }
}
