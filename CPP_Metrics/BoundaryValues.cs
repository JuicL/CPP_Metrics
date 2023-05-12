using System.Text;
using System.Text.Json;

namespace CPP_Metrics
{
    public class BoundaryValues
    {
        public int Complexity { get; set; }
        public int DIT { get; set; }

    }

    public static class BoundaryValuesReader
    {
        public static BoundaryValues? ReadConfigFile(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                var str = sr.ReadToEnd();
                try
                {
                    BoundaryValues? boundaryValues = JsonSerializer.Deserialize<BoundaryValues>(str);
                    return boundaryValues;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
    }
    public static class BoundaryValuesWriter
    {
        public static void CreateConfigFile(string path)
        {
            FileStream fileFS = new FileStream(path, FileMode.Create);
            StreamWriter fileWriter = new StreamWriter(fileFS, Encoding.Default);

            BoundaryValues boundaryValues = new BoundaryValues();

            string str = JsonSerializer.Serialize(boundaryValues);
            fileWriter.Write(str);
            fileWriter.Close(); 
            fileFS.Close();
        }

        public static void Write()
        {

        }
    }
}
