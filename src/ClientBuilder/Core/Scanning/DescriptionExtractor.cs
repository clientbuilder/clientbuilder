using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using ClientBuilder.DataAnnotations;
using ClientBuilder.Extensions;
using ClientBuilder.Options;
using Essentials.Extensions;
using Essentials.Functions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ClientBuilder.Core.Scanning;

/// <inheritdoc />
public class DescriptionExtractor : IDescriptionExtractor
{
    private readonly ILogger<DescriptionExtractor> logger;
    private readonly ClientBuilderOptions options;

    /// <summary>
    /// Initializes a new instance of the <see cref="DescriptionExtractor"/> class.
    /// </summary>
    /// <param name="optionsAccessor"></param>
    /// <param name="logger"></param>
    public DescriptionExtractor(
        IOptions<ClientBuilderOptions> optionsAccessor,
        ILogger<DescriptionExtractor> logger)
    {
        this.logger = logger;
        this.options = optionsAccessor.Value;
    }

    /// <inheritdoc/>
    public TypeDescription ExtractTypeDescription(
        Type type,
        Type parentType = null,
        TypeDescription parentDescription = null)
    {
        if (type == null)
        {
            return new TypeDescription
            {
                IsValid = false,
            };
        }

        if (IsProtectedType(type))
        {
            return new TypeDescription
            {
                Name = type.Name,
                FullName = type.FullName,
                SourceType = type,
                IsValid = false,
            };
        }

        this.logger.LogInformation("Extracting type: '{Type}'", type);

        try
        {
            TypeDescription description = new TypeDescription();
            description.SourceType = type;
            description.IsClass = type.IsClass;
            description.IsInterface = type.IsInterface;
            description.IsCollection = type.GetInterface(nameof(IEnumerable)) != null && type != typeof(string);

            if (description.IsCollection)
            {
                type = type.GetGenericArguments().FirstOrDefault() ?? type;
            }

            if (type.IsArray)
            {
                type = type.GetElementType();
            }

            description.IsNullable = Nullable.GetUnderlyingType(type) != null;
            description.IsEnum = type.IsEnum;
            bool isPrimitiveType = false;

            if (type == parentType)
            {
                return parentDescription with
                {
#pragma warning disable SA1101
                    IsCollection = description.IsCollection,
                    IsNullable = description.IsNullable,
#pragma warning restore SA1101
                };
            }

            if (this.options.PrimitiveTypes.ContainsKey(type))
            {
                description.Name = this.options.PrimitiveTypes[type];
                description.FullName = description.Name;
                isPrimitiveType = true;
            }

            if (description.IsNullable && this.options.PrimitiveTypes.ContainsKey(Nullable.GetUnderlyingType(type)))
            {
                description.Name = this.options.PrimitiveTypes[Nullable.GetUnderlyingType(type)];
                description.FullName = description.Name;
                isPrimitiveType = true;
            }

            if (!isPrimitiveType && !description.IsEnum)
            {
                description.IsComplex = true;
                description.IsNullable = true;
                description.Name = type.Name;
                description.FullName = type.FullName;
            }

            if (!isPrimitiveType && type.IsGenericType && !description.IsCollection)
            {
                description.Name = GetGenericTypeClearName(type.Name);
                description.FullName = GetGenericTypeClearName(type.FullName);
                description.GenericTypes = type.GetGenericArguments().Select(x => this.ExtractTypeDescription(x)).ToArray();

                var genericTypeDefinition = type.GetGenericTypeDefinition();
                if (genericTypeDefinition != type)
                {
                    description.GenericTypeDescription = this.ExtractTypeDescription(genericTypeDefinition);
                }
            }

            if (description.IsEnum)
            {
                description.Name = type.Name;
                description.FullName = type.FullName;
                description.EnumValueItems = GetEnumValueItems(type);
                description.EnumValues = new Dictionary<string, int>();
                var enumValues = Enum.GetValues(type);
                foreach (var value in enumValues)
                {
                    description.EnumValues[value.ToString()] = (int)Enum.Parse(type, value.ToString());
                }
            }

            if (description.IsComplex)
            {
                var properties = type.GetProperties();
                foreach (var propertyInfo in properties)
                {
                    if (!propertyInfo.IsPropertyStatic() && !propertyInfo.HasCustomAttribute<ExcludeElementAttribute>())
                    {
                        PropertyDescription propertyDescription = new PropertyDescription();
                        propertyDescription.Name = propertyInfo.Name;
                        propertyDescription.ReadOnly = propertyInfo.HasCustomAttribute<ReadOnlyAttribute>();
                        propertyDescription.Type = this.ExtractTypeDescription(propertyInfo.PropertyType, type, description);
                        propertyDescription.DefaultValue = GetDefault(propertyInfo.PropertyType)?.ToString() ?? "null";
                        description.Properties.Add(propertyDescription);
                    }
                }

                if (type.BaseType != null && type.BaseType != typeof(object))
                {
                    description.BaseType = this.ExtractTypeDescription(type.BaseType);
                }
            }

            return description;
        }
        catch (Exception ex)
        {
            this.logger.LogDebug("Problematic type: {Type}", type?.ToString());
            this.logger.LogError(ex, "Error during type description extraction");
            throw new ArgumentException($"Invalid type extraction for type '{type.FullName}'");
        }
    }

    /// <inheritdoc/>
    public ResponseDescription ExtractResponseDescription(Type type)
    {
        if (type == null)
        {
            return new ResponseDescription { Void = true };
        }

        return new ResponseDescription
        {
            Type = this.ExtractTypeDescription(type),
        };
    }

