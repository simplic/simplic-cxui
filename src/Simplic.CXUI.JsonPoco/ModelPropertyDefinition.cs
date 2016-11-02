using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.CXUI.JsonPoco
{
    /// <summary>
    /// Contains a single property definition
    /// </summary>
    public class ModelPropertyDefinition
    {
        /// <summary>
        /// Gets the field name based on the property name. SampleProperty will be _sampleProperty
        /// </summary>
        internal string Field
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Name))
                {
                    string field = this.Name;

                    // Try to make the first char to lower
                    if (field.Length > 0)
                    {
                        string lower = field[0].ToString().ToLower();
                        field = field.Remove(0, 1);
                        field = field.Insert(0, lower);
                    }

                    return $"_{field}".Trim();
                }

                return "";
            }
        }

        /// <summary>
        /// Gets or sets the name of the property
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of the property
        /// </summary>
        public string Type
        {
            get;
            set;
        }
    }
}
