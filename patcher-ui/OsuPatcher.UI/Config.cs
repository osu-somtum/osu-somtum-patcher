using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace OsuPatcher.UI
{
    [DataContract]
    internal sealed class Config
    {
        public static readonly string Directory =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "osu-patcher");

        private static readonly string FilePath = Path.Combine(Directory, "config.json");

        [DataMember]
        public string OsuPath { get; set; }

        public static Config Load()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    var ser = new DataContractJsonSerializer(typeof(Config));
                    using (var fs = File.OpenRead(FilePath))
                        return (Config)ser.ReadObject(fs) ?? new Config();
                }
            }
            catch { }
            return new Config();
        }

        public void Save()
        {
            System.IO.Directory.CreateDirectory(Directory);
            var ser = new DataContractJsonSerializer(typeof(Config));
            using (var fs = File.Create(FilePath))
                ser.WriteObject(fs, this);
        }
    }
}
