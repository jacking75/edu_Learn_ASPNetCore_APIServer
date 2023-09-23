using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace APIServer.MasterData;

public class Artifacts
{
    public Dictionary<int, Entity> Datas { get; private set; } = new();

    private readonly string FileName = "Artifacts.json";

    public void Load(string filePath)
    {
        var stream = File.ReadAllText(filePath + FileName);

        using (var document = JsonDocument.Parse(stream))
        {
            var root = document.RootElement;
            var elementCount = root.GetArrayLength();

            for (var i = 0; i < elementCount; i++)
            {
                var element = root[i];

                var data = new Entity()
                {
                    Id = element.GetProperty("00").GetInt32(),
                    Function = element.GetProperty("06").GetInt32(),
                    IsAvailableInTomb = element.GetProperty("07").GetInt32(),
                };

                Datas[data.Id] = data;
            }
        }
    }

    public class Entity
    {
        public int Id { get; set; }

        public int Function { get; set; }

        public int IsAvailableInTomb { get; set; }
    }
}