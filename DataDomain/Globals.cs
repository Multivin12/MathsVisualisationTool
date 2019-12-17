﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDomain
{
    public static class Globals
    {
        public enum SUPPORTED_TOKENS
        {
            CONSTANT, //supported data types
            PLUS, MINUS, DIVISION, MULTIPLICATION, ASSIGNMENT, INDICIES, //supported ops.
            VARIABLE_NAME, //tokens related to the assignment of variables
            WHITE_SPACE, OPEN_BRACKET, CLOSE_BRACKET, COMMA, LESS_THAN, //miscellaneous characters.
            PLOT //supported functions
        };

        //record the string rep of the keywords.
        public static List<string> keyWords = new List<string>() { "plot" };
        //record the SUPPORTED_TOKENS rep of the keywords.
        public static List<SUPPORTED_TOKENS> keyWordTokens = new List<SUPPORTED_TOKENS>() { SUPPORTED_TOKENS.PLOT };
        //record the order of each operation.
        public static List<SUPPORTED_TOKENS> orderOfOperators = new List<SUPPORTED_TOKENS>()
        {SUPPORTED_TOKENS.INDICIES,SUPPORTED_TOKENS.DIVISION, SUPPORTED_TOKENS.MULTIPLICATION, SUPPORTED_TOKENS.PLUS,SUPPORTED_TOKENS.MINUS};

        /// <summary>
        /// Function to get the token type based on a given word.
        /// Must be a keyword otherwise an ArgumentException is thrown.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static SUPPORTED_TOKENS getTokenFromKeyword(string word)
        {
            if(word == "plot")
            {
                return SUPPORTED_TOKENS.PLOT;
            } else
            {
                //Normally shouldn't happen but if someone else makes a mistake then this
                //should hopefully be clear enough.
                throw new ArgumentException("Word given is not a keyword.");
            }
        }

        /// <summary>
        /// Function used to round a double to X sig figures because C# doesn't have this built in.
        /// This solution was taken from https://stackoverflow.com/questions/374316/round-a-double-to-x-significant-figures.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        public static double RoundToSignificantDigits(double d,int digits)
        {
            if (d == 0)
            {
                return 0;
            }
                
            double scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(d))) + 1);
            return scale * Math.Round(d / scale, digits);
        }
    }
}
