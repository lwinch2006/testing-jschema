using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dka.Net5.TestingJSchema.Extensions;
using Dka.Net5.TestingJSchema.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using NJsonSchema.CodeGeneration.CSharp;

namespace Dka.Net5.TestingJSchema
{
    class Program
    {
        private const string OutputJSchemaDirectory = "OutputJSchemas";
        private const string OutputJObjectDirectory = "OutputJObjects";
        private const string OutputJObjectTranslatedDirectory = "OutputJObjectsTranslated";
        private const string OutputClassDirectory = "OutputClasses";

        private static readonly string OutputJSchemaFullPath = Path.Combine(Environment.CurrentDirectory, OutputJSchemaDirectory);
        private static readonly string OutputJObjectFullPath = Path.Combine(Environment.CurrentDirectory, OutputJObjectDirectory);
        private static readonly string OutputJObjectTranslatedFullPath = Path.Combine(Environment.CurrentDirectory, OutputJObjectTranslatedDirectory);
        private static readonly string OutputClassFullPath = Path.Combine(Environment.CurrentDirectory, OutputClassDirectory);
        
        private static async Task Main()
        {
            // Initializing directories.
            InitDirectories();
            
            // Generating JSchema.
            await GenerateJSchemasFromClasses();

            // Generating class.
            await GenerateClassesFromJSchemas();

            // Converting JSchemas to JObjects.
            await ConvertJSchemasToJObjects();
            
            // Replacing some values in enums in JObjects.
            await ReplaceValuesInEnums();

            // Exiting application.
            Console.WriteLine("Generation is done.");
        }

        private static void InitDirectories()
        {
            if (!Directory.Exists(OutputJSchemaFullPath))
            {
                Directory.CreateDirectory(OutputJSchemaFullPath);
            }
            
            if (!Directory.Exists(OutputJObjectFullPath))
            {
                Directory.CreateDirectory(OutputJObjectFullPath);
            }
            
            if (!Directory.Exists(OutputJObjectTranslatedFullPath))
            {
                Directory.CreateDirectory(OutputJObjectTranslatedFullPath);
            }
            
            if (!Directory.Exists(OutputClassFullPath))
            {
                Directory.CreateDirectory(OutputClassFullPath);
            }
        }
        
        private static async Task GenerateJSchemasFromClasses()
        {
            var generator = new JSchemaGenerator
            {
                SchemaIdGenerationHandling = SchemaIdGenerationHandling.TypeName, 
                SchemaReferenceHandling = SchemaReferenceHandling.None,
                
            };
            
            generator.GenerationProviders.Add(new StringEnumGenerationProvider());
            
            var schemaFileName = $"{nameof(Person).ToLower()}-jschema.txt";
            var schemaFullFileName = Path.Combine(OutputJSchemaFullPath, schemaFileName);
            var schema = generator.Generate(typeof(Person));
            await File.WriteAllTextAsync(schemaFullFileName, schema.ToString());
            
            schemaFileName = $"{nameof(FormV2_003).ToLower()}-jschema.txt";
            schemaFullFileName = Path.Combine(OutputJSchemaFullPath, schemaFileName);
            schema = generator.Generate(typeof(FormV2_003));
            await File.WriteAllTextAsync(schemaFullFileName, schema.ToString());
        }

        private static async Task GenerateClassesFromJSchemas()
        {
            var jschemaFileFullNames = Directory.EnumerateFiles(OutputJSchemaFullPath, "*-jschema.txt");

            foreach (var jschemaFileFullName in jschemaFileFullNames)
            {
                var schemaFromFile = await NJsonSchema.JsonSchema.FromFileAsync(jschemaFileFullName);
                var classGenerator = new CSharpGenerator(schemaFromFile, new CSharpGeneratorSettings
                {
                    ClassStyle = CSharpClassStyle.Poco
                });
            
                var classText = classGenerator.GenerateFile();                
                
                var jschemaFileName = Path.GetFileName(jschemaFileFullName);
                var classFileName = $"{jschemaFileName.Substring(0, jschemaFileName.IndexOf('-'))}-class.txt";
                await File.WriteAllTextAsync(Path.Combine(OutputClassFullPath, classFileName), classText);
            }
        }

        private static async Task ConvertJSchemasToJObjects()
        {
            var jSchemaFileFullNames = Directory.EnumerateFiles(OutputJSchemaFullPath, "*-jschema.txt");

            foreach (var jSchemaFileFullName in jSchemaFileFullNames)
            {
                var jSchemaAsString = await File.ReadAllTextAsync(jSchemaFileFullName);
                var jSchema = JSchema.Parse(jSchemaAsString);

                var jObject = JObject.Parse(jSchema.ToString());
                var jObjectAsString = jObject.ToString();
                
                var jschemaFileName = Path.GetFileName(jSchemaFileFullName);
                var jObjectFileName = $"{jschemaFileName.Substring(0, jschemaFileName.IndexOf('-'))}-jobject.txt";
                await File.WriteAllTextAsync(Path.Combine(OutputJObjectFullPath, jObjectFileName), jObjectAsString);
            }
        }

        private static async Task ReplaceValuesInEnums()
        {
            var jObjectFileFullNames = Directory.EnumerateFiles(OutputJObjectFullPath, "*-jobject.txt");

            foreach (var jObjectFileFullName in jObjectFileFullNames)
            {
                var jObjectAsString = await File.ReadAllTextAsync(jObjectFileFullName);
                var jObject = JObject.Parse(jObjectAsString);
                
                // Replacing "Yes" with "Ja".
                var foundJTokens = jObject.FindJTokensByPropertyValue("Yes");
                foreach (var foundJToken in foundJTokens)
                {
                    foundJToken.Replace(new JValue("Ja"));
                }
                
                // Replacing "No" with "Nei".
                foundJTokens = jObject.FindJTokensByPropertyValue("No");
                foreach (var foundJToken in foundJTokens)
                {
                    foundJToken.Replace(new JValue("Nei"));
                }                
                
                // Replacing "Cat" with "Kat".
                foundJTokens = jObject.FindJTokensByPropertyValue("Cat");
                foreach (var foundJToken in foundJTokens)
                {
                    foundJToken.Replace(new JValue("Kat"));
                }
                
                // Replacing "Dog" with "Hund".
                foundJTokens = jObject.FindJTokensByPropertyValue("Dog");
                foreach (var foundJToken in foundJTokens)
                {
                    foundJToken.Replace(new JValue("Hund"));
                }
                
                jObjectAsString = jObject.ToString();
                
                var jObjectFileName = Path.GetFileName(jObjectFileFullName);
                var jObjectTranslatedFileName = $"{jObjectFileName.Substring(0, jObjectFileName.IndexOf('-'))}-jobject_translated.txt";
                await File.WriteAllTextAsync(Path.Combine(OutputJObjectTranslatedFullPath, jObjectTranslatedFileName), jObjectAsString);
            }
        }
    }
}