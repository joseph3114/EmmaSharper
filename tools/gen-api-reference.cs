#:package System.Reflection.MetadataLoadContext@9.0.0
#:property PublishAot=false

// Generates the wiki's API Reference page from the built assembly and its XML documentation.
//
// Reflection supplies the signatures - parameter names, defaults, real return types - which the
// XML file does not carry. The XML supplies the prose. Joining them means the page cannot drift
// from the code: regenerate it and any rename or new overload shows up.
//
//   dotnet tools/gen-api-reference.cs -- <assembly.dll> <docs.xml> <output.md>

using System.Globalization;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

string dllPath = args.Length > 0 ? args[0] : throw new ArgumentException("assembly path required");
string xmlPath = args.Length > 1 ? args[1] : throw new ArgumentException("xml path required");
string outPath = args.Length > 2 ? args[2] : throw new ArgumentException("output path required");

// ---- XML docs -------------------------------------------------------------------------------
Dictionary<string, XElement> docs = XDocument.Load(xmlPath)
    .Descendants("member")
    .Where(m => m.Attribute("name") is not null)
    .GroupBy(m => m.Attribute("name")!.Value)
    .ToDictionary(g => g.Key, g => g.First());

// ---- Reflect over the assembly without executing it or resolving its dependencies -------------
// The resolver needs the whole closure, not just the target: decoding a method signature that
// mentions IServiceCollection or IHttpClientBuilder requires those assemblies too. Point it at
// the publish folder, which has them all side by side.
string[] runtime = Directory.GetFiles(RuntimeEnvironment(), "*.dll");
string[] alongside = Directory.GetFiles(Path.GetDirectoryName(Path.GetFullPath(dllPath))!, "*.dll");
var resolver = new PathAssemblyResolver(runtime.Concat(alongside));
using var mlc = new MetadataLoadContext(resolver);
Assembly asm = mlc.LoadFromAssemblyPath(Path.GetFullPath(dllPath));

Type[] exported = asm.GetExportedTypes().OrderBy(t => t.FullName, StringComparer.Ordinal).ToArray();

bool IsProvider(Type t) => t.IsInterface && t.Name.StartsWith("IEmma", StringComparison.Ordinal);
bool IsException(Type t) => Inherits(t, "System.Exception");
bool IsEnumType(Type t) => t.IsEnum;
bool IsStatic(Type t) => t.IsAbstract && t.IsSealed;

var providers = exported.Where(IsProvider).ToArray();
var enums = exported.Where(IsEnumType).ToArray();
var exceptions = exported.Where(IsException).ToArray();
var statics = exported.Where(t => IsStatic(t) && !IsEnumType(t)).ToArray();
var models = exported
    .Where(t => t.IsClass && !IsException(t) && !IsStatic(t))
    .ToArray();

var sb = new StringBuilder();
sb.AppendLine("# API Reference");
sb.AppendLine();
sb.AppendLine("Every public type and member, generated from the compiled assembly and its XML");
sb.AppendLine("documentation. Regenerated at release, so it cannot drift from the code.");
sb.AppendLine();
sb.AppendLine($"`{asm.GetName().Name}` {asm.GetName().Version?.ToString(3)}");
sb.AppendLine();

// ---- Contents -------------------------------------------------------------------------------
sb.AppendLine("## Contents");
sb.AppendLine();
sb.AppendLine("**Providers**  ");
sb.AppendLine(string.Join(" · ", providers.Select(p => $"[{p.Name}](#{Anchor(p.Name)})")));
sb.AppendLine();
sb.AppendLine("**Configuration and helpers**  ");
sb.AppendLine(string.Join(" · ", statics.Concat(models.Where(m => m.Name is "EmmaOptions"))
    .Select(t => $"[{t.Name}](#{Anchor(t.Name)})")));
