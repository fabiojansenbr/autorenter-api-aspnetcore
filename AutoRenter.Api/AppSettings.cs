﻿namespace AutoRenter.Api
{
    public class AppSettings
    {
        public virtual bool InMemoryProvider { get; set; }

        public virtual TokenSettings TokenSettings { get; set; }
    }
}
