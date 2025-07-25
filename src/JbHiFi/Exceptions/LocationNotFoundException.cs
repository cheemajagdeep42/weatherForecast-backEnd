﻿using System;

namespace JbHiFi.Exceptions
{
    public class LocationNotFoundException : Exception
    {
        public LocationNotFoundException()
        {
        }

        public LocationNotFoundException(string message)
            : base(message)
        {
        }

        public LocationNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
