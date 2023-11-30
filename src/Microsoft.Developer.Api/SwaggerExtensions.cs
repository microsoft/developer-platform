// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Developer.Azure;
using Microsoft.Developer.Data;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Microsoft.Developer.Api;

internal static class SwaggerExtensions
{
    public class DeveloperApiSwaggerOptions
    {
        public ICollection<Type> OneOf { get; } = new HashSet<Type>();
    }

    public static IDeveloperPlatformBuilder AddOpenApi(this IDeveloperPlatformBuilder builder, IConfiguration config, Action<DeveloperApiSwaggerOptions> configure)
    {
        const string version = "v1";
        const string documentName = version;

        builder.Services.AddOptions<DeveloperApiSwaggerOptions>()
            .Configure(configure);
        builder.Services.AddOptions<SwaggerGenOptions>()
            .Configure<IOptions<DeveloperApiSwaggerOptions>>((swagger, polymorphic) =>
            {
                swagger.UseOneOfForPolymorphism();
                swagger.SelectSubTypesUsing(SelectSubTypes);
                swagger.DocumentFilter<IgnorePropertiesBase>();

                IEnumerable<Type> SelectSubTypes(Type type)
                {
                    foreach (var item in polymorphic.Value.OneOf)
                    {
                        if (type.IsAssignableFrom(item))
                        {
                            yield return item;
                        }
                    }
                }
            });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SchemaFilter<IgnorePropertiesBase>();
            options.SchemaFilter<DescribeEnumMembers>();
            options.SchemaFilter<RequiredFilter>();
            options.SchemaFilter<DetailsFilter>();
            options.DocumentFilter<DetailsFilter>();

            options.SwaggerDoc(documentName, new OpenApiInfo
            {
                Version = version,
                Title = "Developer Platform API",
                Description = "API for working with the Developer Platform.",
                Contact = new OpenApiContact
                {
                    Url = new Uri("https://github.com/microsoft/developer-platform/issues/new"),
                    Email = @"colbyw@microsoft.com",
                    Name = "Platform Engineering"
                },
                License = new OpenApiLicense
                {
                    Name = "The Developer Platfom is licensed under the MIT License",
                    Url = new Uri("https://github.com/microsoft/developer-platform/blob/main/LICENSE")
                }
            });
        });

        builder.Services.AddOptions<SwaggerGenOptions>()
            .Configure<IOptions<AzureAdOptions>>((options, azure) =>
            {
                const string OAuthSchema = "AADToken";

                var scopes = new Dictionary<string, string>
            {
                { $"api://{azure.Value.ClientId}/.default", "Access to the API project" }
            };

                options.AddSecurityDefinition(OAuthSchema, new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows()
                    {
                        Implicit = new OpenApiOAuthFlow()
                        {
                            AuthorizationUrl = new Uri($"https://login.microsoftonline.com/{azure.Value.TenantId}/oauth2/v2.0/authorize"),
                            Scopes = scopes,
                        }
                    }
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = OAuthSchema
                        },
                        Scheme = OAuthSchema,
                        Name = OAuthSchema,
                        In = ParameterLocation.Header
                    }, scopes.Keys.ToList()
                }
            });
            });

        builder.Services.AddOptions<SwaggerOptions>()
            .Configure(options => options.RouteTemplate = "openapi/{documentName}/openapi.yaml");

        builder.Services.AddOptions<SwaggerUIOptions>()
            .Configure(options =>
            {
                options.DocumentTitle = "Developer Platform API";

                options.DisplayRequestDuration();
                options.EnableDeepLinking();
                options.EnableFilter();
                options.EnableTryItOutByDefault();
                options.ShowCommonExtensions();
                options.DisplayOperationId();
                options.SwaggerEndpoint($"{version}/openapi.yaml", documentName);
                options.RoutePrefix = "openapi";

                if (config["Website:ClientId"] is { } clientId)
                {
                    options.OAuthClientId(clientId);
                }
            });

        return builder;
    }

    private class RequiredFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            foreach (var item in context.Type.GetProperties())
            {
                if (item.GetCustomAttribute<RequiredMemberAttribute>() is { })
                {
                    if (schema.Properties.FirstOrDefault(p => string.Equals(p.Key, item.Name, StringComparison.OrdinalIgnoreCase)) is { Key: { } key })
                    {
                        schema.Required.Add(key);
                    }
                }
            }
        }
    }

    private class DetailsFilter : ISchemaFilter, IDocumentFilter
    {
        private const string DisplayAsStringSentinel = "__displayAsString";

        private static readonly Action<OpenApiSchema, Context, object>[] processors =
        [
            HandleDisplayAs,
            IgnoreDataMember,
            HandleDescription,
            HandleExample,
            HandleMinLength
        ];

        private sealed class Context(object parent)
        {
            public object Initial => parent;

            public bool IsIgnored { get; set; }
        }

        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            Run(schema, context.Type.GetCustomAttributes(true), new(context.Type), context.Type);

            foreach (var (propertyName, propertySchema) in schema.Properties)
            {
                if (context.Type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) is { } typeProperty)
                {
                    var propertyContext = new Context(typeProperty);
                    Run(propertySchema, typeProperty.GetCustomAttributes(true), propertyContext, typeProperty);

                    if (propertyContext.IsIgnored)
                    {
                        schema.Properties.Remove(propertyName);
                    }
                }
            }
        }

        private static void Run(OpenApiSchema schema, object[] attributes, Context context, object state)
        {
            foreach (var processor in processors)
            {
                processor(schema, context, state);

                foreach (var obj in attributes)
                {
                    if (obj is Attribute attribute)
                    {
                        processor(schema, context, attribute);
                    }
                }
            }
        }

        private static void IgnoreDataMember(OpenApiSchema schema, Context context, object state)
        {
            if (state is IgnoreDataMemberAttribute)
            {
                context.IsIgnored = true;
            }
        }

        private static Type UnfoldListType(Type type)
        {
            if (type.IsGenericType && type.GetGenericArguments() is [{ } innerType])
            {
                var generic = typeof(IEnumerable<>).MakeGenericType(innerType);
                if (type.IsAssignableTo(generic))
                {
                    return innerType;
                }
            }

            return type;
        }
        private static void HandleDisplayAs(OpenApiSchema schema, Context context, object state)
        {
            if (context.Initial is Type && state is DisplayAsStringAttribute { OnlyString: true })
            {
                schema.Type = DisplayAsStringSentinel;
            }
            if (state is PropertyInfo property)
            {
                var propertyType = UnfoldListType(property.PropertyType);

                if (propertyType.GetCustomAttribute<DisplayAsStringAttribute>() is { } display)
                {
                    if (display.OnlyString)
                    {
                        PopulateStringSchema(schema, display);
                        Run(schema, propertyType.GetCustomAttributes(true), context, schema.Type);
                    }
                    else
                    {
                        var stringSchema = new OpenApiSchema();

                        PopulateStringSchema(stringSchema, display);
                        Run(stringSchema, propertyType.GetCustomAttributes(true), context, schema.Type);

                        var original = new OpenApiSchema
                        {
                            Reference = schema.Reference ?? schema.Items.Reference
                        };

                        if (schema.Type == "array")
                        {
                            schema.Items.Reference = null;
                            schema.Items.OneOf.Add(original);
                            schema.Items.OneOf.Add(stringSchema);
                        }
                        else
                        {
                            schema.Reference = null;
                            schema.OneOf.Add(original);
                            schema.OneOf.Add(stringSchema);
                        }
                    }
                }
            }

            static OpenApiSchema PopulateStringSchema(OpenApiSchema schema, DisplayAsStringAttribute display)
            {
                schema.Reference = null;
                schema.Type = "string";
                schema.MinLength = display.GetMinLength();
                schema.MaxLength = display.GetMaxLength();
                schema.Pattern = display.RegularExpression;

                if (display.DefaultValue is { } defaultValue)
                {
                    schema.Default = new OpenApiString(defaultValue);
                }

                return schema;
            }
        }

        private static void HandleMinLength(OpenApiSchema schema, Context context, object state)
        {
            if (schema.MinLength is > 0)
            {
                schema.Nullable = false;
            }
        }

        private static void HandleExample(OpenApiSchema schema, Context context, object state)
        {
            if (state is ExampleAttribute { Example: { } example })
            {
                schema.Example = new OpenApiString(example);
            }
        }

        private static void HandleDescription(OpenApiSchema schema, Context context, object state)
        {
            if (state is DescriptionAttribute { Description: { } description })
            {
                schema.Description = description;
            }
        }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var (name, item) in swaggerDoc.Components.Schemas)
            {
                if (item.Type == DisplayAsStringSentinel)
                {
                    swaggerDoc.Components.Schemas.Remove(name);
                }
            }
        }
    }

    private class DescribeEnumMembers : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (!context.Type.IsEnum)
            {
                return;
            }

            schema.Enum = Enum.GetNames(context.Type)
                .Select(item => new OpenApiString(item))
                .Cast<IOpenApiAny>()
                .ToList();

            schema.Type = "string";
            schema.Format = null;
        }
    }

    private class IgnorePropertiesBase : ISchemaFilter, IDocumentFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type.IsAssignableTo(typeof(PropertiesBase)))
            {
                schema.Properties.Remove(nameof(PropertiesBase.Properties).ToLowerInvariant());
                schema.AdditionalPropertiesAllowed = true;
            }
        }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Components.Schemas.Remove("StringObjectKeyValuePair");
        }
    }
}
