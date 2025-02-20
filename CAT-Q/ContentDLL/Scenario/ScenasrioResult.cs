using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentDLL.Scenario;

public class ScenasrioResult
{
    public string Name { get; set; }

    public bool IsCompleted { get; set; } = false;

    public bool IsSuccess { get; set; }

    public string Message { get; set; }

    public List<SceneResult> SceneResults { get; set; } = new List<SceneResult>();
}

public class SceneResult
{
    public string Name { get; set; }

    public bool IsCompleted { get; set; } = false;
        
    public ErrorCode ErrorCode { get; set; }

    public string Message { get; set; }
}
