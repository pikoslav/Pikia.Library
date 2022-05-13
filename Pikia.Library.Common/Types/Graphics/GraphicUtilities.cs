using SkiaSharp;

namespace Pikia.Library.Common.Types.Graphics;


public class GraphicUtilities
{
  public static Boolean ResizeWhenNecessary(String fileName, Int32 maxDimension) => ResizeWhenNecessary(fileName, fileName, maxDimension);
  public static Boolean ResizeWhenNecessary(String inputFile, String outputFile, Int32 maxDimension)
  {
    ArgumentNullException.ThrowIfNull(nameof(inputFile));
    if (!File.Exists(inputFile)) throw new FileNotFoundException(nameof(inputFile));

    SKBitmap? bitmap;
    using (var reader = File.OpenRead(inputFile))
    {
      bitmap = SKBitmap.Decode(reader);
    }
    if (bitmap is not null && ResizeWhenNecessary(ref bitmap, maxDimension) && bitmap is not null)
    {
      using var writer = File.Create(outputFile);
      bitmap.Encode(writer, SKEncodedImageFormat.Jpeg, 90);
      return true;
    }

    return false;
  }

  public static Boolean ResizeWhenNecessary(ref SKBitmap? bitmap, Int32 maxDimension)
  {
    if (maxDimension < 1) throw new ArgumentOutOfRangeException(nameof(maxDimension));
    if (bitmap is null) return false;
    if (bitmap.Width <= maxDimension && bitmap.Height <= maxDimension) return false;

    Int32 width, height;
    if (bitmap.Width >= bitmap.Height)
    {
      var factor = 1.0 * maxDimension / bitmap.Width;
      width = maxDimension;
      height = (Int32)(factor * bitmap.Height);
    }
    else
    {
      var factor = 1.0 * maxDimension / bitmap.Height;
      height = maxDimension;
      width = (Int32)(factor * bitmap.Width);
    }

    var newBitmap = new SKBitmap(width, height);
    if (bitmap.ScalePixels(newBitmap, SKFilterQuality.High))
    {
      bitmap.Dispose();
      bitmap = newBitmap;
      return true;
    }

    return false;
  }
}
