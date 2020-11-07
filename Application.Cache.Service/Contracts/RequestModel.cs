namespace Application.Cache.Service.Contracts
{
    public class RequestModel
    {
        public CommandType CommandCode { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }
    }
}
