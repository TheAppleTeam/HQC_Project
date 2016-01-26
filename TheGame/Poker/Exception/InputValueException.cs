namespace Poker.Exception
{
    using System;

    public class InputValueException : ArgumentException
    {
        public InputValueException(string msg)
            : base(msg)
        {
        }
    }
}
