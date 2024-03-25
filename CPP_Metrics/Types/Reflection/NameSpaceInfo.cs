namespace CPP_Metrics.Types.Reflection
{
    public class NameSpaceInfo
    {
        public bool IsInline { get; set; } = false;
        public string Name { get; set; }
        public List<CPPType> Nested { get; set; } = new();
        public string FullName()
        {
            string res = "";
            foreach (var item in Nested)
            {
                res += item.TypeName;
                if (item.TypeName.Equals("::"))
                    continue;
                res += "::";
            }
            res += Name;
            return res;
        }
    }

}