sb.AppendLine();
sb.AppendLine("**Exceptions**  ");
sb.AppendLine(string.Join(" · ", exceptions.Select(t => $"[{t.Name}](#{Anchor(t.Name)})")));
sb.AppendLine();
sb.AppendLine("**Enums**  ");
sb.AppendLine(string.Join(" · ", enums.Select(t => $"[{t.Name}](#{Anchor(t.Name)})")));
sb.AppendLine();
sb.AppendLine($"**Models** — {models.Length} types, listed at the end.");
sb.AppendLine();
sb.AppendLine("---");
sb.AppendLine();

// ---- Providers ------------------------------------------------------------------------------
sb.AppendLine("## Providers");
sb.AppendLine();
foreach (Type t in providers)
{
    EmitTypeHeader(t);
    MethodInfo[] methods = t.GetMethods()
        .Where(m => !m.IsSpecialName)
        .OrderBy(m => m.Name, StringComparer.Ordinal)
        .ToArray();

    foreach (MethodInfo m in methods)
    {
        EmitMethod(t, m);
    }

    PropertyInfo[] props = t.GetProperties().OrderBy(p => p.Name, StringComparer.Ordinal).ToArray();
    if (props.Length > 0)
    {
        EmitPropertyTable(t, props);
    }
}

// ---- Static helpers and options ---------------------------------------------------------------
sb.AppendLine("## Configuration and helpers");
sb.AppendLine();
foreach (Type t in statics.Concat(models.Where(m => m.Name is "EmmaOptions")))
{
    EmitTypeHeader(t);
    foreach (MethodInfo m in t.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                              .Where(m => !m.IsSpecialName)
                              .OrderBy(m => m.Name, StringComparer.Ordinal))
    {
        EmitMethod(t, m);
    }

    PropertyInfo[] props = t.GetProperties().OrderBy(p => p.Name, StringComparer.Ordinal).ToArray();
    if (props.Length > 0)
    {
        EmitPropertyTable(t, props);
    }
}

// ---- Exceptions -------------------------------------------------------------------------------
sb.AppendLine("## Exceptions");
sb.AppendLine();
foreach (Type t in exceptions)
{
    EmitTypeHeader(t);
    PropertyInfo[] props = t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                            .OrderBy(p => p.Name, StringComparer.Ordinal).ToArray();
    if (props.Length > 0)
    {
        EmitPropertyTable(t, props);
    }
}

// ---- Enums ------------------------------------------------------------------------------------
sb.AppendLine("## Enums");
sb.AppendLine();
foreach (Type t in enums)
{
    EmitTypeHeader(t);
    sb.AppendLine("| Member | Description |");
    sb.AppendLine("|---|---|");
    foreach (FieldInfo f in t.GetFields(BindingFlags.Public | BindingFlags.Static))
    {
        sb.AppendLine($"| `{f.Name}` | {Cell(Summary($"F:{DocType(t)}.{f.Name}"))} |");
    }
    sb.AppendLine();
}

// ---- Models -----------------------------------------------------------------------------------
sb.AppendLine("## Models");
sb.AppendLine();
sb.AppendLine("Data types returned by, or sent to, the providers above.");
sb.AppendLine();
foreach (Type t in models.Where(m => m.Name is not "EmmaOptions").OrderBy(t => t.Name, StringComparer.Ordinal))
{
    EmitTypeHeader(t);
    PropertyInfo[] props = t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .OrderBy(p => p.Name, StringComparer.Ordinal).ToArray();
    if (props.Length > 0)
    {
        EmitPropertyTable(t, props);
    }
}

sb.AppendLine("---");
sb.AppendLine();
sb.AppendLine("*Generated by `tools/gen-api-reference.cs`. Do not edit by hand — regenerate instead.*");

Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(outPath))!);
File.WriteAllText(outPath, sb.ToString());

Console.WriteLine($"wrote {outPath}");
Console.WriteLine($"  providers {providers.Length}, models {models.Length}, enums {enums.Length}, exceptions {exceptions.Length}");

// ================================================================================================

void EmitTypeHeader(Type t)
{
    sb.AppendLine($"### {t.Name}");
    sb.AppendLine();
    string summary = Summary($"T:{DocType(t)}");
    if (summary.Length > 0)
    {
        sb.AppendLine(summary);
        sb.AppendLine();
    }
    string remarks = Section($"T:{DocType(t)}", "remarks");
    if (remarks.Length > 0)
    {
        sb.AppendLine($"> {remarks.Replace("\n", "\n> ")}");
        sb.AppendLine();
    }
}

