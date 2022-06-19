using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ClientBuilder.DataAnnotations;
using ClientBuilder.Extensions;
using ClientBuilder.Options;
using Microsoft.Extensions.Options;

namespace ClientBuilder.Core.Scanning;

/// <inheritdoc />
public class DescriptionExtractor : IDescriptionExtractor
{
    private readonly ClientBuilderOptions options;

    /// <summary>
    /// Initializes a new instance of the <see cref="DescriptionExtractor"/> class.
    /// </summary>
    /// <param name="optionsAccessor"></param>
    public DescriptionExtractor(IOptions<ClientBuilderOptions> optionsAccessor)
    {
        this.options = optionsAccessor.Value;
    }

    /// <inheritdoc/>
    public TypeDescription ExtractTypeDescription(
        Type type,
        Type parentType = null,
        TypeDescription parentDescription = null)
    {
        try
        {
            if (type == null)
            {
                return new TypeDescription();
            }

            TypeDescription description = new TypeDescription();
            description.IsClass = type.IsClass;
            description.IsInterface = type.IsInterface;
            description.IsCollection = type.GetInterface(nameof(IEnumerable)) != null && type != typeof(string);
            description.IsGenericType = type.GetGenericArguments()?.Any() ?? false;

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
                return parentDescription;
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

            if (!isPrimitiveType && description.IsGenericType && !description.IsCollection)
            {
                description.Name = this.GetGenericTypeClearName(description.Name);
                description.FullName = this.GetGenericTypeClearName(description.FullName);
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
                        propertyDescription.DefaultValue = this.GetDefault(propertyInfo.PropertyType)?.ToString() ?? "null";
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
        catch (Exception)
        {
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
    public ArgumentDescription ExtractArgumentDescription(string name, Type type)
    {
        if (type == null)
        {
            throw new NullReferenceException($"Description argument type for {name} cannot be null");
        }

        return new ArgumentDescription
        {
            Name = name,
            Type = this.ExtractTypeDescription(type),
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
            if (property.Type.IsComplex && property.Type.FullName != classDescription.FullName)
            {
                resultClasses.Add(property.Type);
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

    private string GetGenericTypeClearName(string name) => name.Split('`').FirstOrDefault() ?? name;

    private object GetDefault(Type type) => type.IsValueType ? Activator.CreateInstance(type) : null;
}