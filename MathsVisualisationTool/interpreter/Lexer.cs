﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DataDomain;

namespace MathsVisualisationTool
{

    public class Lexer
    {
        
        public Lexer()
        {

        }

        /*
         * Function to tokenise the user's input. 
         */
        public List<Token> TokeniseInput(string input)
        {

            if(string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
            {
                throw new EmptyInputException("Error: Empty input added.");
            }

            //Variable to record the token type currently stored in the list of characters.
            Globals.SUPPORTED_TOKENS typeInList = getFirstCharType(input[0]);
            List<char> characters = new List<char>() { input[0] };
            int startPos;

            //Because the unicode is 2 characters in length.
            if(typeInList == Globals.SUPPORTED_TOKENS.EULER)
            {
                startPos = 2;
            } else
            {
                startPos = 1;
            }

            //Variable to store the token to be added into the list of tokens.
            Token tokenToAdd;
            List<Token> tokens = new List<Token>();

            //Go through the line of code added by the user.
            int i;
            for(i=startPos;i<input.Length;i++)
            {
                //Is it a digit?
                if(char.IsDigit(input[i]))
                {
                    //Variable names can contain numbers
                    if(typeInList == Globals.SUPPORTED_TOKENS.VARIABLE_NAME)
                    {
                        characters.Add(input[i]);
                    }
                    //If previous character was a constant then just prepend it onto the list
                    //i.e. characters {2,2} would become 22.
                    else if(typeInList.Equals(Globals.SUPPORTED_TOKENS.CONSTANT))
                    {
                        characters.Add(input[i]);
                    } else
                    {
                        //if it was anything else then create a new token and start again.
                        tokenToAdd = TokeniseList(characters, typeInList);
                        //if the word gathered is a keyword.
                        if (Globals.keyWords.Contains(tokenToAdd.GetValue()))
                        {
                            //get the keyword.
                            tokenToAdd.SetType(Globals.getTokenFromKeyword(tokenToAdd.GetValue()));
                        }
                        tokens.Add(tokenToAdd);
                        //Create a new list and set the type to CONSTANT
                        characters = new List<char>(){ input[i] };
                        typeInList = Globals.SUPPORTED_TOKENS.CONSTANT;
                    }
                }

                else if(input[i] == '+')
                {
                    //+ operator is always 2 seperate tokens not ++.
                    tokenToAdd = TokeniseList(characters, typeInList);
                    //if the word gathered is a keyword.
                    if (Globals.keyWords.Contains(tokenToAdd.GetValue()))
                    {
                        //get the keyword.
                        tokenToAdd.SetType(Globals.getTokenFromKeyword(tokenToAdd.GetValue()));
                    }
                    tokens.Add(tokenToAdd);
                    characters = new List<char>() { input[i] };
                    typeInList = Globals.SUPPORTED_TOKENS.PLUS;
                }

                else if (input[i] == '-')
                {
                    //same for - operator
                    tokenToAdd = TokeniseList(characters, typeInList);
                    //if the word gathered is a keyword.
                    if (Globals.keyWords.Contains(tokenToAdd.GetValue()))
                    {
                        //get the keyword.
                        tokenToAdd.SetType(Globals.getTokenFromKeyword(tokenToAdd.GetValue()));
                    }
                    tokens.Add(tokenToAdd);
                    characters = new List<char>() { input[i] };
                    typeInList = Globals.SUPPORTED_TOKENS.MINUS;
                }
                else if (input[i] == '*' || input[i] == '\u00D7')
                {
                    //same for * operator
                    tokenToAdd = TokeniseList(characters, typeInList);
                    //if the word gathered is a keyword.
                    if (Globals.keyWords.Contains(tokenToAdd.GetValue()))
                    {
                        //get the keyword.
                        tokenToAdd.SetType(Globals.getTokenFromKeyword(tokenToAdd.GetValue()));
                    }
                    tokens.Add(tokenToAdd);
                    characters = new List<char>() { input[i] };
                    typeInList = Globals.SUPPORTED_TOKENS.MULTIPLICATION;
                }
                else if(input[i] == '/' || input[i] == '\u00F7')
                {
                    //same for / operator
                    tokenToAdd = TokeniseList(characters, typeInList);
                    //if the word gathered is a keyword.
                    if (Globals.keyWords.Contains(tokenToAdd.GetValue()))
                    {
                        //get the keyword.
                        tokenToAdd.SetType(Globals.getTokenFromKeyword(tokenToAdd.GetValue()));
                    }
                    tokens.Add(tokenToAdd);
                    characters = new List<char>() { input[i] };
                    typeInList = Globals.SUPPORTED_TOKENS.DIVISION;
                }

                else if(input[i] == '(')
                {

                    tokenToAdd = TokeniseList(characters, typeInList);
                    //if the word gathered is a keyword.
                    if (Globals.keyWords.Contains(tokenToAdd.GetValue()))
                    {
                        //get the keyword.
                        tokenToAdd.SetType(Globals.getTokenFromKeyword(tokenToAdd.GetValue()));
                    }
                    tokens.Add(tokenToAdd);
                    characters = new List<char>() { input[i] };
                    typeInList = Globals.SUPPORTED_TOKENS.OPEN_BRACKET;
                }

                else if (input[i] == '^')
                {
                    tokenToAdd = TokeniseList(characters, typeInList);
                    //if the word gathered is a keyword.
                    if (Globals.keyWords.Contains(tokenToAdd.GetValue()))
                    {
                        //get the keyword.
                        tokenToAdd.SetType(Globals.getTokenFromKeyword(tokenToAdd.GetValue()));
                    }
                    tokens.Add(tokenToAdd);
                    characters = new List<char>() { input[i] };
                    typeInList = Globals.SUPPORTED_TOKENS.INDICIES;
                }

                else if(input[i] == ')')
                {
                    tokenToAdd = TokeniseList(characters, typeInList);
                    //if the word gathered is a keyword.
                    if (Globals.keyWords.Contains(tokenToAdd.GetValue()))
                    {
                        //get the keyword.
                        tokenToAdd.SetType(Globals.getTokenFromKeyword(tokenToAdd.GetValue()));
                    }
                    tokens.Add(tokenToAdd);
                    characters = new List<char>() { input[i] };
                    typeInList = Globals.SUPPORTED_TOKENS.CLOSE_BRACKET;
                }

                else if(input[i] == '=')
                {
                    tokenToAdd = TokeniseList(characters, typeInList);
                    //if the word gathered is a keyword.
                    if (Globals.keyWords.Contains(tokenToAdd.GetValue()))
                    {
                        //get the keyword.
                        tokenToAdd.SetType(Globals.getTokenFromKeyword(tokenToAdd.GetValue()));
                    }
                    tokens.Add(tokenToAdd);
                    characters = new List<char>() { input[i] };
                    typeInList = Globals.SUPPORTED_TOKENS.ASSIGNMENT;
                }

                else if (input[i] == ',')
                {
                    tokenToAdd = TokeniseList(characters, typeInList);
                    //if the word gathered is a keyword.
                    if (Globals.keyWords.Contains(tokenToAdd.GetValue()))
                    {
                        //get the keyword.
                        tokenToAdd.SetType(Globals.getTokenFromKeyword(tokenToAdd.GetValue()));
                    }
                    tokens.Add(tokenToAdd);
                    characters = new List<char>() { input[i] };
                    typeInList = Globals.SUPPORTED_TOKENS.COMMA;
                }
                else if (input[i] == '<')
                {
                    tokenToAdd = TokeniseList(characters, typeInList);
                    //if the word gathered is a keyword.
                    if (Globals.keyWords.Contains(tokenToAdd.GetValue()))
                    {
                        //get the keyword.
                        tokenToAdd.SetType(Globals.getTokenFromKeyword(tokenToAdd.GetValue()));
                    }
                    tokens.Add(tokenToAdd);
                    characters = new List<char>() { input[i] };
                    typeInList = Globals.SUPPORTED_TOKENS.LESS_THAN;
                }
                else if (input[i] == '>')
                {
                    tokenToAdd = TokeniseList(characters, typeInList);
                    //if the word gathered is a keyword.
                    if (Globals.keyWords.Contains(tokenToAdd.GetValue()))
                    {
                        //get the keyword.
                        tokenToAdd.SetType(Globals.getTokenFromKeyword(tokenToAdd.GetValue()));
                    }
                    tokens.Add(tokenToAdd);
                    characters = new List<char>() { input[i] };
                    typeInList = Globals.SUPPORTED_TOKENS.GREATER_THAN;
                }
                //Unicode for PI symbol
                else if (input[i] == '\u03C0')
                {
                    tokenToAdd = TokeniseList(characters, typeInList);
                    //if the word gathered is a keyword.
                    if (Globals.keyWords.Contains(tokenToAdd.GetValue()))
                    {
                        //get the keyword.
                        tokenToAdd.SetType(Globals.getTokenFromKeyword(tokenToAdd.GetValue()));
                    }
                    tokens.Add(tokenToAdd);
                    characters = new List<char>() { input[i] };
                    typeInList = Globals.SUPPORTED_TOKENS.PI;
                }
                //Unicode for Euler's number.
                else if (input[i] == '\uD835')
                {
                    //skip because Euler's unicode has 2 characters
                    i++;
                    tokenToAdd = TokeniseList(characters, typeInList);
                    //if the word gathered is a keyword.
                    if (Globals.keyWords.Contains(tokenToAdd.GetValue()))
                    {
                        //get the keyword.
                        tokenToAdd.SetType(Globals.getTokenFromKeyword(tokenToAdd.GetValue()));
                    }
                    tokens.Add(tokenToAdd);
                    characters = new List<char>() { input[i] };
                    typeInList = Globals.SUPPORTED_TOKENS.EULER;
                }
                else if (char.IsLetter(input[i]))
                {
                    //if the previous tokens were variable names
                    if (typeInList.Equals(Globals.SUPPORTED_TOKENS.VARIABLE_NAME))
                    {
                        characters.Add(input[i]);
                    }
                    else
                    {
                        //any other type, create a new token and add it to the list.
                        tokenToAdd = TokeniseList(characters, typeInList);
                        tokens.Add(tokenToAdd);
                        characters = new List<char>() { input[i] };
                        typeInList = Globals.SUPPORTED_TOKENS.VARIABLE_NAME;
                    }
                }
                else if(input[i] == '.')
                {
                    //if it was already a number then just append onto it.
                    if (typeInList.Equals(Globals.SUPPORTED_TOKENS.CONSTANT))
                    {
                        characters.Add(input[i]);
                    }
                    else
                    {
                        //create a new token and add it to the list.
                        tokenToAdd = TokeniseList(characters, typeInList);
                        //if the word gathered is a keyword.
                        if (Globals.keyWords.Contains(tokenToAdd.GetValue()))
                        {
                            //get the keyword.
                            tokenToAdd.SetType(Globals.getTokenFromKeyword(tokenToAdd.GetValue()));
                        }
                        tokens.Add(tokenToAdd);
                        characters = new List<char>() { input[i] };
                        typeInList = Globals.SUPPORTED_TOKENS.CONSTANT;
                    }
                }
                else if(char.IsWhiteSpace(input[i]))
                {
                    //if whitespace then create a token for it.
                    tokenToAdd = TokeniseList(characters, typeInList);
                    //if the word gathered is a keyword.
                    if(Globals.keyWords.Contains(tokenToAdd.GetValue()))
                    {
                        //get the keyword.
                        tokenToAdd.SetType(Globals.getTokenFromKeyword(tokenToAdd.GetValue()));
                    }
                    tokens.Add(tokenToAdd);
                    
                    //find the next character that is not whitespace
                    while(char.IsWhiteSpace(input[i]))
                    {
                        i++;
                        if(i == input.Length)
                        {
                            break;
                        }
                    }

                    //As long as we haven't hit the end of the string.
                    //We don't want those array index out of bounds tings.
                    if( i != input.Length)
                    {
                        typeInList = getFirstCharType(input[i]);
                        characters = new List<char>() { input[i] };
                    }
                } else
                {
                    throw new UnknownCharacterException("Unknown character - " + input[i] + " .");
                }
            }

            //tokenise anything left over.
            tokenToAdd = TokeniseList(characters, typeInList);
            //if the word gathered is a keyword.
            if (Globals.keyWords.Contains(tokenToAdd.GetValue()))
            {
                //get the keyword.
                tokenToAdd.SetType(Globals.getTokenFromKeyword(tokenToAdd.GetValue()));
            }
            tokens.Add(tokenToAdd);

            //PrintTokens(tokens);

            return tokens;
        }

        /// <summary>
        /// Function to get the token type based on the first character given.
        /// </summary>
        /// <returns></returns>
        private Globals.SUPPORTED_TOKENS getFirstCharType(char c)
        {
            if (char.IsDigit(c))
            {
                return Globals.SUPPORTED_TOKENS.CONSTANT;
            }
            else if (c == '+')
            {
                return Globals.SUPPORTED_TOKENS.PLUS;
            }
            else if (c == '-')
            {
                return Globals.SUPPORTED_TOKENS.MINUS;
            }
            else if (c == '*' || c == '\u00D7')
            {
                return Globals.SUPPORTED_TOKENS.MULTIPLICATION;
            }
            else if (c == '/' || c == '\u00F7')
            {
                return Globals.SUPPORTED_TOKENS.DIVISION;
            }
            else if (c == '(')
            {
                return Globals.SUPPORTED_TOKENS.OPEN_BRACKET;
            }
            else if (c == '^')
            {
                return Globals.SUPPORTED_TOKENS.INDICIES;
            }
            else if (c == ')')
            {
                return Globals.SUPPORTED_TOKENS.CLOSE_BRACKET;
            }
            else if (c == '=')
            {
                return Globals.SUPPORTED_TOKENS.ASSIGNMENT;
            }
            else if (c == '\u03C0')
            {
                return Globals.SUPPORTED_TOKENS.PI;
            }
            else if (c == '\uD835')
            {
                return Globals.SUPPORTED_TOKENS.EULER;
            }
            else if (char.IsLetter(c))
            {
                return Globals.SUPPORTED_TOKENS.VARIABLE_NAME;
            }
            else if (c == '.')
            {
                return Globals.SUPPORTED_TOKENS.CONSTANT;
            }
            else if (c == ',')
            {
                return Globals.SUPPORTED_TOKENS.CONSTANT;
            }
            else if (c == '<')
            {
                return Globals.SUPPORTED_TOKENS.LESS_THAN;
            }
            else if (c == '>')
            {
                return Globals.SUPPORTED_TOKENS.GREATER_THAN;
            }
            else
            {
                throw new UnknownCharacterException("Unknown character - " + c + " .");
            }
        }

        /// <summary>
        /// Method to print out the list of collected tokens. Mainly for debugging purposes.
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        private void PrintTokens(List<Token> tokens)
        {
            foreach (Token t in tokens)
            {
                Console.WriteLine(t.ToString());
            }

            Console.WriteLine("--------------------------------------------------");
        }

        /// <summary>
        /// Convert the array of characters into a list of tokens.
        /// </summary>
        /// <param name="listOfCharacters"></param>
        /// <param name="listType"></param>
        /// <returns></returns>
        private Token TokeniseList(List<char>listOfCharacters, Globals.SUPPORTED_TOKENS listType)
        {
            var value = "";

            if(listType == Globals.SUPPORTED_TOKENS.PI)
            {
                return new Token(Globals.SUPPORTED_TOKENS.CONSTANT, Convert.ToString(Math.PI));
            }

            if (listType == Globals.SUPPORTED_TOKENS.EULER)
            {
                return new Token(Globals.SUPPORTED_TOKENS.CONSTANT, Convert.ToString(Math.E));
            }

            foreach (char c in listOfCharacters)
            {
                value += c;
            }

            return new Token(listType,value);
        }
    }
    
}
