using CSharpFunctionalExtensions;
using GamelistMerger.Services.IO;
using GamelistMerger.Workflows;

namespace GamelistMerger;

public class Program
{
    public static async Task<int> Main(string[] args)
    {

        ConsoleOutput.ShowLogo(typeof(Program).Assembly.GetName().Version);
        var exeName = Path.GetFileName(Environment.GetCommandLineArgs()[0]);
        var result = await GamelistMergeWorkflow.ExecuteAsync(args);

        return result.Match(
            onSuccess: success =>
            {
                ConsoleOutput.ShowSuccess(success.ValidationA, success.ValidationB, success.OutputFile);
                ConsoleOutput.ShowFilterSummary(success.FilterStats, success.Duration, success.Verbose);

                return 0;
            },
            onFailure: errors =>
            {
                if (errors.Count == 1 && errors[0].ToString() == AppConfigParser.ShowUsageIndicator)
                {
                    ConsoleOutput.ShowUsage(exeName);
                    return 0;
                }

                ConsoleOutput.ShowErrors(errors);
                return 1;
            });
    }
}