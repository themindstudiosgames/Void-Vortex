using System;

namespace Infrastructure.ClientAPI
{
    public class Request
    {
        public readonly string Key;
        public readonly Action Action;

        public Request(string key, Action action)
        {
            Key = key;
            Action = action;
        }
    }
}