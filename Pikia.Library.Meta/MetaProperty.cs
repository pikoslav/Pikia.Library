using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pikia.Library.Meta;
public abstract class MetaProperty
{
  protected MetaProperty(String propertyName) 
  {
    PropertyName = propertyName;
  }

  public abstract Type PropertyType { get; }
  public virtual String PropertyName { get; private set; }
}

public abstract class MetaProperty<T> : MetaProperty
{
  protected MetaProperty(String propertyName) : base(propertyName)  {  }

  public override Type PropertyType => typeof(T);
}

public class MetaString : MetaProperty<String>
{
  public MetaString(String propertyName) : base(propertyName)  {  }
}

public class MetaClassProperty
{
  public MetaClassProperty(MetaClass metaClass, MetaProperty metaProperty)
  {
    MetaClass = metaClass;
    MetaProperty = metaProperty;
    
    PropertyInfo = metaClass.Type.GetProperty(metaProperty.PropertyName) ?? throw new ArgumentException(nameof(metaProperty.PropertyName));
  }

  public MetaClass MetaClass { get; set; }
  public MetaProperty MetaProperty { get; set; }

  public PropertyInfo PropertyInfo { get; private set; }

}


