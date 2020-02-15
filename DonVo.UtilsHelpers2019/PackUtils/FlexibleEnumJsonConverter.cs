﻿using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;

namespace PackUtils
{
    /// <summary>
    ///  Flexible enum converter for newtonsoft
    /// </summary>
    public class FlexibleEnumJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            Type type = IsNullableType(objectType)
                ? Nullable.GetUnderlyingType(objectType)
                : objectType;

            return type.GetTypeInfo().IsEnum;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            bool isNullable = IsNullableType(objectType);
            Type enumType = isNullable
                ? Nullable.GetUnderlyingType(objectType)
                : objectType;

            string[] names = Enum.GetNames(enumType);

            if (reader.TokenType == JsonToken.String)
            {
                string enumText = reader.Value.ToString();

                if (!string.IsNullOrEmpty(enumText))
                {
                    string match = names
                        .Where(n => string.Equals(n, enumText, StringComparison.OrdinalIgnoreCase))
                        .FirstOrDefault();

                    if (match != null)
                    {
                        return Enum.Parse(enumType, match);
                    }
                }
            }
            else if (reader.TokenType == JsonToken.Integer)
            {
                int enumVal = Convert.ToInt32(reader.Value);
                int[] values = (int[])Enum.GetValues(enumType);
                if (values.Contains(enumVal))
                {
                    return Enum.Parse(enumType, enumVal.ToString());
                }
            }

            if (!isNullable)
            {
                string defaultName = names
                    .Where(n => string.Equals(n, "Undefined", StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();

                if (defaultName == null)
                {
                    defaultName = names.First();
                }

                return Enum.Parse(enumType, defaultName);
            }

            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        private bool IsNullableType(Type t)
        {
            return (t.GetTypeInfo().IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }
    }
}
