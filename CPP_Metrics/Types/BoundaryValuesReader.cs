using System.Text.Json;

namespace CPP_Metrics.Types
{
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
}
