using Microsoft.Extensions.Logging;
using Shurjopay.Models;
using Shurjopay.Plugin.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Shurjopay.Plugin
{
    public static class JsonHelper
    {
        /// <summary>
        /// Convert Class to Json object (string)
        /// </summary>
        /// <typeparam name="TClass">Class type to be serialized</typeparam>
        /// <param name="data">Class to serialize</param>
        /// <param name="isEmptyToNull">true = return null if empty; 
        /// false empty Json object</param>
        /// <param name="options">JsonSerializer options</param>
        /// <returns>Json encoded string</returns>
        public static string FromClass<TClass>
        (TClass data, bool isEmptyToNull = false, JsonSerializerOptions? options = null)
            where TClass : class
        {
            string response = string.Empty;
            try
            {

                if (!EqualityComparer<TClass>.Default.Equals(data))
                    response = JsonSerializer.Serialize(data, options: options);

                return isEmptyToNull ? response == "{}" ? "null" : response : response;
            }
            catch(JsonException ex)
            {
                
                throw new ShurjopayException("Cannot Serialize Json Response from Shurjopay", ex);
            }
        }

        /// <summary>
        /// Convert a Json object (string) to a class
        /// </summary>
        /// <typeparam name="TClass">Class type to be deserialized into</typeparam>
        /// <param name="data">Json string to be deserialized</param>
        /// <param name="options">JsonSerializer options</param>
        /// <returns>Deserialized class of TClass</returns>
        public static TClass? ToClass<TClass>(string data,
                              JsonSerializerOptions? options = null)
            where TClass : class
        {
            TClass? response = default(TClass);
            try
            {
                return string.IsNullOrEmpty(data)
                 ? response
                 : JsonSerializer.Deserialize<TClass>(data, options ?? null);

            } catch (JsonException ex)
            {
                throw new ShurjopayException("Cannot Deserialize the Json Response from Shurjopay",ex);
            }       
        }

    }
}
