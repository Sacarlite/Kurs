using System.Runtime.Serialization;

namespace AproximateMetods
{

    [DataContract]//Показывает что объект может быть сериализован (это не является необходимостью но помогает при сериализации)
    //ссылка на почитать: https://stackoverflow.com/questions/4836683/when-to-use-datacontract-and-datamember-attributes
    public class Linear
    {
        public Linear()
        {

        }
        public Linear(double a, double b)
        {
            this.a = a;
            this.b = b;
        }

        [DataMember(Name = "a")]
        public double a { get; set; }

        [DataMember(Name = "b")]
        public double b { get; set; }
        public override string ToString()
        {
            return "Линейная аппроксимация";
        }
    }
    [DataContract]
    public class Parabola
    {
        public Parabola()
        {

        }
        public Parabola(double a, double b, double c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
        }

        [DataMember(Name = "a")]
        public double a { get; set; }

        [DataMember(Name = "b")]
        public double b { get; set; }
        [DataMember(Name = "c")]
        public double c { get; set; }
        public override string ToString()
        {
            return "Параболическая аппроксимация";
        }
    }





}