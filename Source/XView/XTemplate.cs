using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace XView
{
    /// <summary>
    /// Templating engine for parsing HTML.
    /// </summary>
    public class XTemplate : DynamicObject
    {
        private const string BlockBeginDelimiter = "<!--";
        private const string BlockBeginWord = "BEGIN:";
        private const string BlockEndDelimiter = "-->";
        private const string BlockEndWord = "END:";
        private const string CommentRegexPattern = @"(?<comment>\s?#.*?)?";
        private const string RootBlockName = "root";
        private const string VariableBeginDelimiter = "{";
        private const string VariableEndDelimiter = "}";

        private readonly string blockRegexPattern = @"\s*(?<word>" + BlockBeginWord + "|" + BlockEndWord + ")" + @"\s*(?<name>\w+)" +
                                                    CommentRegexPattern + @"\s*" + BlockEndDelimiter + "(?<content>.*)";

        private readonly Dictionary<string, string> blocks = new Dictionary<string, string>();
        private readonly Dictionary<string, string> parsedBlocks = new Dictionary<string, string>();
        private readonly Dictionary<string, List<string>> subBlocks = new Dictionary<string, List<string>>();

        private readonly string variableRegexPattern = "(?<vartag>" + VariableBeginDelimiter + @"(?<variable>[A-Za-z0-9\._]+?)" +
                                                       CommentRegexPattern + VariableEndDelimiter + ")";

        private readonly Dictionary<string, string> variables = new Dictionary<string, string>();

        /// <summary>
        /// Value filter. When assigned the <see cref="ValueFilter"/> will be invoked to filter each assigned variable value.
        /// </summary>
        public Func<string, string> ValueFilter;

        /// <summary>
        /// Loads a new <see cref="XTemplate"/> object with the given template's full resource name located in the same assembly that XView is in.
        /// </summary>
        /// <param name="fullyQualifiedResourceName">Fully qualified name of the embedded resource.</param>
        public static XTemplate LoadFromResource(string fullyQualifiedResourceName)
        {
            return LoadFromResource(fullyQualifiedResourceName, typeof(XTemplate).Assembly);
        }

        /// <summary>
        /// Instantiates a new <see cref="XTemplate"/> object with the given template's full resource name in the given <see cref="Assembly"/>.
        /// </summary>
        /// <param name="fullyQualifiedResourceName">Fully qualified name of the embedded resource.</param>
        /// <param name="assembly"><see cref="Assembly"/> containing embedded resource.</param>
        public static XTemplate LoadFromResource(string fullyQualifiedResourceName, Assembly assembly)
        {
            return new XTemplate(ReadFromResource(fullyQualifiedResourceName, assembly));
        }

        /// <summary>
        /// Reads content from the given embedded resource.
        /// </summary>
        /// <param name="fullyQualifiedResourceName">Fully qualified name of the embedded resource.</param>
        /// <returns>String represents content of the given embedded resource.</returns>
        public static string ReadFromResource(string fullyQualifiedResourceName)
        {
            return ReadFromResource(fullyQualifiedResourceName, typeof(XTemplate).Assembly);
        }

        /// <summary>
        /// Reads content from the given embedded resource in the given Assembly.
        /// </summary>
        /// <param name="fullyQualifiedResourceName">Fully qualified name of the embedded resource.</param>
        /// <param name="assembly">The Assembly containing the embedded resource.</param>
        /// <returns>String represents content of the given embedded resource.</returns>
        public static string ReadFromResource(string fullyQualifiedResourceName, Assembly assembly)
        {
            try
            {
                using (Stream stream = assembly.GetManifestResourceStream(fullyQualifiedResourceName))
                using (var streamReader = new StreamReader(stream))
                {
                    return streamReader.ReadToEnd();
                }
            }
            catch (FileNotFoundException ex)
            {
                throw new Exception(
                    string.Format(
                        "Could not find embedded resource {0} in assembly {1}.",
                        fullyQualifiedResourceName,
                        assembly.FullName),
                    ex);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    string.Format(
                        "Could not read embedded resource {0}. Cause: {1}",
                        fullyQualifiedResourceName,
                        ex.Message),
                    ex);
            }
        }

        /// <summary>
        /// Instantiates a new <see cref="XTemplate"/> object with the given template.
        /// </summary>
        /// <param name="template">Template string.</param>
        public XTemplate(string template)
        {
            this.LoadTemplate(template);
        }

        /// <summary>
        /// Specify whether or not to allow null value assignments.
        /// </summary>
        public bool AllowNullValues { get; set; }

        /// <summary>
        /// Gets or sets a boolean to specify whether or not to auto reset a parsed sub block.
        /// </summary>
        public bool AutoResetParsedSubBlocks { get; set; }

        /// <summary>
        /// Gets or sets a boolean to specify whether or not to auto cleanup unassigned variable tags.
        /// When set to false, then unassigned variable tags will be shown in the output.
        /// </summary>
        public bool AutoCleanupUnassignedVariables { get; set; }

        /// <summary>
        /// Assigns a list of variables from the given <see><cref>IEnumerable{KeyValuePair{string, string}}</cref></see>object.
        /// </summary>
        /// <param name="values"><see><cref>IEnumerable{KeyValuePair{string, string}}</cref></see>object.</param>
        public void Assign(IEnumerable<KeyValuePair<string, string>> values)
        {
            foreach (var pair in values)
            {
                this.Assign(pair.Key, pair.Value);
            }
        }

        /// <summary>
        /// Assigns a value to a variable.
        /// </summary>
        /// <param name="variableName">Variable name.</param>
        /// <param name="value">Object value.</param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public void Assign(string variableName, object value)
        {
            if (string.IsNullOrEmpty(variableName))
            {
                throw new InvalidOperationException("Cannot assign value to a variable that is empty or null");
            }

            if (value == null && !this.AllowNullValues)
            {
                throw new NullReferenceException(string.Format("Cannot assign null value to variable {0}", variableName));
            }

            this.Assign(variableName, value != null ? value.ToString() : string.Empty);
        }

        /// <summary>
        /// Assigns a value to a variable.
        /// </summary>
        /// <param name="variableName">Variable name.</param>
        /// <param name="value">String value.</param>
        private void Assign(string variableName, string value)
        {
            value = this.FilterValue(value);

            if (!this.variables.ContainsKey(variableName))
            {
                this.variables.Add(variableName, value);
            }
            else
            {
                this.variables[variableName] = value;
            }
        }

        /// <summary>
        /// Parses a block.
        /// </summary>
        /// <param name="blockName">Fully qualified block name.</param>
        public void Parse(string blockName)
        {
            if (!this.blocks.ContainsKey(blockName))
            {
                throw new Exception(string.Format("Block {0} does not exist", blockName));
            }

            var blockOutput = this.blocks[blockName];
            var match = Regex.Match(blockOutput, this.variableRegexPattern, RegexOptions.Singleline | RegexOptions.Compiled);

            while (match.Success)
            {
                var variableTag = match.Groups["vartag"].ToString();
                var variableName = match.Groups["variable"].ToString();
                var variableNameParts = new List<string>();
                variableNameParts.AddRange(variableName.Split('.'));
                string variableValue = null;

                if (variableNameParts[0].Equals("_BLOCK_"))
                {
                    variableNameParts.RemoveAt(0);
                    var blockVariableName = string.Join(".", variableNameParts.ToArray());

                    if (this.parsedBlocks.ContainsKey(blockVariableName))
                    {
                        variableValue = this.parsedBlocks[blockVariableName];
                    }

                    if (string.IsNullOrEmpty(variableValue))
                    {
                        blockOutput = blockOutput.Replace(variableTag, string.Empty);
                    }
                    else
                    {
                        blockOutput = blockOutput.TrimStart(new[] { '\r', '\n' });
                        variableValue = variableValue.Trim();
                        blockOutput = blockOutput.Replace(variableTag, variableValue);
                    }
                }
                else
                {
                    variableValue = this.variables.ContainsKey(variableName) ? this.variables[variableName] : null;

                    if (variableValue != null)
                    {
                        blockOutput = blockOutput.Replace(variableTag, variableValue);
                    }
                    else
                    {
                        if (this.AutoCleanupUnassignedVariables)
                        {
                            blockOutput = blockOutput.Replace(variableTag, string.Empty);
                        }
                    }
                }

                match = match.NextMatch();
            }

            if (this.parsedBlocks.ContainsKey(blockName))
            {
                this.parsedBlocks[blockName] += blockOutput;
            }
            else
            {
                this.parsedBlocks.Add(blockName, blockOutput);
            }

            if (!this.AutoResetParsedSubBlocks || !this.subBlocks.ContainsKey(blockName))
            {
                return;
            }

            foreach (var subBlockName in this.subBlocks[blockName])
            {
                this.ResetParsedBlock(subBlockName);
            }
        }

        /// <summary>
        /// Assigns the given variable and value, and parses the given block.
        /// </summary>
        /// <param name="variableName">Variable name.</param>
        /// <param name="variableValue">Value.</param>
        /// <param name="blockName">Block name to parse.</param>
        public void AssignParse(string variableName, string variableValue, string blockName)
        {
            this.Assign(variableName, variableValue);
            this.Parse(blockName);
        }

        /// <summary>
        /// Resets a parsed block.
        /// </summary>
        /// <param name="blockName">Name of a parsed block.</param>
        public void ResetParsedBlock(string blockName = RootBlockName)
        {
            if (this.parsedBlocks.ContainsKey(blockName))
            {
                this.parsedBlocks.Remove(blockName);
            }
        }

        /// <summary>
        /// Parses and outputs root block.
        /// </summary>
        /// <returns>String output.</returns>
        public override string ToString()
        {
            return this.ToString(RootBlockName);
        }

        /// <summary>
        /// Parses and outputs the given block.
        /// </summary>
        /// <param name="blockName">Fully qualified block name.</param>
        /// <returns>Parsed string of the given block.</returns>
        public string ToString(string blockName)
        {
            if (string.IsNullOrEmpty(blockName))
            {
                throw new NullReferenceException("Cannot output a null or empty block");
            }

            if (!this.parsedBlocks.ContainsKey(blockName))
            {
                this.Parse(blockName);
            }

            return this.parsedBlocks[blockName];
        }

        /// <summary>
        /// Adds root block (the most outter block) to template, if root block is omitted.
        /// </summary>
        private static string AddRootBlock(string template)
        {
            var rootBlockRegex = string.Concat(
                BlockBeginDelimiter,
                @"\s*",
                BlockBeginWord,
                @"\s*",
                RootBlockName,
                CommentRegexPattern,
                BlockEndDelimiter);

            if (!Regex.Match(template, rootBlockRegex, RegexOptions.IgnoreCase | RegexOptions.Compiled).Success)
            {
                var prefix = string.Concat(BlockBeginDelimiter, " ", BlockBeginWord, " ", RootBlockName, " ", BlockEndDelimiter);
                var postfix = string.Concat(BlockBeginDelimiter, " ", BlockEndWord, " ", RootBlockName, " ", BlockEndDelimiter);
                template = prefix + template + postfix;
            }

            return template;
        }

        private string FilterValue(string value)
        {
            return this.ValueFilter != null ? this.ValueFilter(value) : value;
        }

        private void LoadTemplate(string template)
        {
            this.AutoResetParsedSubBlocks = true;
            this.AllowNullValues = true;
            this.AutoCleanupUnassignedVariables = true;
            this.BuildParsingStructure(template);
        }

        private void BuildParsingStructure(string template)
        {
            template = AddRootBlock(template);
            string[] templateParts = template.Split(new string[] { BlockBeginDelimiter }, StringSplitOptions.None);
            var blockNames = new List<string>();
            var blockDepth = 0;

            for (var i = 0; i < templateParts.Length; i++)
            {
                string templatePart = templateParts[i];
                var match = Regex.Match(
                    templatePart,
                    this.blockRegexPattern,
                    RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

                if (match.Success)
                {
                    while (match.Success)
                    {
                        var blockWord = match.Groups["word"].ToString();
                        var blockName = match.Groups["name"].ToString();
                        var blockContent = match.Groups["content"].ToString();
                        string tempParentBlockName;

                        if (blockWord.ToUpper().Equals(BlockBeginWord))
                        {
                            blockDepth++;
                            tempParentBlockName = string.Join(".", blockNames.ToArray());
                            blockNames.Add(blockName);
                            var currentBlockName = string.Join(".", blockNames.ToArray());
                            this.subBlocks.Add(currentBlockName, new List<string>());

                            if (this.blocks.ContainsKey(currentBlockName))
                            {
                                this.blocks[currentBlockName] += blockContent;
                            }
                            else
                            {
                                this.blocks.Add(currentBlockName, blockContent);
                            }

                            this.blocks[tempParentBlockName] += VariableBeginDelimiter + "_BLOCK_." + currentBlockName
                                                                + VariableEndDelimiter;

                            if (this.subBlocks.ContainsKey(tempParentBlockName))
                            {
                                this.subBlocks[tempParentBlockName].Add(currentBlockName);
                            }
                        }
                        else if (blockWord.ToUpper().Equals(BlockEndWord))
                        {
                            if (!blockNames.Contains(blockName))
                            {
                                throw new Exception(
                                    string.Format("Block {0} does not have a begin delimiter", blockName));
                            }

                            blockDepth--;
                            blockNames.RemoveAt(blockDepth);
                            tempParentBlockName = string.Join(".", blockNames.ToArray());
                            this.blocks[tempParentBlockName] += blockContent;
                        }

                        match = match.NextMatch();
                    }
                }
                else
                {
                    var tempBlockName = string.Join(".", blockNames.ToArray());

                    if (i > 0)
                    {
                        this.blocks[tempBlockName] += BlockBeginDelimiter;
                    }

                    if (this.blocks.ContainsKey(tempBlockName))
                    {
                        this.blocks[tempBlockName] += templatePart;
                    }
                    else
                    {
                        this.blocks.Add(tempBlockName, templatePart);
                    }
                }
            }
        }

        #region DynamicObject Members

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            this.Assign(binder.Name, value);

            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var variableName = binder.Name;

            if (!this.variables.ContainsKey(variableName))
            {
                throw new Exception(string.Format("Variable {0} does not assist", variableName));
            }

            result = this.variables[variableName];

            return true;
        }

        #endregion DynamicObject Members
    }
}