void EmitMethod(Type owner, MethodInfo m)
{
    string id = MethodDocId(owner, m);
    sb.AppendLine($"#### `{Signature(m)}`");
    sb.AppendLine();

    string summary = Summary(id);
    if (summary.Length > 0)
    {
        sb.AppendLine(summary);
        sb.AppendLine();
    }

    ParameterInfo[] ps = m.GetParameters();
    if (ps.Length > 0 && docs.TryGetValue(id, out XElement? el))
    {
        var rows = new List<string>();
        foreach (ParameterInfo p in ps)
        {
            XElement? tag = el.Elements("param").FirstOrDefault(x => x.Attribute("name")?.Value == p.Name);
            string text = tag is null ? "" : Flatten(tag);
            rows.Add($"| `{p.Name}` | {Cell(text)} |");
        }
        if (rows.Any(r => !r.EndsWith("|  |", StringComparison.Ordinal)))
        {
            sb.AppendLine("| Parameter | |");
            sb.AppendLine("|---|---|");
            foreach (string r in rows) sb.AppendLine(r);
            sb.AppendLine();
        }
    }

    string returns = Section(id, "returns");
    if (returns.Length > 0)
    {
        sb.AppendLine($"**Returns** — {returns}");
        sb.AppendLine();
    }

    string remarks = Section(id, "remarks");
    if (remarks.Length > 0)
    {
        sb.AppendLine($"> {remarks.Replace("\n", "\n> ")}");
        sb.AppendLine();
    }
}

void EmitPropertyTable(Type t, PropertyInfo[] props)
{
    sb.AppendLine("| Property | Type | |");
    sb.AppendLine("|---|---|---|");
    foreach (PropertyInfo p in props)
    {
        sb.AppendLine($"| `{p.Name}` | `{Pretty(p.PropertyType)}` | {Cell(Summary($"P:{DocType(t)}.{p.Name}"))} |");
    }
    sb.AppendLine();
}

string Summary(string id) => Section(id, "summary");

string Section(string id, string tag)
    => docs.TryGetValue(id, out XElement? el) && el.Element(tag) is XElement e ? Flatten(e) : "";

// Collapses an XML doc element to markdown-ish text, turning <see cref="..."/> into code spans.
string Flatten(XElement el)
{
    var buf = new StringBuilder();
    foreach (XNode node in el.Nodes())
    {
        switch (node)
        {
            case XText txt:
                buf.Append(txt.Value);
                break;
            case XElement e when e.Name == "see" || e.Name == "paramref" || e.Name == "typeparamref":
                string r = e.Attribute("cref")?.Value ?? e.Attribute("name")?.Value ?? e.Value;
                int dot = r.LastIndexOf('.');
                buf.Append('`').Append(dot >= 0 ? r[(dot + 1)..] : r.TrimStart('T', 'M', 'P', 'F', ':')).Append('`');
                break;
            case XElement e when e.Name == "c":
                buf.Append('`').Append(e.Value.Trim()).Append('`');
                break;
            case XElement e when e.Name == "code":
                // Multi-line samples must be fenced, not wrapped in inline backticks.
                string[] codeLines = e.Value.Replace("\r", "").Split('\n');
                int pad = codeLines.Where(l => l.Trim().Length > 0)
                                   .Select(l => l.Length - l.TrimStart().Length)
                                   .DefaultIfEmpty(0).Min();
                string body = string.Join("\n", codeLines.Select(l => l.Length >= pad ? l[pad..] : l.TrimStart())).Trim();
                buf.Append("\n\n```csharp\n").Append(body).Append("\n```\n\n");
                break;
            case XElement e when e.Name == "b":
                buf.Append("**").Append(e.Value.Trim()).Append("**");
                break;
            case XElement e when e.Name == "para":
                buf.Append('\n').Append(Flatten(e).Trim()).Append('\n');
                break;
            case XElement e:
                buf.Append(Flatten(e));
                break;
        }
    }

    // Trim prose lines, but leave anything inside a fenced block alone.
    var outLines = new List<string>();
    bool fenced = false;
    foreach (string raw in buf.ToString().Split('\n'))
    {
        if (raw.TrimStart().StartsWith("```", StringComparison.Ordinal))
        {
            fenced = !fenced;
            outLines.Add(raw.Trim());
            continue;
        }
        outLines.Add(fenced ? raw.TrimEnd() : raw.Trim());
    }

    return string.Join("\n", outLines).Trim();
}

