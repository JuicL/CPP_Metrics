namespace CPP_Metrics.Types.Context
{

    public class CPPType
    {
        public string? ClassStructMarker { get; set; }
        public string TypeName { get; set; }
        public List<CPPType>? NestedNames { get; set; } = new List<CPPType>();
        public virtual List<CPPType>? TemplateNames { get; set; }
        public string? ClassMarker { get; set; }
        public string? FunctionSpecifier { get; set; }
        public bool IsTemplate { get { return TemplateNames is not null; } }
        public bool IsStandartType { get; set; }
        public bool IsVirtual
        {
            get
            {
                return (FunctionSpecifier is not null && FunctionSpecifier.Equals("virtual")) ? true : false;
            }
        }
        public List<CPPType> GetTemplateNamesList()
        {
            var list = new List<CPPType>();
            if (TemplateNames != null) return list;
            Stack<CPPType> stack = new();
            foreach (var type in TemplateNames.Reverse<CPPType>())
            {
                stack.Push(type);
            }

            while (stack.Any())
            {
                var cur = stack.Pop();
                list.Add(cur);
                if (cur.TemplateNames is null) continue;
                foreach (var type in cur.TemplateNames)
                {
                    stack.Push(type);
                }
            }

            return list;
        }
        public string GetFullName()
        {
            string res = "";
            res += GetNamespace();
            res += TypeName;
            return res;
        }
        public string GetNamespace()
        {
            string res = "";
            foreach (var item in NestedNames)
            {
                res += item.TypeName;
                if (item.TypeName.Equals("::"))
                    continue;
                res += "::";
            }

            return res;
        }
    }

}
