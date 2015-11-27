using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Automation;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Winium.Cruciatus.Exceptions;

namespace Cruciatus.Xml.CLI
{
    internal class ElementItem : XPathItem
    {
        #region Fields

        private readonly AutomationElement _element;

        private readonly TreeWalker _treeWalker = TreeWalker.ControlViewWalker;

        private List<AutomationProperty> _properties;

        #endregion

        #region Constructors and Destructors

        internal ElementItem(AutomationElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            _element = element;
        }

        #endregion

        #region Properties

        internal override bool IsEmptyElement
        {
            get
            {
                var hasChild = this.MoveToFirstChild() != null;
                if (hasChild)
                {
                    return false;
                }

                try
                {
                    //new CruciatusElement(null, this._element, null).Text();
                    return false;
                }
                catch (CruciatusException)
                {
                    return true;
                }
            }
        }

        internal override string Name
        {
            get
            {
                // This is a bit stupid because most names are not valid xml
                try
                {
                    return XmlConvert.VerifyNCName(_element.Current.Name);
                }
                catch (ArgumentNullException)
                {
                    Console.Error.WriteLine("Missing tag for: " + _element.Current.ClassName);
                    return "NoTag";
                }
                catch (XmlException)
                {
                    Console.Error.WriteLine("Found invalid tag name for: " + _element.Current.Name);
                    return "UnavailableTag";
                }
            }
        }

        internal override XPathNodeType NodeType
        {
            get
            {
                return XPathNodeType.Element;
            }
        }

        internal List<AutomationProperty> SupportedProperties
        {
            get
            {
                return this._properties ?? (this._properties = this._element.GetSupportedProperties().ToList());
            }
        }

        #endregion

        #region Public Methods and Operators

        public override object TypedValue()
        {
            return this._element;
        }

        #endregion

        #region Methods

        internal IEnumerable<PropertyItem> GetProperties()
        {
            return SupportedProperties.Select(p => new PropertyItem(this, p));
        }

        internal IEnumerable<ElementItem> GetElements()
        {
            foreach(AutomationElement e in _element.FindAll(TreeScope.Children, Condition.TrueCondition))
                yield return new ElementItem(e);
        }

        internal static XPathItem Create(AutomationElement instance)
        {
            return instance.Equals(AutomationElement.RootElement) ? null : new ElementItem(instance);
        }

        internal AutomationProperty GetNextPropertyOrNull(AutomationProperty property)
        {
            var index = this.SupportedProperties.IndexOf(property);
            return this.SupportedProperties.ElementAtOrDefault(index + 1);
        }

        internal object GetPropertyValue(AutomationProperty property)
        {
            try
            {
                return this._element.GetCurrentPropertyValue(property);
            }
            catch (InvalidOperationException)
            {
                Console.Error.WriteLine("Failed to get property: " + property.Id);
                return string.Empty;
            }
        }

        internal override bool IsSamePosition(XPathItem item)
        {
            var obj = item as ElementItem;
            return obj != null && obj._element.Equals(this._element);
        }

        internal override XPathItem MoveToFirstChild()
        {
            var firstChild = this._treeWalker.GetFirstChild(this._element);
            return (firstChild == null) ? null : Create(firstChild);
        }

        internal override XPathItem MoveToFirstProperty()
        {
            return this.SupportedProperties.Any() ? new PropertyItem(this, this.SupportedProperties[0]) : null;
        }

        internal override XPathItem MoveToNext()
        {
            var next = this._treeWalker.GetNextSibling(this._element);
            return (next == null) ? null : Create(next);
        }

        internal override XPathItem MoveToNextProperty()
        {
            return null;
        }

        internal override XPathItem MoveToParent()
        {
            var parent = this._treeWalker.GetParent(this._element);
            return (parent == null) ? null : Create(parent);
        }

        internal override XPathItem MoveToPrevious()
        {
            var previous = this._treeWalker.GetPreviousSibling(this._element);
            return (previous == null) ? null : Create(previous);
        }

        #endregion
    }
}
