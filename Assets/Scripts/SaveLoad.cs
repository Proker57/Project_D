using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using BOYAREngine.Controller;
using UnityEngine;

namespace BOYAREngine
{
    public class SaveLoad
    {
        private static readonly string Destination = $"{Application.persistentDataPath}\\save.sosi";
        public static void Save(SaveData saveData)
        {
            
            var file = File.Exists(Destination) ? File.OpenWrite(Destination) : File.Create(Destination);
            var bf = new BinaryFormatter();
            
            bf.Serialize(file, saveData);
            file.Close();
        }

        public static SaveData Load()
        {
            var file = File.Exists(Destination) ? File.OpenRead(Destination) : null;
            if (file == null) return null;

            var bf = new BinaryFormatter();
            var data = (SaveData) bf.Deserialize(file);
            return data;
        }
    }
}

