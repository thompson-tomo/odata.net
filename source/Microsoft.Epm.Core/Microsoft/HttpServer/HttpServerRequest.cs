﻿namespace Microsoft.HttpServer
{
    using System.Collections.Generic;
    using System.IO;

    public sealed class HttpServerRequest
    {
        public string HttpMethod { get; set; } //// TODO no setters

        public string Url { get; set; }

        public IEnumerable<string> Headers { get; set; }

        public Stream Body { get; set; }
    }
}