string Cell(string s) => s.Replace("\n", " ").Replace("|", "\\|").Trim();

string Anchor(string name) => name.ToLowerInvariant();

// ---- Signature rendering ----------------------------------------------------------------------

string Signature(MethodInfo m)
{
    byte ctx = NullableContext(m);
    var ps = m.GetParameters().Select(p =>
    {
        string s = $"{PrettyAnnotated(p.ParameterType, NullableFlags(p.GetCustomAttributesData()), ctx)} {p.Name}";
        if (p.HasDefaultValue)
        {
            s += $" = {Literal(p)}";
        }
        return s;
    });

    string ret = PrettyAnnotated(m.ReturnType, NullableFlags(m.ReturnParameter.GetCustomAttributesData()), ctx);
    return $"{ret} {m.Name}({string.Join(", ", ps)})";
}

string Literal(ParameterInfo p) => p.RawDefaultValue switch
{
    // A struct parameter with no explicit value reports null. For CancellationToken that is
    // `default`; for Nullable<T> the source says `null`, and both are equivalent C#.
    // Compared by name: under MetadataLoadContext the loaded Nullable<> is a different Type
    // instance from this process's, so Nullable.GetUnderlyingType always returns null here.
    null => p.ParameterType.IsValueType && !IsNullableOfT(p.ParameterType) ? "default" : "null",
    bool b => b ? "true" : "false",
    string s => $"\"{s}\"",
    var v => Convert.ToString(v, CultureInfo.InvariantCulture) ?? "default",
};

// ---- Nullable reference type annotations ------------------------------------------------------
// Nullability lives in NullableAttribute / NullableContextAttribute rather than in the type
// system, so plain reflection would render `Task<Workflow?>` as `Task<Workflow>`. For a library
// whose annotations are part of its published contract, that would be actively misleading.

byte[] NullableFlags(IList<CustomAttributeData> attrs)
{
    CustomAttributeData? a = attrs.FirstOrDefault(
        x => x.AttributeType.FullName == "System.Runtime.CompilerServices.NullableAttribute");
    if (a is null || a.ConstructorArguments.Count == 0)
    {
        return [];
    }

    CustomAttributeTypedArgument arg = a.ConstructorArguments[0];
    if (arg.Value is IReadOnlyCollection<CustomAttributeTypedArgument> many)
    {
        return many.Select(x => Convert.ToByte(x.Value, CultureInfo.InvariantCulture)).ToArray();
    }

    return [Convert.ToByte(arg.Value, CultureInfo.InvariantCulture)];
}

// The enclosing default, from the method, then its type, then the module.
byte NullableContext(MethodInfo m)
{
    foreach (IList<CustomAttributeData> set in new[]
             {
                 m.GetCustomAttributesData(),
                 m.DeclaringType?.GetCustomAttributesData() ?? [],
                 m.Module.GetCustomAttributesData(),
             })
    {
        CustomAttributeData? a = set.FirstOrDefault(
            x => x.AttributeType.FullName == "System.Runtime.CompilerServices.NullableContextAttribute");
        if (a is not null && a.ConstructorArguments.Count > 0)
        {
            return Convert.ToByte(a.ConstructorArguments[0].Value, CultureInfo.InvariantCulture);
        }
    }

    return 0;
}

string PrettyAnnotated(Type t, byte[] flags, byte ctx)
{
    int i = 0;
    return Walk(t, flags, ctx, ref i);
}

