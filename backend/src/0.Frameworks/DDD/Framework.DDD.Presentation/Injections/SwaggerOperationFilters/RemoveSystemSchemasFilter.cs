using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Framework.DDD.Presentation.Injections.SwaggerOperationFilters;

public class RemoveSystemSchemasFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var schemas = swaggerDoc.Components?.Schemas;
        if (schemas is null)
            return;

        string[] array = new string[36]
        {
            "Assembly", "CallingConventions", "ConstructorInfo", "CustomAttributeData", "CustomAttributeNamedArgument", "CustomAttributeTypedArgument", "EventAttributes", "EventInfo", "FieldAttributes", "FieldInfo",
            "Exception", "ApiException", "GenericParameterAttributes", "ICustomAttributeProvider", "IntPtr", "LayoutKind", "MemberInfo", "MemberTypes", "MethodAttributes", "MethodImplAttributes",
            "MethodInfo", "MethodBase", "Module", "ModuleHandle", "ParameterAttributes", "ParameterInfo", "PropertyAttributes", "PropertyInfo", "RuntimeFieldHandle", "RuntimeMethodHandle",
            "RuntimeTypeHandle", "SecurityRuleSet", "StructLayoutAttribute", "Type", "TypeAttributes", "TypeInfo"
        };
        foreach (string key in array)
        {
            if (schemas.ContainsKey(key))
            {
                schemas.Remove(key);
            }
        }
    }
}
