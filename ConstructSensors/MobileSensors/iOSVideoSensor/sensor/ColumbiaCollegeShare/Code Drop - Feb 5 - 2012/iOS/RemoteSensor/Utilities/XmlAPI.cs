using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

namespace RemoteSensor
{
    public static class XmlAPI
    {
        public static XmlTextWriter CreateWriter()
        {
            MemoryStream ms = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(ms, System.Text.Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            return writer;
        }

        public static string FlushWriter(XmlTextWriter writer)
        {
            writer.Flush();
            writer.BaseStream.Position = 0;
            StreamReader reader = new StreamReader(writer.BaseStream);
            string xml = reader.ReadToEnd();
            return xml;
        }
    }}