// Flags are a pre-order walk of the type tree. Value types never carry an annotation, so they
// do not consume a flag - Nullable<T> is rendered from the type itself.
string Walk(Type t, byte[] flags, byte ctx, ref int i)
{
    if (t.IsByRef)
    {
        return Walk(t.GetElementType()!, flags, ctx, ref i);
    }

    bool annotatable = !t.IsValueType;
    byte flag = 0;
    if (annotatable)
    {
        flag = i < flags.Length ? flags[i] : ctx;
        i++;
    }

    string rendered;
    if (t.IsArray)
    {
        rendered = Walk(t.GetElementType()!, flags, ctx, ref i) + "[]";
    }
    else if (t.IsGenericType)
    {
        Type def = t.GetGenericTypeDefinition();
        Type[] gargs = t.GetGenericArguments();
        if (def.FullName == "System.Nullable`1")
        {
            int inner = i;
            return Walk(gargs[0], flags, ctx, ref inner) + "?";
        }

        string name = def.Name[..def.Name.IndexOf('`')];
        var parts = new List<string>();
        foreach (Type g in gargs)
        {
            parts.Add(Walk(g, flags, ctx, ref i));
        }
        rendered = $"{name}<{string.Join(", ", parts)}>";
    }
    else
    {
        rendered = Pretty(t);
    }

    return annotatable && flag == 2 ? rendered + "?" : rendered;
}

string Pretty(Type t)
{
    if (t.IsByRef) return Pretty(t.GetElementType()!);
    if (t.IsArray) return Pretty(t.GetElementType()!) + "[]";

    if (t.IsGenericType)
    {
        Type def = t.GetGenericTypeDefinition();
        Type[] args = t.GetGenericArguments();
        if (def.FullName == "System.Nullable`1") return Pretty(args[0]) + "?";
        string name = def.Name[..def.Name.IndexOf('`')];
        return $"{name}<{string.Join(", ", args.Select(Pretty))}>";
    }

    return t.FullName switch
    {
        "System.Boolean" => "bool",
        "System.Int32" => "int",
        "System.Int64" => "long",
        "System.UInt32" => "uint",
        "System.String" => "string",
        "System.Object" => "object",
        "System.Void" => "void",
        _ => t.Name,
    };
}

// ---- XML documentation ids -----------------------------------------------------------------

string DocType(Type t) => (t.FullName ?? t.Name).Replace('+', '.');

string MethodDocId(Type owner, MethodInfo m)
{
    string name = m.Name;
    if (m.IsGenericMethodDefinition)
    {
        name += "``" + m.GetGenericArguments().Length;
    }

    ParameterInfo[] ps = m.GetParameters();
    string args = ps.Length == 0 ? "" : "(" + string.Join(",", ps.Select(p => DocParam(p.ParameterType))) + ")";
    return $"M:{DocType(owner)}.{name}{args}";
}

string DocParam(Type t)
{
    if (t.IsByRef) return DocParam(t.GetElementType()!) + "@";
    if (t.IsArray) return DocParam(t.GetElementType()!) + "[]";
    if (t.IsGenericParameter)
    {
        return (t.DeclaringMethod is not null ? "``" : "`") + t.GenericParameterPosition;
    }
    if (t.IsGenericType)
    {
        Type def = t.GetGenericTypeDefinition();
        string full = (def.FullName ?? def.Name).Replace('+', '.');
        full = full[..full.IndexOf('`')];
        return $"{full}{{{string.Join(",", t.GetGenericArguments().Select(DocParam))}}}";
    }
    return DocType(t);
}

static string RuntimeEnvironment() => Path.GetDirectoryName(typeof(object).Assembly.Location)!;

static bool IsNullableOfT(Type t)
    => t.IsGenericType && t.GetGenericTypeDefinition().FullName == "System.Nullable`1";

// BaseType comparison by name: under MetadataLoadContext the loaded System.Exception is a
// different Type instance from the one in this process, so typeof() comparison would not match.
static bool Inherits(Type t, string fullName)
{
    for (Type? b = t.BaseType; b is not null; b = b.BaseType)
    {
        if (b.FullName == fullName)
        {
            return true;
        }
    }

    return false;
}
