using Genesis.Output.Poco;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Output.ReactEditorComponent
{
    public class ReactEditorComponentGenerator : OutputExecutor
    {
        public override string CommandText => "react-ec";
        public override string Description => "A validating object Editor component for React.js";
        public override string FriendlyName => "React Editor Component";

        public ReactEditorComponentConfig Config { get; set; } = new ReactEditorComponentConfig();

        protected override void OnInitialized()
        {
            Config = (ReactEditorComponentConfig)Configuration;

            try
            {
                if (!Directory.Exists(Config.OutputPath))
                    Directory.CreateDirectory(Config.OutputPath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message); //NOTE: Inputs don't have OutputPath properties
            }
        }

        public override async Task<IGenesisExecutionResult> Execute(GenesisContext genesis, string[] args)
        {
            Text.DarkGrayLine($@"Generating react editor components in: {Config.OutputPath}");

            foreach (var obj in genesis.Objects)
                await ExecuteGraph(obj);

            return new OutputGenesisExecutionResult();
        }

        public async Task ExecuteGraph(ObjectGraph objectGraph)
        {
            var entityName = objectGraph.Name.ToSingular();

            var output = Template.Raw 
                            .Replace(Tokens.ObjectName, entityName)
                            .Replace(Tokens.ObjectNameAsArgument, entityName.ToCorrectedCase())
                            .Replace(Tokens.PropertiesValidationStub, GetValidationBlock(objectGraph))
                            .Replace(Tokens.EditorRowsStub, GetEditorRows(objectGraph));

            var path = Path.Combine(Config.OutputPath, $@"Edit{entityName}.{Config.FileExtension.TrimStart('.')}");

            Text.White($"Wrote '"); Text.Yellow(path); Text.WhiteLine("'");

            File.WriteAllText(path, output);

            await Task.CompletedTask;
        }

        private static string GetEditorRows(ObjectGraph objectGraph)
        {
            const string textVal = // General text validator
@"    <Form.Group as={Col} lg={6} md={12}>
        <Form.Control
            type=""text""
            name=""~OBJECT_NAME_ARGUMENT~""
            value={values.~OBJECT_NAME_ARGUMENT~}
            placeholder=""~OBJECT_NAME_SPACED~""
            onChange={handleChange}
            isInvalid={touched.~OBJECT_NAME_ARGUMENT~ && !! errors.~OBJECT_NAME_ARGUMENT~}/>
        <Form.Control.Feedback type=""invalid"">
            {errors.~OBJECT_NAME_ARGUMENT~}
        </Form.Control.Feedback>
      </Form.Group>";

            var sb = new StringBuilder();

            foreach (var p in objectGraph.Properties)
            {
                if (p.SourceType.Equals("sysname", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                //TODO: Different editor types

                sb.AppendLine(textVal
                    .Replace(Tokens.ObjectNameSpaced, p.Name.ToSpaceSeperated())
                    .Replace(Tokens.ObjectNameAsArgument, p.Name.ToCorrectedCase()));
            }

            return sb.ToString().Trim().TrimEnd(',');
        }

        private static string GetValidationBlock(ObjectGraph objectGraph) //TODO: Child templates... This is ugly as hell and hard coded
        {
            const string textVal = // General text validator
@"  ~OBJECT_NAME_ARGUMENT~: yup.string().ensure().trim()
  .required('~OBJECT_NAME_SPACED~ is required')
  .max(255, '~OBJECT_NAME_SPACED~ cannot be greater than 255 chars'),
";
            const string dateVal = // Date validator
@"  ~OBJECT_NAME_ARGUMENT~: yup.date()
  .typeError('~OBJECT_NAME_SPACED~ is not a valid date')
  .required('~OBJECT_NAME_SPACED~ is required'),
";

            const string numVal = // Number validator
@"  ~OBJECT_NAME_ARGUMENT~: yup.string()
  .required('~OBJECT_NAME_SPACED~ is required'),
";

            var sb = new StringBuilder();

            foreach(var p in objectGraph.Properties)
            {
                if (p.SourceType.Equals("sysname", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                var pstr = p.TypeGuess.ToLower() switch
                {

                    "date" => dateVal,
                    "datetime" => dateVal,
                    "datetimeoffset" => dateVal,
                    "byte" => numVal,
                    "short" => numVal,
                    "int" => numVal,
                    "long" => numVal,
                    "decimal" => numVal,
                    "float" => numVal,
                    "double" => numVal,
                    _ => textVal
                };

                sb.AppendLine(pstr
                    .Replace(Tokens.ObjectNameSpaced, p.Name.ToSpaceSeperated())
                    .Replace(Tokens.ObjectNameAsArgument, p.Name.ToCorrectedCase()));
            }

            return sb.ToString().Trim().TrimEnd(',');
        }
    }
}