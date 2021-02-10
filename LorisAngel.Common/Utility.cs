namespace LorisAngel.Common
{
    public class Utility
    {
        private Utility() { }

        /// <summary>
        /// Fix the string to be more readable with an uppercase first letter and then lowercase for the rest.
        /// </summary>
        /// <param name="s">The string to fix casing of</param>
        /// <returns>Returns the string with an upper case first letter</returns>
        public static string ToUppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            return char.ToUpper(s[0]) + s.Substring(1).ToLower();
        }
    }
}
