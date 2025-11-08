using System.ComponentModel;
using System.Reflection;

namespace CSDL7.MasterService.Shared.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Get description of enum
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum value)
        {
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = (DescriptionAttribute)fieldInfo.GetCustomAttribute(typeof(DescriptionAttribute), false);

            return attribute != null ? attribute.Description : value.ToString();

        }
    }
}
