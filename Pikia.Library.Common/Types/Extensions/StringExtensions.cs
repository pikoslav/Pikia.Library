using System.ComponentModel;
using System.Globalization;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Pikia.Library.Common.Types;


public static partial class StringExtensions
{
  public static Boolean IsBlank(this String? value) => value is null || String.IsNullOrWhiteSpace(value);
  public static Boolean NotBlank(this String? value) => !IsBlank(value);

  public static String? Nullify(this String? s) => String.IsNullOrWhiteSpace(s) ? null : s.Trim();
  public static String Emptify(this String? s) => String.IsNullOrWhiteSpace(s) ? String.Empty : s;
  public static String Denullify(this String? s, String? replacement = default) => String.IsNullOrWhiteSpace(s) ? (replacement ?? String.Empty) : s;

  public static String? CombineWith(this String? a, String? b, String combo = " — ")
  {
    if (a.IsBlank()) return null;
    if (b.IsBlank()) return a;
    return $"{a}{combo}{b}";
  }

  //
  //

  // "aàáâãäåçc" -> "aaaaaaacc"
  public static String? RemoveDiacritics(this String? value)
  {
    if (String.IsNullOrWhiteSpace(value)) return value;

    var normalized = value.Normalize(NormalizationForm.FormD);
    var sb = new StringBuilder();
    for (var i = 0; i < normalized.Length; i++)
    {
      var c = normalized[i];
      if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark) sb.Append(c);
    }
    return sb.ToString().Normalize(NormalizationForm.FormC);
  }

  public static String? RemoveHead(this String? value, String? head, StringComparison comparision = StringComparison.CurrentCulture)
  {
    if (value is null) return value;
    if (head is null) return value;
    if (value.StartsWith(head, comparision)) value = value[head.Length..];
    return value;
  }

  public static String? RemoveTail(this String? value, String? tail, StringComparison comparison = StringComparison.CurrentCulture)
  {
    if (value is null) return value;
    if (tail is null) return value;
    if (value.EndsWith(tail, comparison)) value = value[..^tail.Length];
    return value;
  }

  public static String? Repeat(this String? value, Int32 count) => String.Concat(Enumerable.Repeat(value, count));

  //
  //

  public static String? SafeTrim(this String? value) => value?.Trim();

  public static String? SafeSubstring(this String? value, Int32 startIndex, Int32 length)
  {
    if (String.IsNullOrEmpty(value)) return value;
    if (startIndex >= value.Length) return String.Empty;
    return value.Substring(startIndex, (length > value.Length - startIndex) ? value.Length - startIndex : length);
  }

  public static String? SafeSubstring(this String? value, Int32 startIndex)
  {
    if (String.IsNullOrEmpty(value)) return value;
    if (startIndex >= value.Length) return String.Empty;
    return value[startIndex..];
  }

  public static Int32 SafeIndexOf(this String value, String substring) => value == null ? -1 : value.IndexOf(substring);
  public static Int32 SafeLength(this String value) => value == null ? 0 : value.Length;

  //
  //

  public static String SubstringBefore(this String? value, String? substring)
  {
    ArgumentNullException.ThrowIfNull(value);
    ArgumentNullException.ThrowIfNull(substring);

    var substringOffset = value.IndexOf(substring, StringComparison.CurrentCulture);
    return substringOffset == -1 ? value : value[..substringOffset];
  }

  /// <summary>
  /// Returns a substring of <paramref name="value"/> until <paramref name="substring"/>
  /// or <paramref name="default"/>, if there's no substring in <paramref name="value"/>.
  /// </summary>
  /// <param name="value"></param>
  /// <param name="substring"></param>
  /// <param name="default"></param>
  /// <returns></returns>
  /// <example>
  /// <code>
  /// "ab" == "abc".SubstringBefore("c", null);
  /// null == "abc".SubstringBefore("d", null);
  /// "xx" == "abc".SubstringBefore("e", "xx");
  /// </code>
  /// </example>
  public static String SubstringBefore(this String value, String substring, String @default)
  {
    ArgumentNullException.ThrowIfNull(value);
    ArgumentNullException.ThrowIfNull(substring);

    var substringOffset = value.IndexOf(substring, StringComparison.CurrentCulture);
    return substringOffset == -1 ? @default : value[..substringOffset];
  }

  public static String SubstringBeforeSuffix(this String s, String substring)
  {
    if (s is null) throw new ArgumentNullException(nameof(s));
    if (substring is null) throw new ArgumentNullException(nameof(substring));

    if (!s.EndsWithSuffix(substring)) return s;
    var substringOffset = s.LastIndexOf(substring, StringComparison.CurrentCulture);
    return substringOffset == -1 ? s : s.Substring(0, substringOffset);
  }

  public static String SubstringAfter(this String s, String substring)
  {
    if (s is null) throw new ArgumentNullException(nameof(s));
    if (substring is null) throw new ArgumentNullException(nameof(substring));

    var start = s.IndexOf(substring, StringComparison.CurrentCulture);
    return start == -1 ? s : s[(start + substring.Length)..];
  }

  public static String SubstringAfterLast(this String s, String substring)
  {
    if (s is null) throw new ArgumentNullException(nameof(s));
    if (substring is null) throw new ArgumentNullException(nameof(substring));

    var start = s.LastIndexOf(substring, StringComparison.CurrentCulture);
    return start == -1 ? s : s[(start + substring.Length)..];
  }


  public static String? SubstringAfterOrNull(this String s, String substring)
  {
    if (s is null) throw new ArgumentNullException(nameof(s));
    if (substring is null) throw new ArgumentNullException(nameof(substring));

    var i = s.IndexOf(substring, StringComparison.CurrentCulture);
    return i == -1 ? null : s[(i + substring.Length)..];
  }

  public static (String? before, String? after) SubstringBeforeAndAfter(this String value, String substring)
  {
    var before = SubstringBefore(value, substring);
    var after = SubstringAfterOrNull(value, substring);
    return (before, after);
  }

  public static String SubstringBetween(this String s, String start, String end) => s.SubstringAfter(start).SubstringBefore(end);
  public static Boolean EndsWithSuffix(this String s, String suffix) => s.Length > suffix.Length && s.EndsWith(suffix);

  //
  //

  public static String[] Split(this String value, String separator)
  {
    if (value is null) throw new ArgumentNullException(nameof(value));
    if (separator is null) throw new ArgumentNullException(nameof(separator));

    return value.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
  }


  public static T? TryParse<T>(this String value) where T : struct
  {
    if (value == null) return null;
    var type = typeof(T);
    var converter = TypeDescriptor.GetConverter(type);
    if (converter != null)
    {
      try
      {
        return (T)converter.ConvertFromString(value)!;
      }
      catch
      {
        return null;
      }
    }

    var tryParseMethod = type.GetMethod("TryParse", new Type[] { typeof(String), Type.GetType(type.FullName + "&")! });
    if (tryParseMethod is not null)
    {
      var parameters = new Object[2];
      parameters[0] = value;
      tryParseMethod.Invoke(type, parameters);
      return (T)parameters[1];
    }
    throw new NotSupportedException("TryParse is not supported on type " + type.FullName);
  }





  //
  //

  public static String? CombineWith(this String? a, String? b)
  {
    if (a.IsBlank()) return null;
    if (b.IsBlank()) return a;
    return $"{a} — {b}";
  }

  public static String Indent(this String source, Int32 level)
  {
    if (source is null) throw new ArgumentNullException(nameof(source));

    var prefix = new String(' ', level);
    return prefix + source.Replace("\n", "\n" + prefix);
  }

  public static String Unindent(this String source)
  {
    if (source is null) throw new ArgumentNullException(nameof(source));

    var lines = TrimEmptyLines(source.Split(new[] { Environment.NewLine }, StringSplitOptions.None));
    var indent = new String((lines.FirstOrDefault() ?? String.Empty).TakeWhile(Char.IsWhiteSpace).ToArray());
    lines = lines.Select(line => line.StartsWith(indent, StringComparison.InvariantCulture) ? line[indent.Length..] : line.TrimStart(' '));
    return String.Join(Environment.NewLine, lines.ToArray());
  }

  private static IEnumerable<String> TrimEmptyLines(IEnumerable<String> source)
  {
    var array = source.ToArray();
    var start = 0;
    var end = array.Length - 1;
    while (start < end && array[start].All(Char.IsWhiteSpace)) start++;
    while (end >= start && array[end].All(Char.IsWhiteSpace)) end--;
    return array.Skip(start).Take(end - start + 1);
  }


  public static Boolean ContainsAnyChar(this String source, String chars)
  {
    if (source is null) throw new ArgumentNullException(nameof(source));
    if (chars is null) throw new ArgumentNullException(nameof(chars));

    return source.IndexOfAny(chars.ToCharArray()) != -1;
  }

  public static String ReplaceFirst(this String text, String search, String replace)
  {
    if (text is null) throw new ArgumentNullException(nameof(text));
    if (search is null) throw new ArgumentNullException(nameof(search));

    var position = text.IndexOf(search, StringComparison.Ordinal);
    return position < 0 ? text : text.Substring(0, position) + replace + text[(position + search.Length)..];
  }

  public static String Wrap(this String text, Int32 maxLength)
  {
    if (text is null) throw new ArgumentNullException(nameof(text));

    if (text.Length == 0) return String.Empty;
    var words = text.Split(' ');
    var lines = new StringBuilder();
    var line = "";
    foreach (var word in words)
    {
      if ((line.Length > maxLength) || (line.Length + word.Length > maxLength))
      {
        lines.AppendLine(line);
        line = String.Empty;
      }
      line = line.Length > 0 ? line + " " + word : line + word;
    }
    if (line.Length > 0) lines.AppendLine(line);
    return lines.ToString().TrimEnd();
  }


  public static String ToMd5Fingerprint(this String text)
  {
    if (text is null) throw new ArgumentNullException(nameof(text));

    var bytes = Encoding.Unicode.GetBytes(text.ToCharArray());
    var md5 = MD5.Create();
    var hash = md5.ComputeHash(bytes);
    return hash.Aggregate(new StringBuilder(32), (sb, b) => sb.Append(b.ToString("X2", CultureInfo.InvariantCulture))).ToString();
  }


  public static String RemoveCharactersRegex(this String value, Char[] characters, Boolean ignoreCase = true) => RemoveCharactersRegex(value, new String(characters), ignoreCase);
  public static String RemoveCharactersRegex(this String value, String characters, Boolean ignoreCase = true) => Regex.Replace(value, String.Concat("[", characters, "]"), String.Empty, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);


  public static String CutoffWithEllipsis(this String value, Int32 cutoffLength)
  {
    if (value.IsBlank()) return value;
    var expression = "(?<=^.{_}).*".Replace("_", cutoffLength.ToString(CultureInfo.InvariantCulture)); // todo! preveri pravo lkacijo
    return Regex.Replace(value, expression, "…");
  }

  public static Boolean WildcardMatch(this String input, String pattern, Boolean ignoreCase = true)
  {
    var patterenAsRegex = "^" + Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", ".") + "$";
    return Regex.IsMatch(input, patterenAsRegex, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
  }



  public static String EnsureMultiline(this String[] input, Int32 count) => String.Join(Environment.NewLine, input).EnsureMultiline(count);
  public static String EnsureMultiline(this String input, Int32 count)
  {
    var lines = (input ?? String.Empty).Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
    while (lines.Count < count) lines.Add(String.Empty);
    return String.Join(Environment.NewLine, lines.ToArray());
  }


  //
  //

  public static void Deconstruct(this String s, out String original, out Int32 length)
  {
    original = s;
    length = (s ?? String.Empty).Length;
  }

  //
  //

  public static String? RemoveTags(this String? s)
  {
    if (s is null) return null;
    if (s.Length == 0) return s;

    var depth = 0;
    var result = new StringBuilder(s.Length);
    foreach (var c in s)
    {
      if (c == '<')
      {
        depth++;
        continue;
      }
      if (c == '>')
      {
        depth--;
        continue;
      }
      if (depth != 0) continue;
      result.Append(c);
    }
    return result.ToString();
  }

  //
  //

  public static String? ToPascalCase(this String? s) => s is not null ? CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s) : default;
  public static Boolean IsPascalCase(this String s) => s is not null && s == ToPascalCase(s);

  public static Boolean IsAnyOf(this String s, params String[] values) => values.Any(x => x == s);

}
