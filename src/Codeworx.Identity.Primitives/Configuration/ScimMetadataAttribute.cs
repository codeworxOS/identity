using System;

#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable SA1649 // File name should match first type name
namespace Codeworx.Identity.Configuration
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ScimRequiredAttribute : Attribute
    {
        public ScimRequiredAttribute(bool isRequired)
        {
            IsRequired = isRequired;
        }

        public bool IsRequired { get; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ScimDescriptionAttribute : Attribute
    {
        public ScimDescriptionAttribute(string description)
        {
            Description = description;
        }

        public string Description { get; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ScimCaseExactAttribute : Attribute
    {
        public ScimCaseExactAttribute(bool caseExact)
        {
            CaseExact = caseExact;
        }

        public bool CaseExact { get; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ScimUniqueAttribute : Attribute
    {
        public ScimUniqueAttribute(bool isUnique)
        {
            IsUnique = isUnique;
        }

        public bool IsUnique { get; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ScimMutabilityAttribute : Attribute
    {
        public ScimMutabilityAttribute(MutabilityType mutability)
        {
            Mutability = mutability;
        }

        public enum MutabilityType
        {
            ReadWrite,
            ReadOnly,
            Immutable,
            WriteOnly,
        }

        public MutabilityType Mutability { get; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ScimReferenceTypesAttribute : Attribute
    {
        public ScimReferenceTypesAttribute(params string[] referenceTypes)
        {
            ReferenceTypes = referenceTypes;
        }

        public string[] ReferenceTypes { get; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ScimCanonicalValuesAttribute : Attribute
    {
        public ScimCanonicalValuesAttribute(params string[] canonicalValues)
        {
            CanonicalValues = canonicalValues;
        }

        public string[] CanonicalValues { get; }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
#pragma warning restore SA1649 // File name should match first type name