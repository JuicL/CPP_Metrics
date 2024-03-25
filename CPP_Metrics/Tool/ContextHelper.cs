using CPP_Metrics.Types.Reflection;

namespace CPP_Metrics.Tool
{
    public static class ContextHelper
    {
        /// <summary>
        /// Поиск пространства имен в текущем пространстве имен
        /// </summary>
        /// <param name="contextElement"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static NamespaceContext? GetNameSpace(this BaseContextElement contextElement, string? name)
        {
            var currentContext = contextElement;
            while (currentContext is not null)
            {
                var namespaceInf = contextElement.Children.Where(n => n is NamespaceContext)
                                            .Cast<NamespaceContext>().FirstOrDefault(x => x.NameSpaceInfo.Name.Equals(name));
                if (namespaceInf != null)
                    return namespaceInf;
                currentContext = currentContext.Paren;
            }
            return null;
        }

        public static NamespaceContext? GetNameSpace(this BaseContextElement contextElement, string? name,List<CPPType> nested)
        {
            List<BaseContextElement?> currentNamespaceList = new List<BaseContextElement?>();
            currentNamespaceList.Add(BaseContextElement.GetGeneralNameSpace());
            currentNamespaceList.Add(contextElement);

            List<CPPType> newnested = new List<CPPType>(nested);
            if(name is not null)
                newnested.Add(new CPPType() { TypeName = name });

            //foreach (var item in newnested)
            // Проход наверх -- зацепим больше 
            foreach (var item in currentNamespaceList)
            {
                var currentNamespace = item;
                for (int i = 0; i < newnested.Count; i++)
                {
                    if (currentNamespace == null) break;
                    currentNamespace = currentNamespace.GetNameSpace(newnested[i].TypeName);
                    i++;
                    if (currentNamespace != null && i < newnested.Count)
                        currentNamespace = currentNamespace.GetNameSpace(newnested[i].TypeName, new List<CPPType>());
                }
                if (currentNamespace != null) 
                    return (NamespaceContext?)currentNamespace;
            }
            // Ищем среди using
            var usingNamespaces = contextElement.UsingNamespaces;
            foreach (var space in usingNamespaces)
            {
                var res = contextElement.GetNameSpace(space.Name,space.Nested);
                if(res is null) continue;
                
                var res2 = res.GetNameSpace(name, nested);
                if(res2 is not null)
                    return (NamespaceContext?)res2;
            }
            return null;
        }
        public static FunctionDeclaration? GetFunctionDeclaration(this BaseContextElement contextElement)
        {
            for (var context = contextElement; context is not null; context = context.Paren)
            {
                if (context is FunctionDeclaration)
                    return (FunctionDeclaration)context;
            }
            return null;
        }
        private static Variable? FindThisFields(BaseContextElement contextElement, string thisName)
        {
            var name = thisName.Substring(5);// this.
            var func = contextElement.GetFunctionDeclaration();
            if (func is not null && func.FunctionInfo.IsMethod)
            {
                var nestedList = new List<CPPType>(func.FunctionInfo.NestedNames);
                var className = nestedList.Last().TypeName;
                nestedList.RemoveAt(nestedList.Count - 1);

                var classContext = func.GetTypeName(className, nestedList);
                if (classContext != null && classContext is ClassStructDeclaration)
                {

                    var classStructDeclaration = (ClassStructDeclaration)classContext;
                    var field = classStructDeclaration.ClassStructInfo.Fields.FirstOrDefault(x => x.Name == name);
                    if (field is not null)
                    {
                        return field;
                    }
                }
            }

            return null;
        }
        public static Variable? GetVariableName(this BaseContextElement contextElement, string? name)
        {

            // TODO If contextElement is functionDeclaration and this method of class, first check in class field, and after in global context
            
            if (name == null) return null;
            
            if(name.StartsWith("this."))
            {
                var fieldName = FindThisFields(contextElement,name); 
                if(fieldName is not null)
                {
                    return fieldName;
                }
                return null;
            }
            
            for (var context = contextElement; context is not null; context = context.Paren)
            {

                if (context.VariableDeclaration.TryGetValue(name, out var variable))
                {
                    //variable.References.Add(context);
                    return variable;
                }
                if (context is FunctionDeclaration functionDeclaration)
                {
                    if(functionDeclaration.FunctionInfo.IsMethod)
                    {
                        var nestedList = new List<CPPType>(functionDeclaration.FunctionInfo.NestedNames);
                        var className = nestedList.Last().TypeName;
                        nestedList.RemoveAt(nestedList.Count - 1);

                        var classContext = functionDeclaration.GetTypeName(className,nestedList);
                        if(classContext != null && classContext is ClassStructDeclaration)
                        {
                            var classStructDeclaration = (ClassStructDeclaration)classContext;
                            var field = classStructDeclaration.ClassStructInfo.Fields.FirstOrDefault(x => x.Name == name);
                            if(field is not null)
                            {
                                //field.References.Add(context);
                                return (Variable)field;
                            }
                        }
                    }
                }
            }

            return null;
        }
        public static bool GetVariableName(this BaseContextElement contextElement, string? name, List<CPPType>? nestedNames)
        {

            return false;
        }

