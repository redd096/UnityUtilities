
namespace redd096
{
    public static class NumberToWords
    {
        /// <summary>
        /// From 2 to "two" and 13 to "thirteen"
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string ToCardinalNumberInWords(long number)
        {
            //zero
            if (number == 0)
                return "zero";

            //-number
            if (number < 0)
                return "minus " + ToCardinalNumberInWords(System.Math.Abs(number));

            string s = "";

            //big numbers
            if ((number / 1000000000) > 0)
            {
                s += ToCardinalNumberInWords(number / 1000000000) + " billion";
                number %= 1000000000;
            }

            if ((number / 1000000) > 0)
            {
                s += ToCardinalNumberInWords(number / 1000000) + " million";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                s += ToCardinalNumberInWords(number / 1000) + " thousand";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                s += ToCardinalNumberInWords(number / 100) + " hundred";
                number %= 100;
            }

            if (number > 0)
            {
                //add "and" for example "one thousand and three hundred"
                if (s != "")
                    s += " and ";

                var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                //until 19 units
                if (number < 20)
                {
                    s += unitsMap[number];
                }
                //else, add tens + unit
                else
                {
                    s += tensMap[number / 10];
                    if ((number % 10) > 0)
                        s += "-" + unitsMap[number % 10];
                }
            }

            return s;
        }

        /// <summary>
        /// From 2 to "2nd" and 13 to "13th"
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string ToOrdinalNumber(long number)
        {
            string s = number.ToString();

            //eleventh, twelfth and thirteenth 
            if (s.EndsWith("11") || s.EndsWith("12") || s.EndsWith("13"))
                return s + "th";

            //first, twenty first - second, twenty second - third, twenty third - fourth, twenty fourth
            if (s.EndsWith("1"))
                return s + "st";
            else if (s.EndsWith("2"))
                return s + "nd";
            else if (s.EndsWith("3"))
                return s + "rd";
            else
                return s + "th";
        }

        /// <summary>
        /// From 2 to "second" and 13 to "thirteenth"
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string ToOrdinalNumberInWords(long number)
        {
            string s = ToCardinalNumberInWords(number);

            //first, twenty first
            if (s.EndsWith("one"))
                return s.Remove(s.Length - "one".Length) + "first";
            //second, twenty second
            if (s.EndsWith("two"))
                return s.Remove(s.Length - "two".Length) + "second";
            //third, twenty third
            if (s.EndsWith("three"))
                return s.Remove(s.Length - "three".Length) + "third";

            //numbers to modify
            if (s.EndsWith("five"))
                return s.Remove(s.Length - "five".Length) + "fifth";
            if (s.EndsWith("nine"))
                return s.Remove(s.Length - "nine".Length) + "ninth";
            if (s.EndsWith("twelve"))
                return s.Remove(s.Length - "twelve".Length) + "twelfth";

            //remove "y" and add "ieth" (twenty, twentieth - thirty, thirtieth...)
            if (s.EndsWith("twenty") || s.EndsWith("thirty") || s.EndsWith("forty") || s.EndsWith("fifty") || s.EndsWith("sixty") || s.EndsWith("seventy") || s.EndsWith("eighty") || s.EndsWith("ninety"))
                return s.Remove(s.Length - 1) + "ieth";

            //every other number just add "th" add the end (four, fourth - six, sixth...)
            return s + "th";
        }
    }
}