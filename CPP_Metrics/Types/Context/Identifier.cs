﻿namespace CPP_Metrics.Types.Context
{
    public class Identifier
    {
        public string? Name { get; set; }

        public IList<CPPType>? Templates { get; set; }

        public bool IsTemplateIdentifier { get { return Templates is not null; } }

    }


}
