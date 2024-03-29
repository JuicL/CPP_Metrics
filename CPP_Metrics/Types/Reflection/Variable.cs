﻿namespace CPP_Metrics.Types.Reflection
{
    //Variable Type and Identifier
    public class Variable
    {
        public CPPType? Type { get; set; }
        public string? Name { get; set; }
        public List<BaseContextElement> References { get; set; } = new List<BaseContextElement>();

    }
}
