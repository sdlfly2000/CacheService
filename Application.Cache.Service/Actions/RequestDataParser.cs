using Application.Cache.Service.Contracts;
using Common.Core.DependencyInjection;
using System;
using System.Text;

namespace Application.Cache.Service.Actions
{
    [ServiceLocate(typeof(IRequestDataParser))]
    public class RequestDataParser : IRequestDataParser
    {
        // Structure of Request
        // **|**|**|****************|**|******************************************|
        //  1  2  3                4  5                                          6 

        // 1) Total Length of Request:  int
        // 2) Command Code:             int
        // 3) Length of Key:            int
        // 4) Key:                      string
        // 5) Length of Value:          int
        // 6) Value:                    string

        private int _totalLength;
        private int _CommandCode;
        private int _keyLength;
        private string _key;
        private int _valueLength;
        private string _value;

        public RequestModel Parse(byte[] rev)
        {
            _totalLength = BitConverter.ToInt16(rev.AsSpan().Slice(0, 2));
            _CommandCode = BitConverter.ToInt16(rev.AsSpan().Slice(2, 2));
            _keyLength = BitConverter.ToInt16(rev.AsSpan().Slice(4, 2));
            _key = Encoding.ASCII.GetString(rev, 6, _keyLength);
            _valueLength = BitConverter.ToInt16(rev.AsSpan().Slice(6+_keyLength, 2));
            _value = Encoding.ASCII.GetString(rev, 6 + _keyLength + 2, _valueLength);

            return new RequestModel
            {
                CommandCode = (CommandType)_CommandCode,
                Key = _key,
                Value = _value
            };
        }
    }
}
