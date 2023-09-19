/// <summary>
/// 정규식 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 05. 11
/// @version        : 0.1
/// @update
///     v0.1 (2023. 05. 11) : Joycollab 에서 사용하던 항목 정리.
/// </summary>

using System.Text.RegularExpressions;

namespace Joycollab.v2
{
	public class RegExp 
	{
		public static bool MatchDatetime(string str) 
		{
			Regex exp = new Regex("^[0-9]{4}-[0-9]{2}-[0-9]{2}$");
			Match m = exp.Match(str);
			return m.Success;
		}

		public static string MatchTime(string str)
		{
			string result;
			Regex exp = new Regex("^[0-2]{1}[0-9]{1}:[0-5]{1}[0-9]{1}$");
			Regex exp2 = new Regex("^[0-2]{1}[0-9]{1}[0-5]{1}[0-9]{1}$");
			Match m = exp.Match(str);
			Match m2 = exp2.Match(str);
			if (m.Success)
			{
				if(int.Parse( str.Substring(0,2)) < 24)
					result = str;
				else
					result = "";
			}
			else if (m2.Success)
			{
				if (int.Parse(str.Substring(0, 2)) < 24)
					result = str.Substring(0, 2) + ":" + str.Substring(2, 2);
				else
					result = "";
			}
			else
				result = "";
			return result;
		}

		public static string ReplaceOnlyNumber(string number) 
		{
			return Regex.Replace(number, @"\D", "");
		}

		public static bool MatchPhoneNumber(string number, out string onlyNumber) 
		{
			onlyNumber = Regex.Replace(number, @"\D", "");

			Regex exp = new Regex("^01([0|1|6|7|8|9])([0-9]{7,8})$");
			Match m = exp.Match(onlyNumber);
			return m.Success;
		}

		public static bool MatchUUID(string uuid) 
		{
			Regex exp = new Regex("^bearer[ ]{1}[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$");
			Match m = exp.Match(uuid);
			return m.Success;
		}

		public static bool MatchEnglishName(string name) 
		{
			Regex exp = new Regex("^[a-zA-Z\\s]*$");
			Match m = exp.Match(name);
			return m.Success;
		}

		public static bool MatchKoreanName(string name) 
		{
			Regex exp = new Regex("^[ㄱ-ㅎㅏ-ㅣ가-힣\\s]*$");
			Match m = exp.Match(name);
			return m.Success;
		}

		public static string MatchRegularName(string name)
		{
			return Regex.Replace(name, "^[a-zA-Z0-9ㄱ-ㅎㅏ-ㅣ가-힣\\s]*$", "", RegexOptions.Singleline);
		}

		public static bool MatchEmail(string email)
		{
			return Regex.IsMatch(email, @"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?");
		}

		public static bool MatchPasswordRule(string pw) 
		{
			Regex exp = new Regex("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*#?&])[A-Za-z\\d@$!%*#?&]{8,}$");
			Match m = exp.Match(pw);
			return m.Success;
		}

		public static bool MatchNumbers(int min, int max, string value) 
		{
			if (min > max) return false;

			int minLen = min.ToString().Length;
			int maxLen = max.ToString().Length;
			// Debug.Log(minLen +", "+ maxLen);

			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			builder.Clear();

			builder.Append("^[1-9]{1}[0-9]{");
			builder.Append(minLen-1);
			builder.Append(",");
			builder.Append(maxLen-1);
			builder.Append("}$");

			string temp = builder.ToString();
			// Debug.Log("RegExp | Match numbers - temp : "+ temp);
			Regex exp = new Regex(temp);
			Match m = exp.Match(value);
			return m.Success;
		}
	}
}