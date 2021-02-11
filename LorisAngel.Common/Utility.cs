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

        /// <summary>
        /// Get the invite link to add the bot to the server
        /// </summary>
        /// <param name="clientid">The client ID of the bot. The default is LorisAngel</param>
        /// <param name="permissions">The permissions to return. Default is LorisAngel perms</param>
        /// <returns>Returns a string of a url for the invite link</returns>
        public static string GetBotInviteLink(ulong clientid = 729696788097007717, int permissions = 44038)
        {
            return $"https://discordapp.com/oauth2/authorize?client_id={clientid}&scope=bot&permissions={permissions}";
        }
    }
}
