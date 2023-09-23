using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace APIServer.MasterData;

public class ArrayExample
{
    public Dictionary<int, Entity> Datas { get; private set; } = new();

    private readonly string FileName = "ArrayExample.json";

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
                };

                var numbers = JsonSerializer.Deserialize<List<int>>(element.GetProperty("01").ToString());
                foreach (var number in numbers)
                {
                    data.Numbers.Add(number);
                }

                Datas[data.Id] = data;
            }
        }
    }

    public class Entity
    {
        public int Id { get; set; }

        public List<int> Numbers { get; set; } = new();
    }
}