﻿using CPP_Metrics.CyclomaticComplexity;
using CPP_Metrics.Types.Context;

namespace CPP_Metrics.Metrics
{
    public class CyclomaticComplexityInfo
    {
        public FunctionInfo FunctionInfo { get; set; }
        public CyclomaticGraph CyclomaticGraph { get; set; }
        public int CyclomaticComplexityValue { get; set; }
    }
}