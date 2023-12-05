namespace MVC_Project.Utilities.Extensions
{
    public class StringFormat
    {
        public static string Capitalize(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            return char.ToUpper(input[0]) + input.Substring(1).ToLower().Trim();
        }
    }
   
}
