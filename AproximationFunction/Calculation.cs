using AproximateMetods;
using Data;

namespace AproximationFunction
{

    public class Aproximation
    {

        public Aproximation(ManyParameters parametrsList, double step, Type selectedType)
        {
            this._step = step;
            this._selectedType = selectedType;
            this._parametrsList = parametrsList;
        }
        private ManyParameters _parametrsList;
        private double _step;
        private Type _selectedType;
        private Linear GetLinearCoeff(List<double> Expenditure, List<double> values)
        {
            double SumX = 0.0;
            double SumSquaredX = 0.0;
            double SumY = 0.0;
            double SumXAndY = 0.0;
            for (int i = 0; i < Expenditure.Count; i++)
            {
                SumX += Expenditure[i];
                SumSquaredX += Math.Pow(Expenditure[i], 2);
                SumY += values[i];
                SumXAndY += values[i] * Expenditure[i];
            }
            double a = (Expenditure.Count * SumXAndY - SumX * SumY) / (Expenditure.Count * SumSquaredX - Math.Pow(SumX, 2));
            double b = (SumY - a * SumX) / (Expenditure.Count);
            return new Linear(a, b);
        }
        private Parabola GetParabolicCoeff(List<double> Expenditure, List<double> values)
        {
            double SumX = 0.0;
            double SumX2 = 0.0;
            double SumX3 = 0.0;
            double SumX4 = 0.0;
            double SumY = 0.0;
            double SumXAndY = 0.0;
            double SumX2AndY = 0.0;
            for (int i = 0; i < Expenditure.Count; i++)
            {
                SumX += Expenditure[i];
                SumX2 += Math.Pow(Expenditure[i], 2);
                SumX3 += Math.Pow(Expenditure[i], 3);
                SumX4 += Math.Pow(Expenditure[i], 4);
                SumY += values[i];
                SumXAndY += Expenditure[i] * values[i];
                SumX2AndY += Math.Pow(Expenditure[i], 2) * values[i];
            }
            double det0 = GetDet(new List<List<double>>
                { new List<double>(){ Expenditure.Count,SumX,SumX2 },
            new List < double >() { SumX, SumX2, SumX3 },
            new List < double >() { SumX2, SumX3, SumX4 }
            }
            );
            double det1 = GetDet(new List<List<double>> { new List<double>() { SumY, SumX, SumX2 }, new List<double>() { SumXAndY, SumX2, SumX3 }, new List<double>() { SumX2AndY, SumX3, SumX4 } });
            double det2 = GetDet(new List<List<double>> { new List<double>() { Expenditure.Count, SumY, SumX2 }, new List<double>() { SumX, SumXAndY, SumX3 }, new List<double>() { SumX2, SumX2AndY, SumX4 } });
            double det3 = GetDet(new List<List<double>> { new List<double>() { Expenditure.Count, SumX, SumY }, new List<double>() { SumX, SumX2, SumXAndY }, new List<double>() { SumX2, SumX3, SumX2AndY } });

            return new(det1 / det0, det2 / det0, det3 / det0);
        }
        private double GetDet(List<List<double>> matrix)
        {
            if (matrix.Count == 1)
            {
                return matrix[0][0];
            }
            double det = 0.0;
            for (int i = 0; i < matrix.Count; i++)
            {
                if (i % 2 == 0)
                {
                    det += -matrix[0][i] * GetDet(GetMinor(matrix, i));
                }
                else
                {

                    det += matrix[0][i] * GetDet(GetMinor(matrix, i));
                }
            }

            return det;
        }
        private List<List<double>> GetMinor(List<List<double>> matrix, int index)
        {
            List<List<double>> tmpMatrix = new List<List<double>>();
            for (int i = 1; i < matrix.Count; i++)
            {
                List<double> filterList = new List<double>();
                for (int j = 0; j < matrix[i].Count; j++)
                {
                    int counter = 0;
                    if (j != index)
                    {
                        filterList.Add(matrix[i][j]);
                        counter++;

                    }
                }
                tmpMatrix.Add(filterList);
            }

            return tmpMatrix;

        }
        private (double, double) GetMinValue()
        {
            return (_parametrsList.Expenditures.Min(), _parametrsList.Expenditures.Max());
        }
        public ManyParameters Aproximat()
        {
            ManyParameters chartedPoints = new ManyParameters();
            chartedPoints.Levels = GetDoubles(_parametrsList.Expenditures, _parametrsList.Levels);
            chartedPoints.Concentrations = GetDoubles(_parametrsList.Expenditures, _parametrsList.Concentrations);
            chartedPoints.Expenditures = new List<double>();
            for (double i = GetMinValue().Item1; i <= GetMinValue().Item2; i += 0.1)
            {
                chartedPoints.Expenditures.Add(i);
            }
            return chartedPoints;
        }
        private List<double> GetDoubles(List<double> Expenditures, List<double> values)
        {
            List<double> doubles = new List<double>();
            
            if (_selectedType==typeof(Linear))
            {

                var linearMethod = GetLinearCoeff(Expenditures, values);
                for (double i = GetMinValue().Item1; i <= GetMinValue().Item2; i += _step)
                {
                    doubles.Add(linearMethod.a * i + linearMethod.b);

                }
            }
            else if (_selectedType == typeof(Parabola))
            {
                //TODO Даша сделай блять параболическую аппроксимацию
            }

            return doubles;
        }
        public ManyParameters Aproximate()
        {
            ManyParameters chartedPoints = new ManyParameters();
            chartedPoints.Levels = GetDoubles(_parametrsList.Expenditures, _parametrsList.Levels);
            chartedPoints.Concentrations = GetDoubles(_parametrsList.Expenditures, _parametrsList.Concentrations);
            chartedPoints.Expenditures = new List<double>();
            for (double i = GetMinValue().Item1; i <= GetMinValue().Item2; i += 0.1)
            {
                chartedPoints.Expenditures.Add(i);
            }
            return chartedPoints;
        }
    }
}
