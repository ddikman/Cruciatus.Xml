using System;
using System.Security;
using System.Windows.Automation;
using System.Xml;
using System.Xml.Linq;

namespace Cruciatus.Xml.CLI
{
    internal class XmlRetriever
    {
        private readonly int _processId;

        public XmlRetriever(int processId)
        {
            _processId = processId;
        }

        public string Parse()
        {
            var rootElement = AutomationHelper.GetApplicationRoot(_processId);
            if(rootElement == null)
                throw new Exception("Failed to find process with process id: " + _processId);

            var root = CreateElementRecursive(rootElement);
            var doc = new XDocument(root);
            return doc.ToString(SaveOptions.None);
        }

        private XElement CreateElementRecursive(AutomationElement rootElement)
        {
            var xpathElement = new ElementItem(rootElement);
            return CreateXElement(xpathElement);
        }

        private XElement CreateXElement(ElementItem xpathElement)
        {
            var element = new XElement(xpathElement.Name);

            foreach (var property in xpathElement.GetProperties())
            {
                var value = property.Value;
                value = value.Replace((char)0x1B, ' ');
                element.Add(new XAttribute(property.Name, value));
            }

            foreach (var child in xpathElement.GetElements())
                element.Add(CreateXElement(child));

            return element;
        }
    }
}