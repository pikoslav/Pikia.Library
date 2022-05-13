// See https://aka.ms/new-console-template for more information
using Pikia.Library.Meta;

Console.WriteLine("Hello, World!");




var s = MetaGenerator.Generate<MetaTestis>();

Console.WriteLine(s);






public class MetaTestis : MetaClass<Testis>
{
  public MetaString Ime = new(nameof(Ime));
  public MetaString Priimek = new(nameof(Priimek));

  public override HashSet<MetaProperty> MetaProperties => new()
  {
    Ime,
    Priimek,
  };
}
