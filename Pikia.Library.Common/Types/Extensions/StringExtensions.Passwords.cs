using System.Text.RegularExpressions;

namespace Pikia.Library.Common.Types;


public static partial class StringExtensions
{
  public static Int32 GetPasswordPoints(this String password) // min 0, max 5
  {
    var passwordStrength = 0;
    var passwordPoints = 0;
    if (password == null) return passwordStrength;
    
    Regex rx;
    // If password is longer than 6 symbols than add 1 point
    if (password.Length > 6) passwordPoints++;
    
    // If password has both lower and upper case characters than add 1 point
    rx = new Regex(@"(?=.*[a-z])(?=.*[A-Z])");
    if (rx.Match(password).Success) passwordPoints++;
    
    // Add 1 point if password contains at least one digit
    rx = new Regex(@"[0-9]");
    if (rx.Match(password).Success) passwordPoints++;
    
    // Add 1 point in case if password contains at least one special char
    rx = new Regex(@"\~|\@|\#|\$|\%|\^|\&|\*|\:|\;");
    if (rx.Match(password).Success) passwordPoints++;
    
    // Add 1 point if password is longer than 8 symbols
    if (password.Length > 8) passwordPoints++;

    return passwordPoints;
  }

  public enum PasswordStrength
  {
    TooWeak,
    Weak,
    Adequate,
    Strong,
  }

  public static PasswordStrength GetPasswordStrength(this String password) => GetPasswordPoints(password) switch
  {
    > 5 => PasswordStrength.Strong,
    > 3 => PasswordStrength.Adequate,
    > 2 => PasswordStrength.Weak,
    _ => PasswordStrength.TooWeak
  };
}
