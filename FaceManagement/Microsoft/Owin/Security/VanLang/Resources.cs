using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Microsoft.Owin.Security.VanLang
{
    public class Resources
    {
        public const string Exception_MissingId = "The user does not have an id.";
        public const string Exception_OptionMustBeProvided = "The '{0}' option must be provided.";
        public const string Exception_ValidatorHandlerMismatch = "An ICertificateValidator cannot be specified at the same time as an HttpMessageHandler unless it is a WebRequestHandler.";
    }
}