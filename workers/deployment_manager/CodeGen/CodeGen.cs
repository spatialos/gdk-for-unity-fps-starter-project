using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CodeGen.BundleModel;
using CommandLine;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ValueType = CodeGen.BundleModel.ValueType;

namespace CodeGen
{
    internal class Options
    {
        [Option("input-bundle", Required = true,
            HelpText = "The path to the JSON Bundle file output by the SpatialOS schema_compiler.")]
        public string InputBundle { get; set; }

        [Option("output-dir", Required = true,
            HelpText = "The path to write the generated code to.")]
        public string OutputDir { get; set; }
    }

    internal class Lookups
    {
        public IReadOnlyDictionary<Identifier, TypeDefinition> Types;
        public IReadOnlyDictionary<Identifier, EnumDefinition> Enums;
        public IReadOnlyDictionary<string, Identifier> Identifiers;
    }

    internal class Program
    {
        public static Dictionary<PrimitiveType, string> schemaToCSharpTypes = new Dictionary<PrimitiveType, string>
        {
            {PrimitiveType.Double, "double"},
            {PrimitiveType.Float, "float"},
            {PrimitiveType.Int32, "int"},
            {PrimitiveType.Int64, "long"},
            {PrimitiveType.Uint32, "uint"},
            {PrimitiveType.Uint64, "ulong"},
            {PrimitiveType.Sint32, "int"},
            {PrimitiveType.Sint64, "long"},
            {PrimitiveType.Fixed32, "uint"},
            {PrimitiveType.Fixed64, "ulong"},
            {PrimitiveType.Sfixed32, "int"},
            {PrimitiveType.Sfixed64, "long"},
            {PrimitiveType.Bool, "bool"},
            {PrimitiveType.String, "string"},
            {PrimitiveType.Bytes, "byte[]"},
            {PrimitiveType.EntityId, "global::Improbable.EntityId"}
        };

        public static Dictionary<PrimitiveType, Func<string, string>> schemaToHashFunction = new Dictionary<PrimitiveType, Func<string, string>>
        {
            {PrimitiveType.Double, f => $"{f}.GetHashCode()"},
            {PrimitiveType.Float, f => $"{f}.GetHashCode()"},
            {PrimitiveType.Int32, f => f},
            {PrimitiveType.Int64, f => $"(int){f}"},
            {PrimitiveType.Uint32, f => $"(int){f}"},
            {PrimitiveType.Uint64, f => $"(int){f}"},
            {PrimitiveType.Sint32, f => f},
            {PrimitiveType.Sint64, f => $"(int){f}"},
            {PrimitiveType.Fixed32, f => f},
            {PrimitiveType.Fixed64, f => $"(int){f}"},
            {PrimitiveType.Sfixed32, f => f},
            {PrimitiveType.Sfixed64, f => $"(int){f}"},
            {PrimitiveType.Bool, f => $"{f}.GetHashCode()"},
            {PrimitiveType.String, f => $"{f} != null ? {f}.GetHashCode() : 0"},
            {PrimitiveType.Bytes, f => $"{f}.GetHashCode()"},
            {PrimitiveType.EntityId, f => $"{f}.GetHashCode()"},
        };

        public static Dictionary<PrimitiveType, Func<string, string>> schemaToEqualsFunction = new Dictionary<PrimitiveType, Func<string, string>>
        {
            {PrimitiveType.Double, f => $"{f}.Equals(other.{f})"},
            {PrimitiveType.Float, f => $"{f}.Equals(other.{f})"},
            {PrimitiveType.Int32, f => $"{f} == other.{f}"},
            {PrimitiveType.Int64, f => $"{f} == other.{f}"},
            {PrimitiveType.Uint32, f => $"{f} == other.{f}"},
            {PrimitiveType.Uint64, f => $"{f} == other.{f}"},
            {PrimitiveType.Sint32, f => $"{f} == other.{f}"},
            {PrimitiveType.Sint64, f => $"{f} == other.{f}"},
            {PrimitiveType.Fixed32, f => $"{f} == other.{f}"},
            {PrimitiveType.Fixed64, f => $"{f} == other.{f}"},
            {PrimitiveType.Sfixed32, f => $"{f} == other.{f}"},
            {PrimitiveType.Sfixed64, f => $"{f} == other.{f}"},
            {PrimitiveType.Bool, f => $"{f} == other.{f}"},
            {PrimitiveType.String, f => $"string.Equals({f}, other.{f})"},
            {PrimitiveType.Bytes, f => $"Equals({f}, other.{f})"},
            {PrimitiveType.EntityId, f => $"Equals({f}, other.{f})"},
        };

        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(Run)
                .WithNotParsed(errors =>
                {
                    foreach (var error in errors)
                    {
                        Console.Error.WriteLine(error);
                    }

                    Environment.ExitCode = 1;
                });
        }

