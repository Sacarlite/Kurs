using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Data
{
    [DataContract]
    public class InputParameters
    {
        public InputParameters()
        {
            this.Level = 0;
            this.Concentration = 0;
            this.Expenditure = 0;
        }
        public InputParameters(double expenditure, double level, double concentration)
        {
            this.Level = level;
            this.Concentration = concentration;
            this.Expenditure = expenditure;
        }
        [DataMember(Name = "expenditure")]
        public double Expenditure { get; set; }
        [DataMember(Name = "level")]
        public double Level { get; set; }
        [DataMember(Name = "concentration")]
        public double Concentration { get; set; }

    }
    [DataContract]
    public class ManyParameters
    {

        public ManyParameters(ObservableCollection<InputParameters> parameters)
        {
            Expenditures = new List<double>();
            Levels = new List<double>();
            Concentrations = new List<double>();
            foreach (var elem in parameters)
            {
                Expenditures.Add(elem.Expenditure);
                Levels.Add(elem.Level);
                Concentrations.Add(elem.Concentration);
            }

        }
        [DataMember(Name = "expenditures")]
        public List<double> Expenditures { get; set; }

        [DataMember(Name = "levels")]
        public List<double> Levels { get; set; }

        [DataMember(Name = "concentrations")]
        public List<double> Concentrations { get; set; }
        public ManyParameters() { }
    }
}