    /// <inheritdoc/>
    public ArgumentDescription ExtractArgumentDescription(string name, Type type, bool hardcodeAsComplex)
    {
        if (type == null)
        {
            throw new NullReferenceException($"Description argument type for {name} cannot be null");
        }

        var typeDescription = this.ExtractTypeDescription(type);
        if (hardcodeAsComplex)
        {
            typeDescription.IsComplex = true;
            typeDescription.Hardcoded = true;
        }

        return new ArgumentDescription
        {
            Name = name,
            Type = typeDescription,
        };
    }

    /// <inheritdoc/>
    public IEnumerable<TypeDescription> ExtractUniqueClassesFromClasses(IEnumerable<TypeDescription> classes)
    {
        var tempClasses = new List<TypeDescription>();
        foreach (var classItem in classes)
        {
            tempClasses.Add(classItem);
            tempClasses.AddRange(this.ExtractInnerClassDescriptions(classItem));
        }

        var resultClasses = new List<TypeDescription>();
        foreach (var tempClass in tempClasses)
        {
            if (resultClasses.All(x => x.FullName != tempClass.FullName))
            {
                resultClasses.Add(tempClass);
            }
        }

        return resultClasses;
    }

    /// <inheritdoc/>
    public IEnumerable<TypeDescription> ExtractInnerClassDescriptions(TypeDescription classDescription)
    {
        var resultClasses = new List<TypeDescription>();
        var classProperties = classDescription.Properties;
        foreach (var property in classProperties)
        {
            if (property.Type.IsComplex && !property.Type.Hardcoded && property.Type.FullName != classDescription.FullName)
            {
                resultClasses.Add(property.Type);
                if (property.Type.BaseType != null)
                {
                    resultClasses.Add(property.Type.BaseType);
                }

                resultClasses.AddRange(this.ExtractInnerClassDescriptions(property.Type));
            }
        }

        return resultClasses;
    }

    /// <inheritdoc/>
    public IEnumerable<TypeDescription> ExtractUniqueEnumsFromClasses(IEnumerable<TypeDescription> classes)
    {
        var tempClasses = new List<TypeDescription>();
        foreach (var classItem in classes)
        {
            tempClasses.Add(classItem);
            tempClasses.AddRange(this.ExtractInnerClassDescriptions(classItem));
            if (classItem.IsGenericType)
            {
                tempClasses.AddRange(classItem.GenericTypes);
            }
        }

        var resultEnumsTypes = new List<TypeDescription>();

        foreach (var tempClass in tempClasses)
        {
            foreach (var tempClassProperty in tempClass.Properties)
            {
                if (tempClassProperty.Type.IsEnum && resultEnumsTypes.All(x => x.FullName != tempClassProperty.Type.FullName))
                {
                    resultEnumsTypes.Add(tempClassProperty.Type);
                }
            }
        }

        return resultEnumsTypes;
    }

    private static bool IsProtectedType(Type type)
    {
        var reflectionNamespace = "System.Reflection";
        if (type.Namespace?.StartsWith(reflectionNamespace) ?? false)
        {
            return true;
        }

        if (type.BaseType?.Namespace?.StartsWith(reflectionNamespace) ?? false)
        {
            return true;
        }

        if (type.HasInterface<IEnumerable>())
        {
            var targetType = type.GetGenericArguments().FirstOrDefault();
            if (targetType == null && type.IsArray)
            {
                targetType = type.GetElementType();
            }

            if (targetType?.Namespace?.StartsWith(reflectionNamespace) ?? false)
            {
                return true;
            }

            if (targetType?.BaseType?.Namespace?.StartsWith(reflectionNamespace) ?? false)
            {
                return true;
            }
        }

        return false;
    }

    private static string GetGenericTypeClearName(string name) => name.Split('`').FirstOrDefault() ?? name;

    private static object GetDefault(Type type) => type.IsValueType ? Activator.CreateInstance(type) : null;

    private static IEnumerable<EnumValueItem> GetEnumValueItems(Type enumType)
    {
        try
        {
            var actualEnumType = Nullable.GetUnderlyingType(enumType) != null ? Nullable.GetUnderlyingType(enumType) : enumType;

            if (actualEnumType.BaseType != typeof(Enum))
            {
                return null;
            }

            List<EnumValueItem> result = new List<EnumValueItem>();

            var enumValues = actualEnumType.GetEnumValues();
            foreach (var value in enumValues)
            {
                int enumValue = (int)Enum.Parse(actualEnumType, value.ToString());
                var memberInfo = actualEnumType.GetMember(actualEnumType.GetEnumName(value));
                var keyAttribute = memberInfo.First().GetCustomAttribute<EnumKeyAttribute>();
                var nameAttribute = memberInfo.First().GetCustomAttribute<EnumNameAttribute>();

                string name = value.ToString();
                string originalName = name;
                string friendlyName = StringFunctions.SplitStringByCapitalLetters(originalName);
                string key = StringFunctions.ConvertToKey(actualEnumType.Name + name);

                if (keyAttribute != null)
                {
                    key = keyAttribute.Key;
                }

                name = nameAttribute != null ? nameAttribute.Name : friendlyName;

                result.Add(new EnumValueItem
                {
                    Value = enumValue,
                    Name = name,
                    OriginalName = originalName,
                    Key = key,
                });
            }

            return result;
        }
        catch (Exception)
        {
            return null;
        }
    }
}