using Framework.DDD.Presentation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Framework.DDD.Presentation.Injections.SwaggerOperationFilters;

public class ClientContextBodyOperationFilter : IOperationFilter
{
    private static readonly string _get = "GET";

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (!context.MethodInfo.GetCustomAttributes(inherit: true).OfType<HttpGetAttribute>().Any() && 
            !_get.Equals(context.ApiDescription.HttpMethod, StringComparison.OrdinalIgnoreCase))
        {
            Type? type = context.MethodInfo.GetParameters().FirstOrDefault()?.ParameterType;
            if (!(type == null))
            {
                Type typeFromHandle = typeof(ApiRequestWrapper<>);
                Type[] typeArguments = new Type[1] { type };
                Type modelType = typeFromHandle.MakeGenericType(typeArguments);
                operation.RequestBody = new OpenApiRequestBody
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new OpenApiMediaType
                        {
                            Schema = context.SchemaGenerator.GenerateSchema(modelType, context.SchemaRepository)
                        }
                    }
                };
            }
        }
    }
}