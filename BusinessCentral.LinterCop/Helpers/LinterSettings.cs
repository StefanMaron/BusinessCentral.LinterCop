using Newtonsoft.Json;

namespace BusinessCentral.LinterCop.Helpers
{
    class LinterSettings
    {
        public int cyclomaticComplexityThreshold = 8;
        public int maintainabilityIndexThreshold = 20;
        public bool enableRule0011ForTableFields = false;
        public bool enableRule0016ForApiObjects = false;
        public string WorkingDir = "";
        static public LinterSettings instance;

        static public void Create(string WorkingDir)
        {
            if (instance == null || instance.WorkingDir != WorkingDir)
            {
                try
                {
                    StreamReader r = File.OpenText(Path.Combine(WorkingDir, "LinterCop.json"));
                    string json = r.ReadToEnd();
                    r.Close();
                    instance = new LinterSettings();

                    InternalLinterSettings internalInstance = JsonConvert.DeserializeObject<InternalLinterSettings>(json);
                    instance.cyclomaticComplexityThreshold = internalInstance.cyclomaticComplexityThreshold ?? internalInstance.cyclomaticComplexetyThreshold ?? instance.cyclomaticComplexityThreshold;
                    instance.maintainabilityIndexThreshold = internalInstance.maintainabilityIndexThreshold ?? internalInstance.maintainablityIndexThreshold ?? instance.maintainabilityIndexThreshold;
                    instance.enableRule0011ForTableFields = internalInstance.enableRule0011ForTableFields;
                    instance.enableRule0016ForApiObjects = internalInstance.enableRule0016ForApiObjects;
                    instance.WorkingDir = WorkingDir;
                }
                catch
                {
                    instance = new LinterSettings();
                }
            }
        }
    }
    internal class InternalLinterSettings
    {
        public int? cyclomaticComplexityThreshold;
        public int? maintainabilityIndexThreshold;
        public int? cyclomaticComplexetyThreshold; // Misspelled, deprecated
        public int? maintainablityIndexThreshold; // Misspelled, deprecated
        public bool enableRule0011ForTableFields = false;
        public bool enableRule0016ForApiObjects = false;

    }
}
