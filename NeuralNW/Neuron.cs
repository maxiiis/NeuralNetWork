using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNW
{
    class Neuron
    {
        public List<double> Weights { get; }
        public double Output { get; private set; }
        public double Delta { get; private set; }
        public List<double> Inputs { get; }

        public Neuron(int inputCount)
        {
            Weights = new List<double>();
            Inputs = new List<double>();

            SetRandomWeights(inputCount);
        }

        private void SetRandomWeights(int inputCount)
        {
            Random random = new Random();

            for (int i = 0; i < inputCount; i++)
            {
                Weights.Add(random.NextDouble());
            }
        }

        public double Sigmoid(double x)
        {
            double result = 1.0 / (1.0 + Math.Exp(-x));
            return result;
        }

        public double GetResult(List<double> inputs)
        {
            for (int i = 0; i < inputs.Count; i++)
            {
                Inputs[i] = inputs[i];
            }

            double sum = 0.0;

            for (int i = 0; i < inputs.Count; i++)
            {
                sum += inputs[i] * Weights[i];
            }

            double result = Sigmoid(sum);
            return result;
        }
    }
}
