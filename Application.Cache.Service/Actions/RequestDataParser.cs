using Application.Cache.Service.Contracts;
using Common.Core.Cache.Client.Contracts;
using Common.Core.Cache.Client.Utils;
using Common.Core.DependencyInjection;
using System;

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

        private int _CommandCode;
        private int _keyLength;
        private string _key;
        private int _valueLength;
        private string _value;

        public RequestModel Parse(byte[] rev)
        {
            var revSpan = rev.AsSpan();

            var bytesCommandCode = revSpan.Slice(0, 2).ToArray();
            _CommandCode = ConvertTools.BytesToInt16(bytesCommandCode);

            if ((CommandType)_CommandCode == CommandType.Set)
            {
                var bytesKeyLength = revSpan.Slice(2, 2).ToArray();
                _keyLength = ConvertTools.BytesToInt16(bytesKeyLength);

                var bytesKey = revSpan.Slice(4, _keyLength).ToArray();
                _key = ConvertTools.BytesToString(bytesKey);

                var bytesValueLength = revSpan.Slice(4 + _keyLength, 2).ToArray();
                _valueLength = ConvertTools.BytesToInt16(bytesValueLength);

                var bytesValue = revSpan.Slice(4 + _keyLength + 2, _valueLength).ToArray();
                _value = ConvertTools.BytesToString(bytesValue);
            }
            else if((CommandType)_CommandCode == CommandType.Get 
                    || (CommandType)_CommandCode == CommandType.Remove)
            {
                var bytesKeyLength = revSpan.Slice(2, 2).ToArray();
                _keyLength = ConvertTools.BytesToInt16(bytesKeyLength);

                var bytesKey = revSpan.Slice(4, _keyLength).ToArray();
                _key = ConvertTools.BytesToString(bytesKey);

                _value = string.Empty;
            }
            else
            {
                _key = string.Empty;
                _value = string.Empty;
            }

            return new RequestModel
            {
                CommandCode = (CommandType)_CommandCode,
                Key = _key,
                Value = _value
            };
        }
    }
}
