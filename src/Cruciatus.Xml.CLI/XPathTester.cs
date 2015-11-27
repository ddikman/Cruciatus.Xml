using System;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Windows.Automation;
using Winium.Cruciatus.Core;

namespace Cruciatus.Xml.CLI
{
    internal class XPathTester
    {
        private readonly int _processId;
        private readonly Type _finderType;

        public XPathTester(int processId)
        {
            _processId = processId;
            _finderType = typeof (ByXPath);
        }

        public string Find(string testXPath)
        {
            var rootElement = AutomationHelper.GetApplicationRoot(_processId);

            var constructor = _finderType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] {typeof(string)}, null);
            var finder = constructor.Invoke(new object[] { testXPath });
            var findMethod = finder.GetType().GetMethod("FindFirst", BindingFlags.NonPublic | BindingFlags.Instance);
            var result = (AutomationElement)findMethod.Invoke(finder, new object[] { rootElement, 15 });

            if (result == null)
                return "Not found.";

            StringBuilder sb = new StringBuilder();
            foreach (var property in result.GetSupportedProperties())
            {
                var value = result.GetCurrentPropertyValue(property);
                var name = property.ProgrammaticName.Replace("AutomationElementIdentifiers.", string.Empty);
                sb.AppendLine($"{name}: {value}");
            }

            return sb.ToString();
        }
    }
}