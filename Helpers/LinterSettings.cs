using System.IO;
using Newtonsoft.Json;

namespace BusinessCentral.LinterCop.Helpers
{
    class LinterSettings
    {
        public int cyclomaticComplexetyThreshold = 8;
        public int maintainablityIndexThreshold = 20;
        public bool enableRule0011ForTableFields = false;
        static public LinterSettings instance;

        static public void Create()
        {
            if (instance == null)
            {
                try
                {
                    StreamReader r = File.OpenText("LinterCop.json");
                    string json = r.ReadToEnd();
                    r.Close();
                    instance = JsonConvert.DeserializeObject<LinterSettings>(json);
                    if (instance == null)
                        instance = new LinterSettings();
                }
                catch
                {
                    instance = new LinterSettings();
                }
            }
        }
    }
}
