using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;

// This class is needed to be able to return static files from the WebAPI 2 self-host infrastructure.
// It will return the index.html contents to the browser.
public class HtmlMediaFormatter : BufferedMediaTypeFormatter
{
    public HtmlMediaFormatter()
    {
        SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
        SupportedEncodings.Add(new UTF8Encoding(false));
    }

    public override bool CanReadType(Type type)
    {
        return false;
    }

    public override bool CanWriteType(Type type)
    {
        return typeof(string) == type ? true : false;
    }

    public override void WriteToStream(Type type, object value, Stream writeStream, HttpContent content)
    {
        var effectiveEncoding = SelectCharacterEncoding(content.Headers);

        using (var writer = new StreamWriter(writeStream, effectiveEncoding))
        {
            writer.Write(value);
        }
    }
}