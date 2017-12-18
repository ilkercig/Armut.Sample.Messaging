using Armut.Sample.Messaging.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Armut.Sample.Messaging
{
    internal static class ResponseFactory 
    {
        public static ResponseBase CreateResponse(string error)
        {
            var response = new ResponseBase() { };
            response.AddErrorMessages(error);
            return response;
        }

        public static ResponseBase CreateResponse(ModelStateDictionary modelState)
        {
            var response = new ResponseBase();
            response.AddErrorMessages(modelState);
            return response;
        }

        public static Response<T> CreateResponse<T>(T result)
        {
            var response = new Response<T>();
            response.Result = result;
            return response;
        }
    }
}
