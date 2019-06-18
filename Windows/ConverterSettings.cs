using Reader;
using System;
using System.IO;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace Windows
{
    public class ConverterSettings
    {
        public string InDirectory { get; set; } = Shared.InDir;

        public string OutDirectory { get; set; } = Shared.OutDir;

        public bool IncludeIAT { get; set; } = true;

        public bool GroupSheets { get; set; } = true;

        public bool GroupSims { get; set; } = false;

        public bool IncludeNABSA { get; set; } = true;

        public ConverterSettings()
        { }

        public static ConverterSettings Read()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                + "/CLEMConverter/settings.json";

            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                ConverterSettings settings = (new JavaScriptSerializer()).Deserialize<ConverterSettings>(json);
                return settings;
            }

            return new ConverterSettings();
        }

        public void Write()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                + "/CLEMConverter";

            Directory.CreateDirectory(path);

            var json = (new JavaScriptSerializer()).Serialize(this);

            File.WriteAllText(path + "/settings.json", json);
        }
    }
}
