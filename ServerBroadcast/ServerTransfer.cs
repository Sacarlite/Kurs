using System.Text.Json;
using System.Text;
using Data;
using Newtonsoft.Json;

namespace ServerBroadcast
{
    public class ServerTransfer
    {
        public ManyParameters AppDecoder(string info)
        {
            return JsonConvert.DeserializeObject<ManyParameters>(info);
        }

        public string AppEncoder(ManyParameters Info, object selectedMethod, double step)
        {
            var builder = new StringBuilder();

            
            builder.Append(JsonConvert.SerializeObject(step) + '@');
            builder.Append(JsonConvert.SerializeObject(selectedMethod.GetType()) + '#');
            builder.Append(JsonConvert.SerializeObject(Info));
            return builder.ToString();
        }

        public (double, Type, ManyParameters) ServerDecoder(string info)
        {

            double step = 0;
            Type? selectedType=null;
            var builder = new StringBuilder();
            var points = new List<InputParameters>();
            foreach (var elem in info)
                if (elem == '@')
                {
                    step = JsonConvert.DeserializeObject<double>(builder.ToString());
                    builder.Clear();
                }
                else if (elem == '#')
                {
                    selectedType = JsonConvert.DeserializeObject<Type>(builder.ToString());
                    builder.Clear();
                }
                else
                {
                    builder.Append(elem);
                }

            
            return (step, selectedType, JsonConvert.DeserializeObject<ManyParameters>(builder.ToString()));
        }

        public string ServerEncoder(ManyParameters Info)
        {
            var settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.Objects;
            return JsonConvert.SerializeObject(Info, settings);
        }
    }
}
