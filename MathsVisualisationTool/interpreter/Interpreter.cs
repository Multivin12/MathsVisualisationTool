﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataDomain;

namespace MathsVisualisationTool
{
    class Interpreter
    {

        private Lexer lexer;
        private Parser parser;
        private Hashtable vars;

        public Interpreter()
        {
            lexer = new Lexer();
            vars = VariableFileHandle.getVariables();
            parser = new Parser(vars);
        }

        /// <summary>
        /// Method to run the interpreter given a certain input.
        /// </summary>
        /// <param name="codeToRun"></param>
        /// <returns></returns>
        public string RunInterpreter(string codeToRun)
        {
            //Use the lexer to tokenise the input
            List<Token> tokens = lexer.TokeniseInput(codeToRun);
            //Then put the gathered tokens into the parser.
            double result = parser.AnalyseTokens(tokens);

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //VERY HACKY SOLUTION - WILL OPTIMISE IN LATER COMMITS.
            if(double.IsNaN(result))
            {
                //this means a variable assignment has occured

                //would like to print out say if someone puts x = 3,
                //then output would be:
                // var x =
                //          3
                return "";
            } else
            {
                return "var ANS = \n\t\t" + Convert.ToString(result);
            }
            
        }
    }
}
