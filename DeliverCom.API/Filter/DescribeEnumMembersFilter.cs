using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

#pragma warning disable 1591

namespace DeliverCom.API.Filter
{
    public class DescribeEnumMembersFilter : ISchemaFilter
    {
        private readonly XDocument _xmlComments;

        public DescribeEnumMembersFilter(string xmlPath)
        {
            if (File.Exists(xmlPath)) _xmlComments = XDocument.Load(xmlPath);
        }

        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var enumType = context.Type;

            if (!enumType.IsEnum) return;

            var sb = new StringBuilder(schema.Description);

            sb.AppendLine("<p>Values:</p>");
            sb.AppendLine("<ul>");

            var enumValues = Enum.GetValues(enumType);

            for (var i = 0; i < enumValues.Length; i++)
            {
                var description = GetDescription(enumType, enumValues.GetValue(i));
                sb.AppendLine(
                    $"<li>{(int)enumValues.GetValue(i)} - <b>{description}</b></li>");
            }

            sb.AppendLine("</ul>");

            schema.Description = sb.ToString();
        }

        private string GetDescription(Type type, object value)
        {
            var oFieldInfo = type.GetField(value.ToString());
            var description = oFieldInfo.GetCustomAttribute<DescriptionAttribute>()?.Description;
            return description ?? value.ToString();
        }
    }
}