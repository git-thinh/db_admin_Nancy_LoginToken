namespace Nancy
{
    using System;
    using System.Linq;
    using System.IO;

    using Extensions;
    using Nancy.Responses;

    public class ByteArrayResponse : Response
    {
        /// <summary>
        /// Byte array response
        /// </summary>
        /// <param name="body">Byte array to be the body of the response</param>
        /// <param name="contentType">Content type to use</param>
        public ByteArrayResponse(byte[] body, string contentType = null)
        {
            this.ContentType = contentType ?? "application/octet-stream";

            this.Contents = stream =>
            {
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(body);
                }
            };
        }
    }

    public static class Extensions22
    {
        public static Response FromByteArray(this IResponseFormatter formatter, byte[] body, string contentType = null)
        {
            return new ByteArrayResponse(body, contentType);
        }
    }

    public static class FormatterExtensions
    {
        private static ISerializer jsonSerializer;

        private static ISerializer xmlSerializer;

        public static Response AsFile(this IResponseFormatter formatter, string applicationRelativeFilePath, string contentType)
        {
            return new GenericFileResponse(applicationRelativeFilePath, contentType);
        }

        public static Response AsFile(this IResponseFormatter formatter, string applicationRelativeFilePath)
        {
            return new GenericFileResponse(applicationRelativeFilePath);
        }

        public static Response AsText(this IResponseFormatter formatter, string contents, string contentType)
        {
            return new TextResponse(contents, contentType);
        }

        public static Response AsText(this IResponseFormatter formatter, string contents)
        {
            return new TextResponse(contents);
        }

        public static Response AsHTML(this IResponseFormatter formatter, string contents)
        {
            var o1 = (Response)contents;
            o1.StatusCode = Nancy.HttpStatusCode.OK;
            o1.ContentType = "text/html";
            return o1;
        }



        public static Response AsImage(this IResponseFormatter formatter, string applicationRelativeFilePath)
        {
            return AsFile(formatter, applicationRelativeFilePath);
        }

        public static Response AsJson<TModel>(this IResponseFormatter formatter, TModel model, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var serializer = jsonSerializer ?? (jsonSerializer = formatter.Serializers.FirstOrDefault(s => s.CanSerialize("application/json")));

            var r = new JsonResponse<TModel>(model, serializer);
        	r.StatusCode = statusCode;

        	return r;
        }

        public static Response AsRedirect(this IResponseFormatter formatter, string location, Nancy.Responses.RedirectResponse.RedirectType type = RedirectResponse.RedirectType.SeeOther)
        {
            return new RedirectResponse(formatter.Context.ToFullPath(location), type);
        }

        public static Response AsXml<TModel>(this IResponseFormatter formatter, TModel model)
        {
            var serializer = xmlSerializer ?? (xmlSerializer = formatter.Serializers.FirstOrDefault(s => s.CanSerialize("application/xml")));

            return new XmlResponse<TModel>(model, serializer);
        }

        public static Response FromStream(this IResponseFormatter formatter, Stream stream, string contentType)
        {
            return new StreamResponse(() => stream, contentType);
        }

        public static Response FromStream(this IResponseFormatter formatter, Func<Stream> streamDelegate, string contentType)
        {
            return new StreamResponse(streamDelegate, contentType);
        }
    }
}