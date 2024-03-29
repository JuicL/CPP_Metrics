﻿using CPP_Metrics.Metrics.CyclomaticComplexity;
using CPP_Metrics.Types.Reflection;

namespace CPP_Metrics.Types
{
    public class CyclomaticComplexityInfo
    {
        public FunctionInfo FunctionInfo { get; set; }
        public CyclomaticGraph CyclomaticGraph { get; set; }
        public int CyclomaticComplexityValue { get; set; }
        public string FileName { get; set; }
    }
}
