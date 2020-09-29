using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Dka.Net5.TestingJSchema.Extensions
{
    public static class JsonExtensions
    {
        public static List<JToken> FindJTokensByPropertyName(this JToken startJToken, string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return new List<JToken>();
            }
            
            var matches = new List<JToken>();
            FindJTokensByPropertyName(startJToken, propertyName, matches);
            return matches;
        }

        public static List<JToken> FindJTokensByPropertyValue(this JToken startJToken, object propertyValue)
        {
            if (propertyValue == null)
            {
                return new List<JToken>();
            }
            
            var matches = new List<JToken>();
            FindJTokensByPropertyValue(startJToken, propertyValue, matches);
            return matches;
        }
        
        private static void FindJTokensByPropertyName(JToken startJToken, string propertyName, ICollection<JToken> foundMatches)
        {
            switch (startJToken.Type)
            {
                case JTokenType.Object:
                {
                    foreach (var jProperty in startJToken.Children<JProperty>())
                    {
                        if (jProperty.Name == propertyName)
                        {
                            foundMatches.Add(jProperty);
                        }
                        
                        FindJTokensByPropertyName(jProperty.Value, propertyName, foundMatches);
                    }

                    break;
                }
                
                case JTokenType.Array:
                {
                    foreach (var jToken in startJToken.Children())
                    {
                        FindJTokensByPropertyName(jToken, propertyName, foundMatches);
                    }

                    break;
                }
            }
        }
        
        private static void FindJTokensByPropertyValue(JToken startJToken, object propertyValue, ICollection<JToken> foundMatches)
        {
            switch (startJToken.Type)
            {
                case JTokenType.Object:
                {
                    foreach (var jProperty in startJToken.Children<JProperty>())
                    {
                        FindJTokensByPropertyValue(jProperty.Value, propertyValue, foundMatches);
                    }

                    break;
                }
                
                case JTokenType.Array:
                {
                    foreach (var jToken in startJToken.Children())
                    {
                        FindJTokensByPropertyValue(jToken, propertyValue, foundMatches);
                    }

                    break;
                }

                case JTokenType.String:
                case JTokenType.Float:
                case JTokenType.Guid:
                case JTokenType.Integer:
                case JTokenType.Boolean:    
                case JTokenType.Uri:
                {
                    if (propertyValue.ToString().Equals(startJToken.Value<string>()))
                    {
                        foundMatches.Add(startJToken);
                    }
                    
                    break;
                }

                case JTokenType.Date:
                {
                    if (propertyValue is DateTime propertyValueAsDateTime && propertyValueAsDateTime.CompareTo(startJToken.Value<DateTime>()) == 0)
                    {
                        foundMatches.Add(startJToken);
                    }

                    break;
                }

                case JTokenType.TimeSpan:
                {
                    if (propertyValue is TimeSpan propertyValueAsTimeSpan && propertyValueAsTimeSpan.CompareTo(startJToken.Value<TimeSpan>()) == 0)
                    {
                        foundMatches.Add(startJToken);
                    }

                    break;
                }
            }
        }        
    }
}