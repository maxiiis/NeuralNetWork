using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeuralNW
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            NeuralNetwork NW = new NeuralNetwork(3,1,2);
            NW.GetResult(0,0,1);
        }
    }
}
