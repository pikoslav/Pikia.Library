using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pikia.Library.Meta;
public class MetaGenerator
{
  public static String Generate<T>() where T : MetaClass, new()
  {
    var metaClass = new T();

    var sb = new StringBuilder();
    sb.AppendLine($"public class {typeof(T).Name}");
    sb.AppendLine($"{{");
    
    foreach(var metaProperty in metaClass.MetaProperties)
    {
      var pType = metaProperty.PropertyType.Name;
      var pField = String.Concat("_", metaProperty.PropertyName[0..1].ToLower() + metaProperty.PropertyName[1..]);
      var pProperty = metaProperty.PropertyName;

      sb.AppendLine($"  private {pType} {pField};");
      sb.AppendLine($"  public {pType} {pProperty}");
      sb.AppendLine($"  {{");
      sb.AppendLine($"    get => {pField};");
      sb.AppendLine($"    set => SetPropertyValue(nameof({pProperty}), ref {pField}, value);");
      sb.AppendLine($"  }}");
      sb.AppendLine($"");
    }
    
    sb.AppendLine($"}}");

    return sb.ToString();
  }
}
