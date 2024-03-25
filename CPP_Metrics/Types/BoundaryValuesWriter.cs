using System.Text;
using System.Text.Json;

namespace CPP_Metrics.Types
{
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
