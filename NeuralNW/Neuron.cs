using System;

namespace NeuralNW
{
    public class Neuron
    {
        public double[] Weights { get; }
        public double Output { get; private set; }
        public double Delta { get; private set; }
        public double[] Inputs { get; }

        public Neuron(int inputCount,double[] weights = null)
        {
            Weights = new double[inputCount];
            Inputs = new double[inputCount];

            if (weights == null)
                SetRandomWeights(inputCount);
            else
                for (int i = 0; i < Weights.Length; i++)
                    Weights[i] = weights[i];
        }

        private void SetRandomWeights(int inputCount)
        {
            if (Inputs.Length == 1)
            {
                Weights[0] = 1;
                return;
            }

            Random random = new Random();

            for (int i = 0; i < inputCount; i++)
            {
                Weights[i] = random.NextDouble();
            }
        }

        public double Sigmoid(double x)
        {
            double result = 1.0 / (1.0 + Math.Exp(-x));
            return result;
        }

        public double SigmoidDx(double x)
        {
            double sigmoid = Sigmoid(x);
            double result = sigmoid / (1 - sigmoid);
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

            if (Inputs.Length == 1)
            {
                Output = sum;
            }
            else
            {
                Output = Sigmoid(sum);
            }

            return Output;
        }

        public void Learn(double error,double learnSpeed)
        {
            if (Inputs.Length == 1)
                return;


            Delta = error * SigmoidDx(Output);

            for (int i = 0; i < Weights.Length; i++)
            {
                double weight = Weights[i];
                double input = Inputs[i];
                double newWeight = weight - input * Delta * learnSpeed;
                Weights[i] = newWeight;
            }
        }
    }
}
