using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;

namespace Blogs.LocalisingJavascriptResourceFiles.UI.Helpers
{
    public class JavascriptResourceFileContentCulturiser
    {
        #region Static Members

        private static string _nameSpace;
        private static Assembly _assembly;
        private static readonly Regex PlaceHolderRegex = new Regex("{.+?}");
        private static readonly char[] LineSeparators = { '\n', '\r' };
        private const string ResourcesFolderName = "Resources";
        private const char PlaceHolderStartChracter = '{';
        private const char PlaceHolderEndChracter = '}';
        private const char NamespaceDelimiter = '.';

        static JavascriptResourceFileContentCulturiser()
        {
            SetAssembly();
            BuildNameSpace();
        }

        private static void SetAssembly()
        {
            _assembly = Assembly.GetExecutingAssembly();
        }

        private static void BuildNameSpace()
        {
            _nameSpace = _assembly.GetName().Name;
        }

        #endregion

        #region Instance members

        private readonly string _content;
        private readonly string _controllerName;
        private readonly string _viewName;
        public string Output { get; private set; }

        public JavascriptResourceFileContentCulturiser(string content, string controllerName, string viewName)
        {
            if (string.IsNullOrWhiteSpace(content)) throw new ArgumentNullException("content");
            if (string.IsNullOrWhiteSpace(controllerName)) throw new ArgumentNullException("controllerName");
            if (string.IsNullOrWhiteSpace(viewName)) throw new ArgumentNullException("viewName");

            _content = content;
            _controllerName = controllerName;
            _viewName = viewName;
        }

        public JavascriptResourceFileContentCulturiser Culturise()
        {
            var type = GetTypeForFullyQualifiedName();
            if (type == null) throw new InvalidOperationException(string.Concat(_viewName, " resource not found "));

            var resourceManager = new ResourceManager(type);
            var scriptBuilder = new StringBuilder();

            string[] originalContentAsLines = _content.Split(LineSeparators, StringSplitOptions.RemoveEmptyEntries);
            foreach (var originalContentLine in originalContentAsLines)
            {
                if (string.IsNullOrWhiteSpace(originalContentLine)) continue; // Should not actuall happen due to string split options.

                var matches = PlaceHolderRegex.Matches(originalContentLine);
                var haveMatches = matches.Count > 0;
                string finalScriptLine = originalContentLine;

                if (haveMatches)
                {
                    string replacementValue;
                    var firstMatch = matches[0].ToString();
                    var haveReplacableValue = TryGetReplacementValue(firstMatch, resourceManager, out replacementValue);
                    if (haveReplacableValue)
                    {
                        finalScriptLine = originalContentLine.Replace(firstMatch, replacementValue);
                    }
                }

                scriptBuilder.AppendLine(finalScriptLine);
            }

            Output = scriptBuilder.ToString();
            return this;
        }

        private Type GetTypeForFullyQualifiedName()
        {
            var fullyQualifiedName = string.Concat(_nameSpace, NamespaceDelimiter, ResourcesFolderName,
                NamespaceDelimiter, _controllerName, NamespaceDelimiter, _viewName);
            var type = _assembly.GetType(fullyQualifiedName);

            return type;
        }

        private static bool TryGetReplacementValue(string initialValue,
            ResourceManager resourceManager, out string replacementValue)
        {
            var placeHolderValue = initialValue.TrimStart(PlaceHolderStartChracter).TrimEnd(PlaceHolderEndChracter);
            replacementValue = resourceManager.GetString(placeHolderValue, CultureInfo.CurrentCulture);

            var haveReplacableValue = !string.IsNullOrEmpty(replacementValue);
            return haveReplacableValue;
        }

        #endregion
    }
}