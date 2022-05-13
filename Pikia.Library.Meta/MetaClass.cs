using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pikia.Library.Meta;
public abstract class MetaClass
{ 
  public abstract Type Type { get; }

  public abstract HashSet<MetaProperty> MetaProperties { get; }
  
}

public abstract class MetaClass<T> : MetaClass where T : class
{
  public override Type Type => typeof(T);
}

public static class MetaClassExtensions
{
  public static T AddMetaProperty<T>(this T metaClass, MetaProperty metaProperty) where T : MetaClass<T>
  {
    metaClass.MetaProperties.Add(metaProperty);
    return metaClass;
  }
}


