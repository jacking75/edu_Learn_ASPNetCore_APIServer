using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace APIServer.MasterData;

public class CharacterAbilityByLevel
{
    public Dictionary<int, Entity> Datas { get; private set; } = new();

    private readonly string FileName = "CharacterAbilityByLevel.json";
    private readonly int AbilityCountByType = 4;

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
                var data = new Entity();

                data.Level = element.GetProperty("00").GetInt32();
                for (var j = 0; j < AbilityCountByType; j++)
                {
                    data.Abilities.Add(element.GetProperty((j * 3 + 1).ToString().PadLeft(2, '0')).GetDouble());
                    data.Abilities.Add(element.GetProperty((j * 3 + 2).ToString().PadLeft(2, '0')).GetDouble());
                    data.Abilities.Add(element.GetProperty((j * 3 + 3).ToString().PadLeft(2, '0')).GetDouble());
                }

                Datas[data.Level] = data;
            }
        }
    }

    public class Entity
    {
        public int Level { get; set; }

        public List<double> Abilities { get; set; } = new();
    }
}

