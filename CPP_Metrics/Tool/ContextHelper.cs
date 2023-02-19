using CPP_Metrics.Types.Context;

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
                if (currentNamespace != null) return (NamespaceContext?)currentNamespace;
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
        public static bool GetVariableName(this BaseContextElement contextElement, string? name)
        {
            // TODO If contextElement is functionDeclaration and this method of class, first check in class field, and after in global context
            if (name == null) return false;
            for (var context = contextElement; context is not null; context = context.Paren)
            {
                if (context.VariableDeclaration.TryGetValue(name, out var variable))
                {
                    return true;
                }
            }

            return false;
        }
        public static bool GetVariableName(this BaseContextElement contextElement, string? name, List<CPPType>? nestedNames)
        {

            return false;
        }

        public static bool GetFunctionName(this BaseContextElement contextElement, string name)
        {
            if (name == null) return false;
            for (var context = contextElement; context is not null; context = context.Paren)
            {
                if (context.FunctionDeclaration.TryGetValue(name, out var functionInfo))
                {
                    return true;
                }
            }
            return false;
        }
        private static BaseContextElement? TypeOrNamespace(BaseContextElement namespaceContext,string name, List<CPPType> nested)
        {
            List<CPPType> newnested = new List<CPPType>(nested);
            newnested.Add(new CPPType() { TypeName = name });
            var currentContext = namespaceContext;
            foreach (var item in newnested)
            {
                var namespaceFind = currentContext.GetNameSpace(item.TypeName,new List<CPPType>());
                var typeFind = currentContext.GetTypeName(item.TypeName);
                BaseContextElement? find = namespaceFind is null ? typeFind : namespaceFind;
                if (find == null) return null;
                currentContext = find;
            }

            return currentContext;
        }

        public static BaseContextElement? GetTypeName(this BaseContextElement contextElement, string name, List<CPPType> nested)
        {
            List<BaseContextElement> namespaces = new List<BaseContextElement>();
                namespaces.Add(contextElement);
            // Добавляем namespace
            foreach (var item in contextElement.UsingNamespaces)
            {
                var temp = contextElement.GetNameSpace(item.Name, item.Nested);
                if (temp is null)
                    throw new Exception("Using namespace not found");
                    namespaces.Add(temp);
            }

            // Ищем тип в using <Ns::Class>
            foreach (var item in contextElement.SimpleUsing)
            {
                var temp = TypeOrNamespace(contextElement, item.Name,item.Nested);
                if(temp != null && temp is ClassStructDeclaration structDeclaration)
                {
                    if (structDeclaration.ClassStructInfo.Name.Equals(name) && nested.Count == 0)
                        return structDeclaration;
                    
                    var temp2 = TypeOrNamespace(temp, name, nested);
                    if (temp2 is not null) 
                        return temp2;
                }
            }
            // проверяем среди всех включенных областей
            foreach (var currentContext in namespaces)
            {
                var temp2 = TypeOrNamespace(currentContext, name, nested);
                if (temp2 is not null && temp2 is ClassStructDeclaration) 
                    return temp2;
            
            }


            return null;
        }
        public static ClassStructDeclaration? GetTypeName(this BaseContextElement contextElement, string name)
        {
            var currentContext = contextElement;
            while (currentContext is not null)
            {
                var classStructInfo = currentContext.Children.Where(n => n is ClassStructDeclaration)
                                            .Cast<ClassStructDeclaration>().FirstOrDefault(x => x.ClassStructInfo.Name.Equals(name));
                if(classStructInfo != null)
                    return classStructInfo;
                currentContext = currentContext.Paren;
            }
            return null;
        }

    }
}
