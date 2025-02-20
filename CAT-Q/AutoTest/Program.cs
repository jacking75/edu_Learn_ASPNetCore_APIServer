using ContentDLL.Scenario;

namespace AutoTest;


class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, CAT-Q: Test Ilzzin!");

        string parameters = @"{
                ""IsLogWriteConsole"": 1,    
                ""DummyCount"": 10,
                ""ServerUrl"" : ""http://localhost:11502"",
                ""HiveDBConnectionString"" : ""Server=localhost;Port=3306;user=root;Password=123qwe;Database=sample_hive_db;Pooling=true;Min Pool Size=0;Max Pool Size=100;AllowUserVariables=True;"",
            }"
        ;


        ScenarioRunner runner = new ScenarioRunner();
        runner.Init(parameters);
        runner.Run().Wait();
                        
        Console.WriteLine("End: Test Ilzzin");         
    }    
}