        private class PascalCaseNamingStrategy : NamingStrategy
        {
            protected override string ResolvePropertyName(string name)
            {
                var pascal = char.ToLowerInvariant(name[0]) + name.Substring(1, name.Length - 1);
                return pascal;
            }
        }

        private static void Run(Options options)
        {
            try
            {
                var contractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new PascalCaseNamingStrategy()
                };

                var settings = new JsonSerializerSettings
                {
                    ContractResolver = contractResolver,
                };

                var bundleFile = JsonConvert.DeserializeObject<SchemaBundle>(File.ReadAllText(options.InputBundle, Encoding.UTF8), settings);
                var componentDataTypes = new HashSet<Identifier>();

                var bundle = bundleFile.V1;

                var lookups = new Lookups
                {
                    Types = bundle.TypeDefinitions.ToDictionary(t => t.Identifier, t => t),
                    Enums = bundle.EnumDefinitions.ToDictionary(t => t.Identifier, t => t),
                    Identifiers = bundle.ComponentDefinitions.Select(c => c.Identifier)
                        .Union(bundle.TypeDefinitions.Select(t => t.Identifier))
                        .Union(bundle.EnumDefinitions.Select(e => e.Identifier))
                        .ToDictionary(i => i.QualifiedName, i => i)
                };

                foreach (var c in bundle.ComponentDefinitions)
                {
                    var componentId = $"public static uint ComponentId => {c.ComponentId};";
                    var workerSdkAdapter = new StringBuilder();

                    var fields = c.FieldDefinitions;
                    if (c.DataDefinition != null)
                    {
                        // FIXME: Make this a public c.DataDefinition.Identifier.Name Data rather than embedding the fields into the type.
                        fields = lookups.Types[lookups.Identifiers[c.DataDefinition.QualifiedName]].FieldDefinitions;
                    }

                    var desc = GenerateDataType(c.Identifier, fields, lookups);
                    CreateWorkerSdkAdapterMethods(c.Identifier, fields, lookups, workerSdkAdapter);

                    desc.Sections.Insert(0, componentId);
                    desc.Sections.Add(workerSdkAdapter.ToString());

                    componentDataTypes.Add(c.Identifier);

                    WriteTypeToFile(options, desc);
                }

                var nestedTypes =
                    new HashSet<Identifier>(lookups.Identifiers.Values.SelectMany(id => GetNestedTypes(id, lookups)));

                var nestedEnums =
                    new HashSet<Identifier>(lookups.Identifiers.Values.SelectMany(id => GetNestedEnums(id, lookups)));

                // Write out raw types, which aren't the implicitly-generated <Component>Data type.
                var rawTypes = bundle.TypeDefinitions.Where(t => !componentDataTypes.Contains(t.Identifier) && !nestedTypes.Contains(t.Identifier));
                foreach (var t in rawTypes)
                {
                    var description = GenerateDataType(t.Identifier, t.FieldDefinitions, lookups);

                    WriteTypeToFile(options, description);
                }

                var rawEnums = bundle.EnumDefinitions.Where(e => !nestedEnums.Contains(e.Identifier));
                foreach (var e in rawEnums)
                {
                    WriteTypeToFile(options, e);
                }

                WriteWorkerSdkAdaptersToFile(options, bundle.ComponentDefinitions);
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(exception);
                Environment.ExitCode = 1;
            }
        }

        private static void WriteTypeToFile(Options options, TypeDescription t)
        {
            var folder = Path.Combine(options.OutputDir,
                t.Namespace.Replace('.', Path.DirectorySeparatorChar));
            t.Namespace = t.Namespace;
            var filename = $"{t.Name}.g.cs";

            var impl = new StringBuilder();

            var interpolated =$@"// Generated by SpatialOS C# Codegen at {DateTime.Now}
namespace Sync.{t.Namespace}
{{
{GenerateDataTypeString(t, 1)}
}}
";
            WriteFile(folder, filename, interpolated);
        }

        private static void WriteTypeToFile(Options options, EnumDefinition e)
        {
            var ns = string.Join(".", e.Identifier.Path.Take(e.Identifier.Path.Count - 1).Select(CaseConversion.CapitalizeFirstLetter));

            var folder = Path.Combine(options.OutputDir, ns);
            var filename = $"{e.Identifier.Name}.g.cs";
            var interpolated =$@"// Generated by SpatialOS C# Codegen at {DateTime.Now}
namespace Sync.{ns}
{{
{GenerateEnumString(e, 1)}
}}
";
            WriteFile(folder, filename, interpolated);
        }

        private static string GenerateDataTypeString(TypeDescription description, int indent = 0)
        {
            var allSections = new StringBuilder();
            foreach (var s in description.Sections)
            {
                allSections.Append(Indent(indent + 1, s.TrimEnd()));
                allSections.AppendLine();
            }

            string storageType;
            switch (description.Storage)
            {
                case OutputStorage.Struct:
                    storageType = "struct";
                    break;
                case OutputStorage.Class:
                    storageType = "sealed class";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var inheritance = new StringBuilder();
            for (var i = 0; i < description.Inheritance.Count; i++)
            {
                if (i == 0)
                {
                    inheritance.Append(" : ");
                }

                inheritance.Append(description.Inheritance[i]);

                if (i < description.Inheritance.Count - 1)
                {
                    inheritance.Append(", ");
                }
            }

            foreach (var s in description.NestedTypes)
            {
                allSections.AppendLine($"{GenerateDataTypeString(s, indent + 1).TrimEnd()}");
            }

            foreach (var e in description.NestedEnums)
            {
                allSections.AppendLine($"{GenerateEnumString(e, indent + 1).TrimEnd()}");
            }

            var interpolated = $@"
public {storageType} {description.Name}{inheritance}
{{
{Indent(1, allSections.ToString().TrimEnd())}
}}
";
            return Indent(indent, interpolated);
        }

        public static string GenerateEnumString(EnumDefinition e, int indent = 0)
        {
            var values = new StringBuilder();
            foreach (var v in e.ValueDefinitions)
            {
                values.AppendLine(Indent(indent + 1, $"{v.Identifier.Name} = {v.Value},"));
            }

            var interpolated = $@"
public enum {CaseConversion.CapitalizeFirstLetter(e.Identifier.Name)}
{{
{values.ToString().TrimEnd()}
}};
";
            return Indent(indent, interpolated);
        }

        private static TypeDescription GenerateDataType(Identifier identifier, IReadOnlyList<FieldDefinition> fields, Lookups lookups)
        {
            var fieldText = new StringBuilder();
            var equatableImpl = new StringBuilder();
            var enumText = new StringBuilder();
            var nestedTypes = new StringBuilder();
            var storage = OutputStorage.Struct;
            var csharpName = CaseConversion.SnakeCaseToPascalCase(identifier.Name);

            CreateFields(fields, fieldText, lookups);

            var inheritsFrom = $"System.IEquatable<{csharpName}>";

            if (identifier.Name.EndsWith("Attribute"))
            {
                storage = OutputStorage.Class;
                inheritsFrom = "System.Attribute";
            }
            else
            {
                CreateEquatableImpl(identifier, fields, equatableImpl);
            }

            var desc = new TypeDescription
            {
                Name = csharpName,
                Namespace = $"{CaseConversion.GetNamespaceFromTypeName(identifier.QualifiedName)}",
                Storage = storage,
                Sections = new List<string> {fieldText.ToString(), enumText.ToString(), nestedTypes.ToString(), equatableImpl.ToString()},
                Inheritance = new List<string> {inheritsFrom},
                NestedTypes = new List<TypeDescription>(),
                NestedEnums = new List<EnumDefinition>()
            };

            var directNestedTypes = GetNestedTypes(identifier, lookups);
            var directNestedEnums = GetNestedEnums(identifier, lookups);

            desc.NestedTypes.AddRange(directNestedTypes.Select(id =>
            {
                var t = lookups.Types[id];
                return GenerateDataType(t.Identifier, t.FieldDefinitions, lookups);
            }));

            desc.NestedEnums.AddRange(directNestedEnums.Select(id => lookups.Enums[id]));

            return desc;
        }


        private static List<Identifier> GetNestedTypes(Identifier identifier, Lookups lookups)
        {
            return lookups.Types.Where(t =>
                t.Key.QualifiedName.StartsWith(identifier.QualifiedName) &&
                t.Key.Path.Count == identifier.Path.Count + 1)
                .Select(kv => kv.Key)
                .ToList();
        }

        private static List<Identifier> GetNestedEnums(Identifier identifier, Lookups lookups)
        {
            return lookups.Enums.Where(t =>
                    t.Key.QualifiedName.StartsWith(identifier.QualifiedName) &&
                    t.Key.Path.Count == identifier.Path.Count + 1)
                .Select(kv => kv.Key)
                .ToList();
        }

        private static void CreateEquatableImpl(Identifier identifier, IReadOnlyList<FieldDefinition> fields, StringBuilder impl)
        {
            var hashFields = new StringBuilder();
            var equalsFields = new StringBuilder();

            if (!fields.Any())
            {
                hashFields.AppendLine($"hashCode = base.GetHashCode();");
                equalsFields.AppendLine($"Equals(this, other);");
            }
            else
            {
                foreach (var field in fields)
                {
                    hashFields.AppendLine($"hashCode = (hashCode * 397) ^ ({FieldToHash(field)});");
                }

                for (var i = 0; i < fields.Count; i++)
                {
                    var field = fields[i];
                    equalsFields.Append($"{FieldToEquals(field)}");
                    if (i <fields.Count - 1)
                    {
                        equalsFields.AppendLine(" &&");
                    }
                }
            }

            var typeName = CaseConversion.CapitalizeFirstLetter(identifier.Name);
            var interpolation =
                $@"public override int GetHashCode()
{{
    unchecked
    {{
        var hashCode = 0;
{Indent(2, hashFields.ToString().TrimEnd())}
        return hashCode;
    }}
}}

public bool Equals({typeName} other)
{{
    return
{Indent(2, equalsFields.ToString())};
}}

public override bool Equals(object obj)
{{
    if (ReferenceEquals(null, obj))
    {{
        return false;
    }}

    return obj is {typeName} other && Equals(other);
}}

public static bool operator ==({typeName} a, {typeName} b)
{{
    return a.Equals(b);
}}

public static bool operator !=({typeName} a, {typeName} b)
{{
    return a.Equals(b);
}}
";
            impl.Append(interpolation);
        }

        private static void WriteWorkerSdkAdaptersToFile(Options options, IReadOnlyList<ComponentDefinition> syncComponents)
        {
            var sendUpdate = new StringBuilder();
            var fromData = new StringBuilder();
            var applyUpdate = new StringBuilder();
            var componentIds = new StringBuilder();
            var typeMap = new StringBuilder();

            foreach (var c in syncComponents)
            {
                var syncName = $"Sync.{CaseConversion.CapitalizeNamespace(c.Identifier.QualifiedName)}";
                sendUpdate.AppendLine(
$@"
{{
    {c.ComponentId}, // {syncName}
    (connection, entityId, jsonText) =>
    {{
        var obj = JsonConvert.DeserializeObject<global::{syncName}>(jsonText);
        connection.SendComponentUpdate(entityId, obj.ToUpdate());
    }}
}},");
                fromData.AppendLine(
                    $"{{ {c.ComponentId}, global::{syncName}.FromData }},");
                applyUpdate.AppendLine(
                    $"{{ {c.ComponentId}, global::{syncName}.ApplyUpdate }},");
                componentIds.AppendLine($"{c.ComponentId},");
                typeMap.AppendLine($"{{ typeof(global::{syncName}), {c.ComponentId} }},");
            }

            const string sendUpdateType = "Dictionary<uint, Action<Improbable.Worker.Connection, Improbable.EntityId, string>>";
            const string fromDataType = "Dictionary<uint, Func<object, object>>";
            const string applyUpdateType = "Dictionary<uint, Func<object, object, object>>";
            const string typeMapType = "Dictionary<Type, uint>";

            var databaseSerializers =
$@"// Generated by SpatialOS C# Codegen at {DateTime.Now}
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Improbable.Database
{{
    public static class Adapter
    {{
        private static {sendUpdateType} sendUpdate = new {sendUpdateType}
        {{
{Indent(3, sendUpdate.ToString().TrimEnd())}
        }};

        private static {fromDataType} fromData = new {fromDataType}
        {{
{Indent(3, fromData.ToString().TrimEnd())}
        }};

        private static {applyUpdateType} applyUpdate = new {applyUpdateType}
        {{
{Indent(3, applyUpdate.ToString().TrimEnd())}
        }};

        private static HashSet<uint> exposedToDatabase = new HashSet<uint>()
        {{
{Indent(3, componentIds.ToString().TrimEnd())}
        }};

        private static {typeMapType} typeMap = new {typeMapType}()
        {{
{Indent(3, typeMap.ToString().TrimEnd())}
        }};

        public static bool IsExposedToDatabase(uint componentId)
        {{
            return exposedToDatabase.Contains(componentId);
        }}

        public static void SendUpdate(Improbable.Worker.Connection connection, Improbable.EntityId entityId, uint componentId, string jsonText)
        {{
            sendUpdate[componentId](connection, entityId, jsonText);
        }}

        public static object FromData(uint componentId, object objData)
        {{
            return fromData[componentId](objData);
        }}

        public static object ApplyUpdate(uint componentId, object objData, object updateData)
        {{
            return applyUpdate[componentId](objData, updateData);
        }}

        public static uint GetComponentId(Type type)
        {{
            return typeMap[type];
        }}
    }}
}}
";
            WriteFile(options.OutputDir, "Adapter.g.cs", databaseSerializers);
        }

        private static void CreateWorkerSdkAdapterMethods(Identifier identifier, IReadOnlyList<FieldDefinition> fields, Lookups lookups, StringBuilder toUpdate)
        {
            var fieldSetters = new StringBuilder();
            var dataSetters = new StringBuilder();
            var updateSetters = new StringBuilder();
            foreach (var field in fields)
            {
                var fieldType = GetFieldTypeAsCsharp(field, lookups);
                var fieldInstantiation = GetFieldInstantiationAsCsharp(field, lookups);
                var workerFieldType = GetFieldTypeAsWorkerCsharp(field, lookups);

                var fieldName = CaseConversion.SnakeCaseToPascalCase(field.Identifier.Name);
                var camelCaseFieldName = CaseConversion.SnakeCaseToCamelCase(field.Identifier.Name);

                updateSetters.AppendLine($"if (spatialUpdate.{camelCaseFieldName}.HasValue)");
                updateSetters.AppendLine("{");
                updateSetters.Append("\t");

                switch (field.TypeSelector)
                {
                    case FieldType.Option:
                        fieldSetters.AppendLine($"update.Set{fieldName}({fieldName}.HasValue ? new {workerFieldType}({fieldName}.Value) : new {workerFieldType}());");

                        dataSetters.AppendLine($"newData.{fieldName} = spatialData.Value.{camelCaseFieldName}.HasValue ? spatialData.Value.{camelCaseFieldName}.Value : ({fieldType}) null;");
                        updateSetters.AppendLine($"spatialData.{fieldName} = spatialUpdate.{camelCaseFieldName}.Value.HasValue ? spatialUpdate.{camelCaseFieldName}.Value.Value : ({fieldType}) null;");
                        break;
                    case FieldType.List:
                    case FieldType.Map:
                        fieldSetters.AppendLine($"update.Set{fieldName}(new {workerFieldType}({fieldName}));");

                        dataSetters.AppendLine($"newData.{fieldName} = new {fieldInstantiation}(spatialData.Value.{camelCaseFieldName});");
                        updateSetters.AppendLine($"spatialData.{fieldName} = new {fieldInstantiation} (spatialUpdate.{camelCaseFieldName}.Value);");
                        break;
                    case FieldType.Singular:
                        if (field.SingularType.Type.Primitive == PrimitiveType.Bytes)
                        {
                            fieldSetters.AppendLine($"update.Set{fieldName}(global::Improbable.Worker.Bytes.CopyOf({fieldName}));");

                            dataSetters.AppendLine($"newData.{fieldName} = spatialData.Value.{camelCaseFieldName}.GetCopy();");
                            updateSetters.AppendLine($"spatialData.{fieldName} = spatialUpdate.{camelCaseFieldName}.Value.GetCopy();");
                        }
                        else
                        {
                            fieldSetters.AppendLine($"update.Set{fieldName}({fieldName});");

                            dataSetters.AppendLine($"newData.{fieldName} = spatialData.Value.{camelCaseFieldName};");
                            updateSetters.AppendLine($"spatialData.{fieldName} = (spatialUpdate.{camelCaseFieldName}.Value);");
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                updateSetters.AppendLine("}");
            }

            var name = CaseConversion.CapitalizeNamespace(identifier.QualifiedName);
            var updateSerializer = $@"
public global::Improbable.Worker.IComponentUpdate<global::{name}> ToUpdate()
{{
    var update = new global::{name}.Update();
{fieldSetters.ToString().TrimEnd()}
    return update;
}}

public static object FromData(object data)
{{
    var newData = new Sync.{name}();
    var spatialData = (global::{name}.Data)data;
{dataSetters.ToString().TrimEnd()}
    return newData;
}}

public static object ApplyUpdate(object data, object update)
{{
    var spatialData = (Sync.{name})data;
    var spatialUpdate = (global::{name}.Update)update;
{updateSetters.ToString().TrimEnd()}
    return spatialData;
}}
";
            toUpdate.Append(updateSerializer);
        }

        private static void WriteFile(string folder, string filename, string text)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            File.WriteAllText(Path.Combine(folder, filename), text,
                Encoding.UTF8);
        }

        private static void CreateFields(IReadOnlyList<FieldDefinition> fields, StringBuilder fieldText, Lookups lookups)
        {
            foreach (var field in fields)
            {
                if (field.Annotations.Any(a => a.TypeValue.Type.QualifiedName == "improbable.database.NoSyncAttribute"))
                {
                    fieldText.AppendLine(
                        $"// NoSyncAttribute: {GetFieldTypeAsCsharp(field, lookups)} {CaseConversion.SnakeCaseToPascalCase(field.Identifier.Name)};");
                    continue;
                }

                fieldText.AppendLine($"[Newtonsoft.Json.JsonProperty(\"{field.Identifier.Name}\")]");
                fieldText.AppendLine(
                    $"public {GetFieldTypeAsCsharp(field, lookups)} {CaseConversion.SnakeCaseToPascalCase(field.Identifier.Name)};");

                fieldText.AppendLine();
            }
        }


        public static string GetFieldTypeAsCsharp(FieldDefinition field, Lookups lookups)
        {
            switch (field.TypeSelector)
            {
                case FieldType.Option:
                    return $"{TypeReferenceToType(field.OptionType.InnerType, lookups)}?";
                case FieldType.List:
                    return $"System.Collections.Generic.IReadOnlyList<{TypeReferenceToType(field.ListType.InnerType, lookups)}>";
                case FieldType.Map:
                    return $"System.Collections.Generic.IReadOnlyDictionary<{TypeReferenceToType(field.MapType.KeyType, lookups)}, {TypeReferenceToType(field.MapType.ValueType, lookups)}>";
                case FieldType.Singular:
                    return TypeReferenceToType(field.SingularType.Type, lookups);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static string GetFieldInstantiationAsCsharp(FieldDefinition field, Lookups lookups)
        {
            switch (field.TypeSelector)
            {
                case FieldType.Option:
                    return $"{TypeReferenceToType(field.OptionType.InnerType, lookups)}";
                case FieldType.List:
                    return $"System.Collections.Generic.List<{TypeReferenceToType(field.ListType.InnerType, lookups)}>";
                case FieldType.Map:
                    return $"System.Collections.Generic.Dictionary<{TypeReferenceToType(field.MapType.KeyType, lookups)}, {TypeReferenceToType(field.MapType.ValueType, lookups)}>";
                case FieldType.Singular:
                    return TypeReferenceToType(field.SingularType.Type, lookups);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static string GetFieldTypeAsWorkerCsharp(FieldDefinition field, Lookups lookups)
        {
            switch (field.TypeSelector)
            {
                case FieldType.Option:
                    return $"global::Improbable.Collections.Option<{TypeReferenceToType(field.OptionType.InnerType, lookups)}>";
                case FieldType.List:
                    return $"global::Improbable.Collections.List<{TypeReferenceToType(field.ListType.InnerType, lookups)}>";
                case FieldType.Map:
                    return $"global::Improbable.Collections.Map<{TypeReferenceToType(field.MapType.KeyType, lookups)}, {TypeReferenceToType(field.MapType.ValueType, lookups)}>";
                case FieldType.Singular:
                    return TypeReferenceToType(field.SingularType.Type, lookups);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static string GetSyncNameFromQualifiedName(Identifier identifier, IReadOnlyDictionary<Identifier, TypeDefinition> types)
        {
            return GetSyncName(types[identifier].Identifier);
        }

        private static string GetSyncNameFromQualifiedName(Identifier identifier, IReadOnlyDictionary<Identifier, EnumDefinition> enums)
        {
            return GetSyncName(enums[identifier].Identifier);
        }

        private static string GetSyncName(Identifier identifier)
        {
            return
                $"{CaseConversion.CapitalizeNamespace(identifier.QualifiedName)}";
        }

        public static string TypeReferenceToType(ValueTypeReference typeRef, Lookups lookups)
        {
            switch (typeRef.ValueTypeSelector)
            {
                case ValueType.Enum:
                    return $"global::{GetSyncNameFromQualifiedName(lookups.Identifiers[typeRef.Enum.QualifiedName], lookups.Enums)}";
                case ValueType.Primitive:
                    return schemaToCSharpTypes[typeRef.Primitive];
                case ValueType.Type:
                    return $"global::{GetSyncNameFromQualifiedName(lookups.Identifiers[typeRef.Type.QualifiedName], lookups.Types)}";
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        public static string FieldToHash(FieldDefinition field)
        {
            var fieldName = CaseConversion.SnakeCaseToPascalCase(field.Identifier.Name);

            switch (field.TypeSelector)
            {
                case FieldType.Option:
                    return $"{fieldName}.HasValue ? {fieldName}.Value.GetHashCode() : 0";
                case FieldType.List:
                case FieldType.Map:
                    return $"{fieldName} == null ? {fieldName}.GetHashCode() : 0";
                case FieldType.Singular:
                    if (field.SingularType.Type.Primitive == PrimitiveType.Invalid)
                    {
                        return $"{fieldName}.GetHashCode()";
                    }

                    return schemaToHashFunction[field.SingularType.Type.Primitive](fieldName);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static string FieldToEquals(FieldDefinition field)
        {
            var fieldName = CaseConversion.SnakeCaseToPascalCase(field.Identifier.Name);

            switch (field.TypeSelector)
            {
                case FieldType.Option:
                    return $"{fieldName} == other.{fieldName}";
                case FieldType.List:
                case FieldType.Map:
                    return $"Equals({fieldName}, other.{fieldName})";
                case FieldType.Singular:
                    if (field.SingularType.Type.Primitive == PrimitiveType.Invalid)
                    {
                        return $"{fieldName} == other.{fieldName}";
                    }

                    return schemaToEqualsFunction[field.SingularType.Type.Primitive](fieldName);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static string Indent(int level, string inputString)
        {
            var indent = string.Empty.PadLeft(level, '\t');
            return inputString.Replace("\n", $"\n{indent}");
        }

        private enum OutputStorage
        {
            Struct,
            Class
        }

        private struct TypeDescription
        {
            public OutputStorage Storage { get; set; }

            public string Namespace { get; set; }

            public string Name { get; set; }

            public List<string> Inheritance { get; set; }

            public List<string> Sections { get; set; }

            public List<TypeDescription> NestedTypes { get; set; }

            public List<EnumDefinition> NestedEnums { get; set; }
        }
    }
}
