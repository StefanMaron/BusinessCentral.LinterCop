#nullable disable // TODO: Enable nullable and review rule
using Newtonsoft.Json;

namespace BusinessCentral.LinterCop.Helpers
{
    public class LinterSettings
    {
        private const string settingsFileName = "LinterCop.json";
        public int cyclomaticComplexityThreshold = 8;
        public int maintainabilityIndexThreshold = 20;
        public int cognitiveComplexityThreshold = 15;
        public string[] languagesToTranslate = null;
        public bool enableRule0011ForTableFields = false;
        public bool enableRule0016ForApiObjects = false;
        public string WorkingDir = "";
        public ProcedureNamePattern procedureNamePattern = new();
        public VariableAndParameterNamePattern variableAndParameterNamePattern = new();
        public CaptionPattern captionNamePattern = new();
        public FieldPattern fieldNamePattern = new();
        public GroupNamePattern groupNamePattern = new();
        public ActionNamePattern actionNamePattern = new();
        static public LinterSettings instance;

        static public void Create(string WorkingDir)
        {
            if (instance is null || instance.WorkingDir != WorkingDir)
            {
                instance = new LinterSettings();

                if (string.IsNullOrEmpty(WorkingDir))
                    return;

                string settingsPath = Path.Combine(WorkingDir, settingsFileName);

                // If the settings file is not found in the working directory, look in the location of the LinterCop file itself
                if (!File.Exists(settingsPath))
                    settingsPath = Path.Combine(Path.GetDirectoryName(typeof(LinterSettings).Assembly.Location), settingsFileName);

                if (File.Exists(settingsPath))
                {
                    using StreamReader reader = File.OpenText(settingsPath);
                    string json = reader.ReadToEnd();

                    InternalLinterSettings internalInstance = JsonConvert.DeserializeObject<InternalLinterSettings>(json);

                    instance.cyclomaticComplexityThreshold = internalInstance.cyclomaticComplexityThreshold ?? instance.cyclomaticComplexityThreshold;
                    instance.maintainabilityIndexThreshold = internalInstance.maintainabilityIndexThreshold ?? instance.maintainabilityIndexThreshold;
                    instance.cognitiveComplexityThreshold = internalInstance.cognitiveComplexityThreshold ?? instance.cognitiveComplexityThreshold;
                    instance.languagesToTranslate = internalInstance.languagesToTranslate ?? instance.languagesToTranslate;
                    instance.enableRule0011ForTableFields = internalInstance.enableRule0011ForTableFields;
                    instance.enableRule0016ForApiObjects = internalInstance.enableRule0016ForApiObjects;

                    if (internalInstance.procedureNamePattern != null)
                        instance.procedureNamePattern = internalInstance.procedureNamePattern;
                    if (internalInstance.variableAndParameterNamePattern != null)
                        instance.variableAndParameterNamePattern = internalInstance.variableAndParameterNamePattern;
                    if (internalInstance.captionNamePattern != null)
                        instance.captionNamePattern = internalInstance.captionNamePattern;
                    if (internalInstance.fieldNamePattern != null)
                        instance.fieldNamePattern = internalInstance.fieldNamePattern;
                    if (internalInstance.groupNamePattern != null)
                        instance.groupNamePattern = internalInstance.groupNamePattern;
                    if (internalInstance.actionNamePattern != null)
                        instance.actionNamePattern = internalInstance.actionNamePattern;
                }

                instance.WorkingDir = WorkingDir;
            }
        }

        public class InternalLinterSettings
        {
            public int? cyclomaticComplexityThreshold;
            public int? maintainabilityIndexThreshold;
            public int? cognitiveComplexityThreshold;
            public string[] languagesToTranslate;
            public bool enableRule0011ForTableFields = false;
            public bool enableRule0016ForApiObjects = false;

            [JsonProperty("procedure.name")]
            public ProcedureNamePattern procedureNamePattern;

            [JsonProperty("variable.name")]
            public VariableAndParameterNamePattern variableAndParameterNamePattern = new();

            [JsonProperty("caption.name")]
            public CaptionPattern captionNamePattern = new();

            [JsonProperty("field.name")]
            public FieldPattern fieldNamePattern = new();

            [JsonProperty("group.name")]
            public GroupNamePattern groupNamePattern= new();
            
            [JsonProperty("action.name")]
            public ActionNamePattern actionNamePattern = new();
        }

        public class ProcedureNamePattern
        {
            [JsonProperty("allow.pattern")]
            public string AllowPattern = "";

            [JsonProperty("disallow.pattern")]
            public string DisallowPattern = "";

            [JsonProperty("global.procedure.allow.pattern")]
            public string GlobalProcedureAllowPattern = "";
            [JsonProperty("global.procedure.disallow.pattern")]
            public string GlobalProcedureDisallowPattern = "";

            [JsonProperty("local.procedure.allow.pattern")]
            public string LocalProcedureAllowPattern = "";
            [JsonProperty("local.procedure.disallow.pattern")]
            public string LocalProcedureDisallowPattern = "";


            [JsonProperty("event.subscriber.allow.pattern")]
            public string EventSubscriberAllowPattern = "";
            [JsonProperty("event.subscriber.disallow.pattern")]
            public string EventSubscriberDisallowPattern = "";

            [JsonProperty("event.declaration.allow.pattern")]
            public string EventDeclarationAllowPattern = "";
            [JsonProperty("event.declaration.disallow.pattern")]
            public string EventDeclarationDisallowPattern = "";
        }

        public class VariableAndParameterNamePattern
        {
            [JsonProperty("allow.pattern")]
            public string AllowPattern = "";

            [JsonProperty("disallow.pattern")]
            public string DisallowPattern = "";
        }

        public class CaptionPattern
        {
            [JsonProperty("allow.pattern")]
            public string AllowPattern = "";

            [JsonProperty("disallow.pattern")]
            public string DisallowPattern = "";
        }

        public class FieldPattern
        {
            [JsonProperty("allow.pattern")]
            public string AllowPattern = "";

            [JsonProperty("disallow.pattern")]
            public string DisallowPattern = "";
        }

        public class GroupNamePattern
        {
            [JsonProperty("allow.pattern")]
            public string AllowPattern = "";

            [JsonProperty("disallow.pattern")]
            public string DisallowPattern = "";
        }
        
        public class ActionNamePattern
        {
            [JsonProperty("allow.pattern")]
            public string AllowPattern = "";

            [JsonProperty("disallow.pattern")]
            public string DisallowPattern = "";
        }
    }
}