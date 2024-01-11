using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Filter;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping
{
    public abstract class ResourceMapping<TEntity, TResource, TData> : IResourceMapping<TEntity, TResource, TData>
        where TEntity : class
        where TResource : IScimResource
    {
        private readonly Expression<Func<ScimEntity<TEntity>, TData>> _entityExpression;
        private readonly Expression<Func<TResource, TData>> _resourceExpression;
        private readonly Func<TResource, TData> _resourceValueDelegate;

        public ResourceMapping(
            Expression<Func<ScimEntity<TEntity>, TData>> entityExpression,
            Expression<Func<TResource, TData>> resourceExpression)
        {
            _entityExpression = entityExpression;
            _resourceExpression = resourceExpression;
            _resourceValueDelegate = _resourceExpression.Compile();

            ResourcePath = _resourceExpression.Body.GetPath();
        }

        public string ResourcePath { get; }

        public Expression<Func<TResource, TData>> Resource => _resourceExpression;

        public Expression<Func<ScimEntity<TEntity>, TData>> Entity => _entityExpression;

        public LambdaExpression ResourceExpression => Resource;

        public LambdaExpression EntityExpression => Entity;

        public TData? GetResourceValue(TResource resource)
        {
            try
            {
                return _resourceValueDelegate(resource);
            }
            catch (Exception)
            {
                return default;
            }
        }

        public virtual async Task ToDatabaseAsync(DbContext db, TEntity entity, ISchemaResource resource, Guid providerId)
        {
            if (resource is TResource typed)
            {
                await CopyValueAsync(db, entity, typed, providerId);
            }
        }

        public abstract Task CopyValueAsync(DbContext db, TEntity entity, TResource resource, Guid providerId);

        public abstract Expression<Func<ScimEntity<TEntity>, bool>>? GetFilter(OperationFilterNode operationFilterNode);

        public virtual async IAsyncEnumerable<SchemaInfo> GetSchemaAttributesAsync(DbContext db)
        {
            await Task.Yield();

            foreach (var property in GetMappedProperties(db))
            {
                var paths = new List<SchemaPath>();

                var parent = property.Parent;
                var name = property.Member.GetJsonName();

                paths.Insert(0, new SchemaPath(name, false));

                while (parent is MemberExpression memberExpression)
                {
                    var parentName = memberExpression.Member.GetJsonName();
                    paths.Insert(0, new SchemaPath(parentName, memberExpression.Type.IsEnumerable()));
                    parent = memberExpression.Expression;
                }

                if (parent != Resource.Parameters[0])
                {
                    throw new NotSupportedException("The resource expression does not contain a propper path to the root parameter! e.g. p => value.FirstName instead of p => p.FirstName");
                }

                var memberType = GetMemberType(property.Member);

                var attributes = property.Member.GetCustomAttributes(true).OfType<Attribute>();

                string schemaType = GetSchemaType(attributes, memberType);
                string? description = GetSchemaDescription(attributes, property);
                bool isRequired = GetIsRequired(attributes, property, memberType);
                bool isUnique = GetIsUnique(attributes, property);
                bool caseExact = GetIsCaseExact(attributes, property);
                string mutability = GetMutability(attributes, property);
                var referenceTypes = GetReferenceTypes(attributes, property);
                var canonicalValues = GetCanonicalValues(attributes, property);

                yield return new SchemaInfo(paths, schemaType, typeof(TResource), description, isRequired, isUnique, caseExact, mutability, referenceTypes, canonicalValues);
            }
        }

        protected abstract IEnumerable<MappedPropertyInfo> GetMappedProperties(DbContext db);

        protected virtual string GetSchemaType(IEnumerable<Attribute> attributes, Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;

            var attrib = attributes.OfType<ScimReferenceTypesAttribute>().FirstOrDefault();
            if (attrib != null)
            {
                return "reference";
            }

            if (type == typeof(int) || type == typeof(uint) || type == typeof(ushort) || type == typeof(short) || type == typeof(byte) || type == typeof(sbyte) || type == typeof(long) || type == typeof(ulong))
            {
                return "integer";
            }
            else if (type == typeof(float) || type == typeof(double) || type == typeof(decimal))
            {
                return "decimal";
            }
            else if (type == typeof(bool))
            {
                return "boolean";
            }
            else if (type == typeof(DateTime))
            {
                return "dateTime";
            }

            return "string";
        }

        protected virtual string? GetSchemaDescription(IEnumerable<Attribute> attributes, ResourceMapping<TEntity, TResource, TData>.MappedPropertyInfo property)
        {
            var attrib = attributes.OfType<ScimDescriptionAttribute>().FirstOrDefault();
            if (attrib != null)
            {
                return attrib.Description;
            }

            return null;
        }

        protected virtual bool GetIsRequired(IEnumerable<Attribute> attributes, MappedPropertyInfo property, Type type)
        {
            var attrib = attributes.OfType<ScimRequiredAttribute>().FirstOrDefault();
            if (attrib != null)
            {
                return attrib.IsRequired;
            }

            if (property.Column != null)
            {
                return !property.Column.IsNullable;
            }

            bool canBeNull = !type.IsValueType || Nullable.GetUnderlyingType(type) != null;

            return !canBeNull;
        }

        protected virtual bool GetIsUnique(IEnumerable<Attribute> attributes, MappedPropertyInfo property)
        {
            var attrib = attributes.OfType<ScimUniqueAttribute>().FirstOrDefault();
            if (attrib != null)
            {
                return attrib.IsUnique;
            }

            if (property.Column != null)
            {
                return property.Column.IsUniqueIndex();
            }

            return false;
        }

        protected virtual bool GetIsCaseExact(IEnumerable<Attribute> attributes, MappedPropertyInfo property)
        {
            var attrib = attributes.OfType<ScimCaseExactAttribute>().FirstOrDefault();
            if (attrib != null)
            {
                return attrib.CaseExact;
            }

            return false;
        }

        protected virtual string GetMutability(IEnumerable<Attribute> attributes, MappedPropertyInfo property)
        {
            var attrib = attributes.OfType<ScimMutabilityAttribute>().FirstOrDefault();
            if (attrib != null)
            {
                switch (attrib.Mutability)
                {
                    case ScimMutabilityAttribute.MutabilityType.ReadWrite:
                        return "readWrite";

                    case ScimMutabilityAttribute.MutabilityType.ReadOnly:
                        return "readOnly";

                    case ScimMutabilityAttribute.MutabilityType.Immutable:
                        return "immutable";

                    case ScimMutabilityAttribute.MutabilityType.WriteOnly:
                        return "writeOnly";

                    default:
                        return "readWrite";
                }
            }

            return "readWrite";
        }

        protected virtual List<string>? GetReferenceTypes(IEnumerable<Attribute> attributes, ResourceMapping<TEntity, TResource, TData>.MappedPropertyInfo property)
        {
            var attrib = attributes.OfType<ScimReferenceTypesAttribute>().FirstOrDefault();
            if (attrib != null)
            {
                return attrib.ReferenceTypes.ToList();
            }

            return null;
        }

        protected virtual List<string>? GetCanonicalValues(IEnumerable<Attribute> attributes, ResourceMapping<TEntity, TResource, TData>.MappedPropertyInfo property)
        {
            var attrib = attributes.OfType<ScimCanonicalValuesAttribute>().FirstOrDefault();
            if (attrib != null)
            {
                return attrib.CanonicalValues.ToList();
            }

            return null;
        }

        private Type GetMemberType(MemberInfo info)
        {
            if (info is PropertyInfo propInfo)
            {
                return propInfo.PropertyType;
            }

            throw new NotSupportedException("Unsupported Type, MemberInfo");
        }

        protected class MappedPropertyInfo
        {
            public MappedPropertyInfo(MemberInfo member, IProperty? column, Expression? parent)
            {
                Member = member;
                Column = column;
                Parent = parent;
            }

            public MemberInfo Member { get; }

            public Expression? Parent { get; }

            public IProperty? Column { get; }
        }
    }
}
