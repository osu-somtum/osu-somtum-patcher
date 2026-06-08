using System;
using System.IO;
using OsuPatcher.Runtime.Utils;

namespace OsuPatcher.Runtime
{
    internal class Config : BaseConfig
    {
        private const string ConfigFileName = "config.ini";
        private static readonly string ConfigPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "osuPatcher");

        public delegate void ConfigChangedHandler();
        public event ConfigChangedHandler OnConfigChanged;

        public bool PatchRelax { get; set; } = true;
        public bool TransitionTime { get; set; } = true;
        public bool PerformanceCalculator { get; set; } = true;

        internal static Config _load()
        {
            Directory.CreateDirectory(ConfigPath);
            string fullPath = Path.Combine(ConfigPath, ConfigFileName);
            Config config = new Config();

            if (File.Exists(fullPath))
                using (var reader = new StreamReader(fullPath))
                    config._loadConfig(reader);
            else
                config._save();

            return config;
        }

        private void _save()
        {
            string fullPath = Path.Combine(ConfigPath, ConfigFileName);
            using (var writer = new StreamWriter(fullPath, false))
                _saveConfig(writer);
        }

        private void ToggleSetting(string propName)
        {
            // HACK: FLIP the value of the auto property directly
            var prop = GetType()
                .GetProperty(propName);
            
            if (prop == null) return;

            var curr = (bool)prop.GetValue(this);
            prop.SetValue(this, !curr);

            _save();
            
            OnConfigChanged?.Invoke();
        }

        public void TogglePatchRelax(object sender, EventArgs e) => ToggleSetting(nameof(PatchRelax));
        public void ToggleTransitionTime(object sender, EventArgs e) => ToggleSetting(nameof(TransitionTime));
        public void TogglePerformanceCalculator(object sender, EventArgs e) => ToggleSetting(nameof(PerformanceCalculator));
    }
}
