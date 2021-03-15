// Copyright (c) János Janka. All rights reserved.

namespace System.ComponentModel.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class TemplateFieldAttribute : Attribute
    {
        private readonly LocalizableString _name;
        private Type _resourceType;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateFieldAttribute"/> class.
        /// </summary>
        public TemplateFieldAttribute()
        {
            _name = new LocalizableString("Name");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateFieldAttribute"/> class.
        /// </summary>
        /// <param name="property">The property name of the current object.</param>
        public TemplateFieldAttribute(string property)
        {
            _name = new LocalizableString("Name");
            Property = property;
        }

        /// <summary>
        /// Gets or sets the expression for getting the property value.
        /// </summary>
        /// <value>
        /// The expression to get the property value.
        /// </value>
        public string Property { get; set; }

        /// <summary>
        /// Gets or sets a value that is used for display in the UI.
        /// </summary>
        /// <value>
        /// A value that is used for display in the UI.
        /// </value>
        public string Name
        {
            get
            {
                return _name.Value;
            }

            set
            {
                if (_name.Value != value)
                {
                    _name.Value = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the type that contains the resources for the System.ComponentModel.DataAnnotations.TemplateFieldAttribute.Name.
        /// </summary>
        /// <value>
        /// The type of the resource that contains the System.ComponentModel.DataAnnotations.TemplateFieldAttribute.Name.
        /// </value>
        public Type ResourceType
        {
            get
            {
                return _resourceType;
            }

            set
            {
                if (_resourceType != value)
                {
                    _resourceType = value;
                    _name.ResourceType = value;
                }
            }
        }

        /// <summary>
        /// Gets the localized name of the property.
        /// </summary>
        /// <returns>The localized property name.</returns>
        public string GetName()
        {
            return _name.GetLocalizableValue();
        }

        /// <summary>
        /// When implemented in a derived class, gets a unique identifier for this <see cref="T:System.Attribute" />.
        /// </summary>
        /// <returns>An <see cref="T:System.Object" /> that is a unique identifier for the attribute.</returns>
        public override object TypeId
        {
            get
            {
                if (Property == null)
                {
                    return "TemplateFieldAttribute";
                }
                return "TemplateFieldAttribute" + Property;
            }
        }
    }
}
