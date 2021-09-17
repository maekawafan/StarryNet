using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;
using MessagePack.Resolvers;

namespace StarryNet.StarryLibrary
{
    public static class MessagePackUtility
    {
        public static void Initialize(IFormatterResolver resolver)
        {
            var option = MessagePackSerializerOptions.Standard.WithResolver(resolver).WithCompression(MessagePackCompression.Lz4BlockArray);
            MessagePackSerializer.DefaultOptions = option;
        }
    }
}
