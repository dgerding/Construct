using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Construct.Server.Entities;
using System.Collections.Generic;

namespace Construct.Server.Models.Data.CodeGeneration
{
    public class TypesAssemblyCreator
    {
        private readonly Construct.Server.Entities.EntitiesModel context;
        private string assembliesDirectory;

        public TypesAssemblyCreator(Construct.Server.Entities.EntitiesModel context)
        {
            this.context = context;
            assembliesDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        // Use this method when the dll you are generating is of a type that DOES NOT exist in the DB/ORM
        public Assembly ReturnTypeAssembly(DataType dataType, IList<PropertyType> propertyTypes)
        {
            string assemblyName = "Construct.Types." + dataType.Name;
            string fullyQualifiedName = Path.Combine(assembliesDirectory, assemblyName + ".dll");

            Assembly generatedAssembly = null;

            if (File.Exists(fullyQualifiedName) == false)
            {
                generatedAssembly = GenerateAssembly(dataType, propertyTypes, assembliesDirectory, assemblyName);
            }
            else
            {
                generatedAssembly = Assembly.LoadFile(fullyQualifiedName);
            }

            return generatedAssembly;
        }

        // Use this method when the dll you are generating is of a type that already exists in the DB/ORM
        public Assembly ReturnTypeAssembly(DataType dataType)
        {
            string assemblyName = "Construct.Types." + dataType.Name;
            string fullyQualifiedName = Path.Combine(assembliesDirectory, assemblyName + ".dll");

            Assembly generatedAssembly = null;

            if (File.Exists(fullyQualifiedName) == false)
            {
                // dataType.PropertyTypes OR dataType.PropertyParents??
                // Need both methods, one for generating a type from DataType already in ORM context
                //                    the other for generating a type from DataType and propertyType NOT IN THE ORM
                generatedAssembly = GenerateAssembly(dataType, dataType.PropertyParents.Cast<PropertyType>().ToList(), assembliesDirectory, assemblyName);
            }
            else
            {
                generatedAssembly = Assembly.LoadFile(fullyQualifiedName);
            }

            return generatedAssembly;
        }

        private Assembly GenerateAssembly(DataType dataType, IList<PropertyType> propertyTypes, string assembliesDirectory, string assemblyName)
        {
            CodeDefinition.AssemblyDefinition assembly = CodeDefinition.Create
                                                                       .SetName(assemblyName)
                                                                       .SetDirectory(assembliesDirectory);

            SetReferences(assembly);

            CodeDefinition.NamespaceDefinition namespaceDefinition = assembly.Namespace.SetName("Construct.Types");

            SetClassDefinition(dataType, propertyTypes, namespaceDefinition);
            SetMetaData(dataType, propertyTypes, namespaceDefinition);

            assembly.Compile();

            string filenameWithExtension = String.Format("{0}{1}", assemblyName, ".dll");
            EnhanceAssembly(assembliesDirectory, filenameWithExtension);

            string totalPath = Path.Combine(assembliesDirectory, filenameWithExtension);


            return Assembly.LoadFrom(totalPath);
        }

        private static string ToCamelCase(string name)
        {
            string newFirstLetter = (name.Substring(0, 1)).ToLower();
            return String.Format("{0}{1}", newFirstLetter, name.Substring(1));
        }

        private static string ToFirstLetterUpperCase(string name)
        {
            string newFirstLetter = (name.Substring(0, 1)).ToUpper();
            return String.Format("{0}{1}", newFirstLetter, name.Substring(1));
        }

        private static void SetMetaData(DataType dataType, IList<PropertyType> propertyTypes, CodeDefinition.NamespaceDefinition namespaceDefinition)
        {
            string body = "\t\t\tIList<MappingConfiguration> configurations = new List<MappingConfiguration>();\r\n";
            body += String.Format("\t\t\tMappingConfiguration<{0}> {0}Configuration = new MappingConfiguration<{0}>();\r\n", dataType.Name);
            body += String.Format("\t\t\t{0}Configuration.MapType().ToTable(\"Items_z{0}\");\r\n", dataType.Name);
            body += String.Format("\t\t\t{0}Configuration.HasProperty(p => p.ItemID).IsIdentity().HasFieldName(\"itemID\").ToColumn(\"ItemID\");\r\n", dataType.Name);
            body += String.Format("\t\t\t{0}Configuration.HasProperty(p => p.RecordCreationDate).HasFieldName(\"recordCreationDate\").ToColumn(\"RecordCreationDate\").IsCalculatedOn(Telerik.OpenAccess.Metadata.DateTimeAutosetMode.None);\r\n", dataType.Name);

            foreach (PropertyType propertyType in propertyTypes)
            {
                body += String.Format(
                    "\t\t\t{0}Configuration.HasProperty(p => p.{1}).HasFieldName(\"{2}\").ToColumn(\"{1}\");\r\n",
                    dataType.Name,
                    ToFirstLetterUpperCase(propertyType.Name),
                    ToCamelCase(propertyType.Name));
            }
            body += String.Format("\t\t\tconfigurations.Add({0}Configuration);\n", dataType.Name);
            body += String.Format("\t\t\treturn configurations;");

            namespaceDefinition.Class
                               .SetName(String.Format("{0}MetaData", dataType.Name))
                               .SetInheritance("FluentMetadataSource")
                               .Method
                               .SetScope(CodeDefinition.Scope.Protected | CodeDefinition.Scope.Override)
                               .SetType("IList<MappingConfiguration>")
                               .SetName("PrepareMapping")
                               .SetBody(body);
        }

        private static void EnhanceAssembly(string assembliesDirectory, string assemblyName)
        {
            AssemblyEnhancer enhancer = new AssemblyEnhancer();
            enhancer.TargetAssembly = assemblyName;
            enhancer.TargetAssemblyDirectory = assembliesDirectory;
            enhancer.vEnhanceDirectory = ConfigurationManager.AppSettings["vEnhancePath"];

            //TODO incoming hack 
            if (string.IsNullOrEmpty(enhancer.vEnhanceDirectory))
            {
                enhancer.vEnhanceDirectory = @"C:\Construct\Assemblies";
            }


            string totalPath = Path.Combine(assembliesDirectory, assemblyName);

            try
            {
                if (enhancer.Enhance())
                {
                    //string executingDirAssemblyPath = Path.Combine(Environment.CurrentDirectory, assemblyGenerator.Name);
                    //File.Copy(Path.Combine(workingDirectory, assemblyGenerator.Name), executingDirAssemblyPath, true);
                }
                else
                {
                    //todo: Raise error messagethat 
                    throw new Telerik.OpenAccess.Exceptions.ObjectNotEnhancedException(enhancer.TargetAssembly);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                throw e;
            }
        }

        private static void SetClassDefinition(DataType dataType, IList<PropertyType> propertyTypes, CodeDefinition.NamespaceDefinition namespaceDefinition)
        {
            CodeDefinition.ClassDefinition classDefiniton = namespaceDefinition.Class
                                                                                    .SetName(dataType.Name);
            classDefiniton.Attribute.SetName("Persistent");

            foreach (PropertyType propertyType in propertyTypes)
            {
                string lowerName = String.Format("{0}{1}", propertyType.Name.Substring(0, 1).ToLower(), propertyType.Name.Substring(1));
                string upperName = String.Format("{0}{1}", propertyType.Name.Substring(0, 1).ToUpper(), propertyType.Name.Substring(1));
                classDefiniton.Field
                                    .SetName(lowerName)
                                    .SetScope(CodeDefinition.Scope.Private)
                                    .SetType(propertyType.DataType.Name);

                classDefiniton.Property
                                    .SetName(upperName)
                                    .SetScope(CodeDefinition.Scope.Public)
                                    .SetType(propertyType.DataType.Name)
                                    .SetGetter("return " + lowerName + ";")
                                    .SetSetter(lowerName + " = value;");
            }

            classDefiniton.Field
                                    .SetName("itemID")
                                    .SetScope(CodeDefinition.Scope.Private)
                                    .SetType("Guid");

            classDefiniton.Property
                                .SetName("ItemID")
                                .SetScope(CodeDefinition.Scope.Public)
                                .SetType("Guid")
                                .SetGetter("return itemID;")
                                .SetSetter("itemID = value;");

            classDefiniton.Field
                        .SetName("recordCreationDate")
                        .SetScope(CodeDefinition.Scope.Private)
                        .SetType("DateTime");

            classDefiniton.Property
                                .SetName("RecordCreationDate")
                                .SetScope(CodeDefinition.Scope.Public)
                                .SetType("DateTime")
                                .SetGetter("return recordCreationDate;")
                                .SetSetter("recordCreationDate = value;");

        }

        private void SetReferences(CodeDefinition.AssemblyDefinition assembly)
        {
            string telerikOpenAccessLocation = Path.GetDirectoryName(typeof(Telerik.OpenAccess.AbstractBlob).Assembly.Location);
            string telerikOpenAccess35ExtensionLocation = Path.GetDirectoryName(typeof(Telerik.OpenAccess.Metadata.Fluent.FluentMetadataSource).Assembly.Location);
            string thisPath = Directory.GetParent(this.GetType().Assembly.Location).FullName;
            
            string path = assembliesDirectory;
            assembly = assembly.ReferencedAssembly
                                    .SetNamespace("System")
                               .Parent.ReferencedAssembly
                                    .SetName("System.Dynamic")
                               // TODO: Why did removing this make it compile, where it didn't before
                               //.Parent.ReferencedAssembly
                               //     .SetName("System.Linq")
                               .Parent.ReferencedAssembly
                                    .SetNamespace("System.Linq.Expressions")
                               .Parent.ReferencedAssembly
                                    .SetNamespace("System.Collections")
                               .Parent.ReferencedAssembly
                                    .SetNamespace("System.Collections.Generic")
                               .Parent.ReferencedAssembly
                                    .SetName("System.Runtime.Serialization")
                               .Parent.ReferencedAssembly
                                    .SetName("Telerik.OpenAccess")
                                    .SetPath(telerikOpenAccessLocation)
                               .Parent.ReferencedAssembly
                                    .SetName("Telerik.OpenAccess.35.Extensions")
                                    .SetNamespace(null)
                                    .SetPath(telerikOpenAccess35ExtensionLocation)
                               .Parent.ReferencedAssembly
                                    .SetNamespace("Telerik.OpenAccess.Metadata")
                               .Parent.ReferencedAssembly
                                    .SetNamespace("Telerik.OpenAccess.Metadata.Fluent")
                               .Parent.ReferencedAssembly
                                    .SetName("Construct.Server.Models.Data")
                                    .SetPath(thisPath)
                               .Parent;
        }
    }
}