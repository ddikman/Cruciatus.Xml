using System.Windows.Automation;

namespace Cruciatus.Xml.CLI
{
    internal static class AutomationHelper
    {
        public static AutomationElement GetApplicationRoot(int processId)
        {
            var processIdCondition = new PropertyCondition(AutomationElement.ProcessIdProperty, processId);
            return AutomationElement.RootElement.FindFirst(TreeScope.Children, processIdCondition);
        }
    }
}
