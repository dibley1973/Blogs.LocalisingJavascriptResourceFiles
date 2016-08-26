using Dibware.Helpers.Validation;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace NewLook.QuestionnaireBuilder.Client.WebApplication.Helpers
{
    public class JavascriptResourceFileBuilder
    {
        #region Static Members

        private static string _nameSpace;
        private static Assembly _assembly;

        static JavascriptResourceFileBuilder()
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

        private readonly string _controllerName;
        private readonly string _resourceName;

        public string Output { get; private set; }

        public JavascriptResourceFileBuilder(string controllerName, string resourceName)
        {
            Guard.ArgumentIsNullOrWhiteSpace(controllerName, "controllerName");
            Guard.ArgumentIsNullOrWhiteSpace(resourceName, "resourceName");

            _controllerName = controllerName;
            _resourceName = resourceName;
        }

        public JavascriptResourceFileBuilder Build()
        {
            string javasriptResourceFilePath = BuildJavasriptResourceFilePathForController();

            var fileNotFound = !System.IO.File.Exists(javasriptResourceFilePath);
            if (fileNotFound) throw new InvalidOperationException("The resource requested was not found on the server. ");

            Output = BuildJavascript(javasriptResourceFilePath, _resourceName);

            return this;
        }

        private string BuildJavasriptResourceFilePathForController()
        {
            var scriptFileName = String.Format(@"\Scripts\{0}\questionnaireBuilder.{1}.resource.js", _controllerName,
                _resourceName.ToLower());
            string javasriptResourceFilePath = BuildJavasriptResourceFilePath(scriptFileName);
            return javasriptResourceFilePath;
        }

        private string BuildJavasriptResourceFilePath(string scriptFileName)
        {
            var javasriptResourceFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, scriptFileName);
            var javasriptResourceFilePath = HttpContext.Current.Server.MapPath(javasriptResourceFileName);
            return javasriptResourceFilePath;
        }

        private static string BuildJavascript(string javasriptResourceFilePath, string resourceName)
        {
            var fullyQualifiedName = string.Concat(_nameSpace, ".Resources.", resourceName);
            var type = _assembly.GetType(fullyQualifiedName);
            if (type == null) throw new InvalidOperationException(string.Concat(resourceName, " resource not found "));

            var resourceManager = new ResourceManager(type);
            var scriptBuilder = new StringBuilder();

            using (var fileReadStream = new FileStream(javasriptResourceFilePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite)
                )
            {
                var regex = new Regex("{.*?}");
                using (StreamReader sr = new StreamReader(fileReadStream))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (line == null) continue;

                        var matches = regex.Matches(line);
                        var haveMatches = matches.Count > 0;
                        if (haveMatches)
                        {
                            var firstMatch = matches[0].ToString();
                            string replacementValue, placeHolderValue;
                            bool haveReplacableValue = TryGetReplacementValue(firstMatch, resourceManager, out replacementValue, out placeHolderValue);
                            if (haveReplacableValue)
                            {
                                line = line.Replace(firstMatch, replacementValue);
                            }
                        }

                        scriptBuilder.AppendLine(line);
                    }
                }
            }

            var script = scriptBuilder.ToString();
            return script;
        }

        private static bool TryGetReplacementValue(string firstMatch, ResourceManager resourceManager,
            out string replacementValue, out string placeHolderValue)
        {
            placeHolderValue = firstMatch.TrimStart('{').TrimEnd('}');
            replacementValue = resourceManager.GetString(placeHolderValue, CultureInfo.CurrentCulture);
            var haveReplacableValue = !string.IsNullOrEmpty(replacementValue);
            return haveReplacableValue;
        }
    }
}