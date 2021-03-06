﻿using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Web.Http.Controllers;
using Flatwhite.Provider;

namespace Flatwhite.WebApi
{
    /// <summary>
    /// A <see cref="IContextProvider" /> for webapi that puts request data to the context dictionary
    /// </summary>
    public class WebApiContextProvider : IContextProvider
    {
        private readonly HttpActionContext _httpActionContext;

        /// <summary>
        /// Initializes an instance of <see cref="WebApiContextProvider" />
        /// </summary>
        /// <param name="httpActionContext"></param>
        public WebApiContextProvider(HttpActionContext httpActionContext)
        {
            _httpActionContext = httpActionContext;
        }

        /// <summary>
        /// Create the context from request
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, object> GetContext()
        {
            NameValueCollection query = HttpUtility.ParseQueryString(_httpActionContext.Request.RequestUri.Query);
            var result = new Dictionary<string, object>
            {
                {WebApiExtensions.__webApi, true},
                {"headers", _httpActionContext.Request.Headers},
                {"method", _httpActionContext.Request.Method},
                {"requestUri", _httpActionContext.Request.RequestUri},
                {"query", query.ToDictionary()}
            };
            return result;
        }
    }
}