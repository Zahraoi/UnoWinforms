using Uno.Core.Services;
using Uno.WinForms.Forms;
using Uno.WinForms.Services;

namespace Uno.WinForms;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        var persistenceService = new GamePersistenceService();
        persistenceService.InitializeAsync().GetAwaiter().GetResult();

        var ruleEngine = new RuleEngine();
        var computerPlayerService = new ComputerPlayerService();

        Application.Run(new StartupForm(ruleEngine, computerPlayerService, persistenceService));
    }
}
