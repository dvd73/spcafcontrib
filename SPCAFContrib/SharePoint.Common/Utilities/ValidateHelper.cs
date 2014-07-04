using System;
using System.Text.RegularExpressions;

namespace SharePoint.Common.Validators
{
    public static class ValidateHelper
    {
        /// <summary>
        /// Validate that input string is email address
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool ValidateEmail(String email)
        {
            if (email.Length > 0)
            {
                Regex re = new Regex(@"[\w\-\.]+@[\w\-\.]+\.[a-zA-Z]{2,}",
                    RegexOptions.Compiled | RegexOptions.IgnoreCase);

                return re.IsMatch(email);
            }
            else
                return false;

        }
        
        /// <summary>
        /// Validate that input string is email address
        /// Execute more stronger validation then ValidateEmail method
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool ValidateEmailEx(String email)
        {
            return new Regex("^[a-zA-Z0-9.]+@[a-zA-Z0-9]+.[a-z]+$").IsMatch(email);
        }
        
        /// <summary>
        /// Validate that input string is presented URL.
        /// </summary>
        /// <param name="checkedString"></param>
        /// <returns></returns>
        public static bool ValidateUrl(String checkedString)
        {
            return new Regex("^[(http|https)://]*[a-zA-Z0-9.]+[:0-9]*.[a-zA-Z0-9]+/[a-zA-Z0-9/.-_!^%#&*()+=|]*").IsMatch(checkedString);
        }

        /// <summary>
        /// Validate credit card number 
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <param name="cardType"></param>
        /// <returns></returns>
        private static bool ValidateCreditCard(String cardNumber, String cardType)
        {
            bool isValid = !Regex.IsMatch(cardNumber, "[^\\d -]");
            string cardNumbersOnly = Regex.Replace(cardNumber, "[ -]", string.Empty);
            int cardNumberLength = cardNumbersOnly.Length;

            cardType = Regex.Replace(cardType, "[ ]", string.Empty).ToLower();

            if (isValid)
            {
                bool lengthIsValid = true;
                string prefixRegExp;

                if (Regex.IsMatch(cardType, "mastercard"))
                {
                    lengthIsValid = (cardNumberLength == 16);
                    prefixRegExp = "^5[1-5]";
                }
                else if (Regex.IsMatch(cardType, "visa"))
                {
                    lengthIsValid = (cardNumberLength == 16 || cardNumberLength == 13);
                    prefixRegExp = "^4";
                }
                else if (Regex.IsMatch(cardType, "amex|americanexpress"))
                {
                    lengthIsValid = (cardNumberLength == 15);
                    prefixRegExp = "^3(4|7)";
                }
                else if (Regex.IsMatch(cardType, "enroute"))
                {
                    //lengthIsValid = (cardNumberLength == 15);
                    prefixRegExp = "^(2014|2149)";
                }
                else if (Regex.IsMatch(cardType, "diners{0,1}club|carteblanche'"))
                {
                    lengthIsValid = (cardNumberLength == 14);
                    prefixRegExp = "^3(0[0-5]|6|8)";
                }
                else if (Regex.IsMatch(cardType, "discover"))
                {
                    lengthIsValid = (cardNumberLength == 16);
                    prefixRegExp = "^6011";
                }
                else
                {
                    prefixRegExp = ".*";
                }

                bool prefixIsValid = Regex.IsMatch(cardNumbersOnly, prefixRegExp);
                isValid = prefixIsValid && lengthIsValid;
            }

            if (isValid)
            {
                string numberProduct;
                int digitCounter;
                int checkSumTotal = 0;

                for (digitCounter = cardNumberLength - 1; digitCounter >= 0; digitCounter--)
                {
                    checkSumTotal += int.Parse(cardNumbersOnly[digitCounter].ToString());
                    digitCounter--;
                    if (digitCounter >= 0)
                    {
                        numberProduct = (int.Parse(cardNumbersOnly[digitCounter].ToString()) * 2).ToString();
                        for (int productDigitCounter = 0; productDigitCounter < numberProduct.Length; productDigitCounter++)
                        {
                            checkSumTotal += int.Parse(numberProduct[productDigitCounter].ToString());
                        }
                    }
                }

                isValid = (checkSumTotal % 10 == 0);
            }

            return isValid;
        }

        /// <summary>
        /// Validate credit card number using LUHN formula
        /// </summary>
        /// <param name="cardNumber">Credit card number</param>
        /// <returns>true is the number is correct otherwise false</returns>
        public static bool ValidateLUHN(String cardNumber)
        {
            bool isValid = false;
            if (Regex.IsMatch(cardNumber, "^[0-9]{2,}$"))
            {
                // calculate LUHN number
                int checkNumber = 0, currentDigit = 0;
                for (int i = cardNumber.Length - 1; i >= 0; i--)
                {
                    currentDigit = System.Convert.ToInt32(cardNumber.Substring(i, 1));
                    if (i % 2 == cardNumber.Length % 2)
                        currentDigit *= 2;
                    checkNumber += currentDigit / 10 + currentDigit % 10;
                };
                // verify the calculated number
                isValid = (checkNumber % 10 == 0);
            }
            return isValid;
        }

        /// <summary>
        /// Validate credit card number prefix against credit card type
        /// </summary>
        /// <param name="cardNumber">Credit card number</param>
        /// <param name="cardType">Credit card type. One of the "Visa", "American Express", "Discover", "MasterCard"</param>
        /// <returns>true is the credit card prefix matchs credit card type otherwise false</returns>
        public static bool ValidateCardPrefix(string cardNumber, string cardType)
        {
            // American Express
            if (cardType == "American Express" &&
                Regex.IsMatch(cardNumber, "^3[47][0-9]{13}$"))
                return true;

            // Visa
            if (cardType == "Visa" &&
                Regex.IsMatch(cardNumber, "^4(([0-9]{15})|([0-9]{12}))$"))
                return true;

            // MasterCard
            if (cardType == "MasterCard" &&
                Regex.IsMatch(cardNumber, "^5[1-5][0-9]{14}$"))
                return true;

            // Discover
            if (cardType == "Discover" &&
                Regex.IsMatch(cardNumber, "^6011[0-9]{12}$"))
                return true;

            return false;
        }

        /// <summary>
        /// This method returns true if the passed string contains only alphanumeric characters
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsAlphanumeric(string text)
        {
            if (text == string.Empty || text == null)
            {
                return false;
            }
            text = text.Trim();
            return Regex.IsMatch(text, @"^\d+$");
        }

        /// <summary>
        /// Validate that input string contains only digits.
        /// </summary>
        /// <param name="checkedString"></param>
        /// <returns></returns>
        public static bool IsNumber(String checkedString)
        {
            return new Regex("^[0-9.,]+$").IsMatch(checkedString);
        }

        /// <summary>
        /// Validate that input coordinates contains only digits.
        /// </summary>
        /// <param name="checkedString"></param>
        /// <returns></returns>
        public static bool IsCoordinates(String checkedString)
        {
            return new Regex("^[-]*[0-9]+.[0-9]+$").IsMatch(checkedString);
        }

        /// <summary>
        /// Validate that a string is a valid GUID
        /// </summary>
        /// <param name="GUIDCheck"></param>
        /// <returns></returns>
        public static bool IsValidGUID(string GUIDCheck)
        {
            if (!string.IsNullOrEmpty(GUIDCheck))
            {
                return new Regex(@"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$").IsMatch(GUIDCheck);
            }
            return false;
        }
    }
}
