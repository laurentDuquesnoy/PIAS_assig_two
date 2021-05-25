using System;
using SerializingUtils.enums;

namespace SerializingUtils.Classes
{
    public class ConverterResult<TResult>
    {
        public ConverterStatus Status { get; set; }
        public Exception  Error { get; set; }
        public TResult ReturnValue { get; set; }
    }
}