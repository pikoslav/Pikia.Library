using Xunit;

using Pikia.Library.Common.Types.Graphics;

namespace Pikia.Library.Tests;


public class UnitTest1
{
  [Fact]
  public void Test1()
  {
    GraphicUtilities.ResizeWhenNecessary(@"C:\TEMP\WP.jpg", 100);
  }
}