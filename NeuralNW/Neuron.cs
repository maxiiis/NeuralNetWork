using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNW
{
    class Neuron
    {
        public double[] Weights { get; }
        public double Output { get; private set; }
        public double Delta { get; private set; }
        public double[] Inputs { get; }

        public Neuron(int inputCount)
        {
            Weights = new double[inputCount];
            Inputs = new double[inputCount];

            SetRandomWeights(inputCount);
        }

        private void SetRandomWeights(int inputCount)
        {
            Random random = new Random();

            for (int i = 0; i < inputCount; i++)
            {
                Weights[i] =random.NextDouble();
            }
        }

        public double Sigmoid(double x)
        {
            double result = 1.0 / (1.0 + Math.Exp(-x));
            return result;
        }

        public double GetResult(double[] inputs)
        {
            for (int i = 0; i < inputs.Length; i++)
            {
                Inputs[i] = inputs[i];
            }

            double sum = 0.0;

            for (int i = 0; i < inputs.Length; i++)
            {
                sum += inputs[i] * Weights[i];
            }

            Output = Sigmoid(sum);
            return Output;
        }
    }
}