        public static List<FunctionInfo>? GetFunctionName(this BaseContextElement contextElement, string name)
        {
            if (name == null) return null;
            for (var context = contextElement; context is not null; context = context.Paren)
            {
                if (context.FunctionDeclaration.TryGetValue(name, out var functionInfo))
                {
                    return functionInfo;
                }

                if (context is FunctionDeclaration functionDeclaration)
                {
                    if (functionDeclaration.FunctionInfo.IsMethod)
                    {
                        var nestedList = functionDeclaration.FunctionInfo.NestedNames;
                        var className = nestedList.Last().TypeName;
                        nestedList.RemoveAt(nestedList.Count - 1);

                        var classContext = functionDeclaration.GetTypeName(className, nestedList);
                        if (classContext != null)
                        {
                            if (classContext.FunctionDeclaration.TryGetValue(name, out var methodsInfo))
                            {
                                return methodsInfo;
                            }
                        }
                    }
                }

            }
            return null;
        }
        private static BaseContextElement? TypeOrNamespace(BaseContextElement namespaceContext,string name, List<CPPType> nested)
        {
            List<CPPType> newnested = new List<CPPType>(nested);
            newnested.Add(new CPPType() { TypeName = name });
            var currentContext = namespaceContext;
            foreach (var item in newnested)
            {
                var namespaceFind = currentContext.GetNameSpace(item.TypeName,new List<CPPType>());
                var typeFind = GetTypeName2(currentContext,item.TypeName);

                BaseContextElement? find = typeFind is null ? namespaceFind : typeFind;
                //BaseContextElement? find = namespaceFind is null ? typeFind : namespaceFind;
                if (find == null) return null;
                currentContext = find;
            }

            return currentContext;
        }

        public static BaseContextElement? GetTypeName(this BaseContextElement contextElement, string name, List<CPPType> nested)
        {
            List<BaseContextElement> namespaces = new List<BaseContextElement>();
            namespaces.Add(contextElement);
            
            foreach (var currentContext in namespaces)
            {
                var temp2 = TypeOrNamespace(currentContext, name, nested);
                if (temp2 is not null && temp2 is ClassStructDeclaration) 
                    return temp2;
            }

            List<SimpleUsing> simpleUsing = new();

            foreach (var item in simpleUsing)
            {
                var temp = TypeOrNamespace(contextElement, item.Name, item.Nested);
                if (temp != null && temp is ClassStructDeclaration structDeclaration)
                {
                    if (structDeclaration.ClassStructInfo.Name.Equals(name) && nested.Count == 0)
                        return structDeclaration;

                    var temp2 = TypeOrNamespace(temp, name, nested);
                    if (temp2 is not null)
                        return (ClassStructDeclaration?)temp2;
                }
            }

            return null;
        }
        private static ClassStructDeclaration? GetTypeName2(BaseContextElement contextElement, string name)
        {
            List<BaseContextElement> namespaces = new List<BaseContextElement>();
            List<ClassStructDeclaration> simpleUsings = new List<ClassStructDeclaration>();

            var currentContext = contextElement;
            while (currentContext is not null)
            {
                foreach (var item in currentContext.UsingNamespaces)
                {
                    var temp = contextElement.GetNameSpace(item.Name, item.Nested);
                    if (temp is not null)
                        namespaces.Add(temp);
                }
                var simpleUsing = currentContext.SimpleUsing.Where(x => x.BaseContextElement is not null).Select(x => x.BaseContextElement);
                if(simpleUsing is not null)
                    simpleUsings.AddRange(simpleUsing);

                var classStructInfo = currentContext.Children.Where(n => n is ClassStructDeclaration)
                                            .Cast<ClassStructDeclaration>().FirstOrDefault(x => x.ClassStructInfo.Name.Equals(name));
                if(classStructInfo != null)
                    return classStructInfo;
                currentContext = currentContext.Paren;
            }
            // перебираем все добавленные области
            foreach (var item in namespaces)
            {
                foreach (var item2 in item.UsingNamespaces)
                {
                    var temp = contextElement.GetNameSpace(item2.Name, item2.Nested);
                    if (temp is not null)
                        namespaces.Add(temp);
                }

                var classStructInfo = item.Children.Where(n => n is ClassStructDeclaration)
                                            .Cast<ClassStructDeclaration>().FirstOrDefault(x => x.ClassStructInfo.Name.Equals(name));
                if (classStructInfo != null)
                    return classStructInfo;
            }

            // Перебираем все Using types
            foreach (var item in simpleUsings)
            {
                if(item.ClassStructInfo.Name.Equals(name))
                {
                    return item;
                }
            }


            return null;
        }

    }
}
