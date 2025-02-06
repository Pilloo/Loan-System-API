using System.Reflection;

namespace Core.Shared;

public class EmptyMembersChecker
{
    public static bool HasEmptyMembers(object obj)
    {
        PropertyInfo[] properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (PropertyInfo p in properties)
        {
            var value = p.GetValue(obj);
            if (value is null || (value is int and 0) || (value is string s && string.IsNullOrEmpty(s)))
            {
                return true;
            }
        }
        
        return false;
    }
}