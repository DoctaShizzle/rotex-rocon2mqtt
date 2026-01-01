using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace RoconMqtt;

public sealed class LinuxOperationParameterProcessor : IOperationProcessor
{
    private readonly IOperationProcessor _inner;
    private readonly ILogger<LinuxOperationParameterProcessor> _log;

    public LinuxOperationParameterProcessor(
        IOperationProcessor inner,
        ILogger<LinuxOperationParameterProcessor> log)
    {
        _inner = inner;
        _log = log;
    }

    public bool Process(OperationProcessorContext context)
    {
        try
        {
            return _inner.Process(context);
        }
        catch (Exception ex)
        {
            if (!ex.Message.Contains("Property Get method was not found"))
                throw;

            _log.LogWarning(
                ex,
                "NSwag ARM64 metadata bug: skipping required-inference for {OperationId}.",
                context.OperationDescription.Operation.OperationId
            );

            // 1) Make sure we don't mark anything as required (play it safe)
            foreach (var p in context.OperationDescription.Operation.Parameters)
                p.IsRequired = false;

            // 2) If the request body is missing because AddBodyParameter() blew up,
            //    reconstruct a basic body schema so Swagger UI still shows it.
            if (context.OperationDescription.Operation.RequestBody == null)
            {
                var method = context.MethodInfo;
                if (method != null)
                {
                    // Heuristic: pick the first non-primitive, non-string parameter as body
                    var bodyParam = method
                        .GetParameters()
                        .FirstOrDefault(p =>
                            !p.ParameterType.IsPrimitive &&
                            p.ParameterType != typeof(string));

                    if (bodyParam != null)
                    {
                        var schema = context.SchemaGenerator.Generate(
                            bodyParam.ParameterType,
                            context.SchemaResolver
                        );

                        context.OperationDescription.Operation.RequestBody = new OpenApiRequestBody
                        {
                            IsRequired = true,
                            Content =
                            {
                                ["application/json"] = new OpenApiMediaType
                                {
                                    Schema = schema
                                }
                            }
                        };

                        _log.LogInformation(
                            "Reconstructed request body for {OperationId} using parameter {ParamName} ({ParamType}).",
                            context.OperationDescription.Operation.OperationId,
                            bodyParam.Name,
                            bodyParam.ParameterType.FullName
                        );
                    }
                    else
                    {
                        _log.LogWarning(
                            "Could not infer body parameter for {OperationId}; request body will be missing.",
                            context.OperationDescription.Operation.OperationId
                        );
                    }
                }
            }

            // We handled the ARM64 bug and patched the operation; continue generation.
            return true;
        }
    }
}
