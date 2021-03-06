﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataDomain;

namespace MathsVisualisationTool
{
    public class Parser
    {
        //List of tokens gathered by the lexer.
        private List<Token> tokens;
        //Variable to store the current token that the parser is analysing.
        private Token nextToken = null;
        //the index of the NEXT token to be analysed when getNextToken() is called.
        private int index = 0;
        //Hashtable for storing a list of known variables.
        //It is stored in the form:
        // key   -> Variable name.
        // value -> Variable value.
        private Hashtable variables = null;
        //Variable to store the newly added variable.
        public string varName = null;
        //Variable for holding a reference to the LiveChartsDrawer.
        private LiveChartsDrawer l = null;
        //Variable for holding a reference to the GraphDrawer (Canvas).
        private GraphDrawer g = null;
        //Write to Variable File?
        public bool WRITE_TO_FILE;
        //Variable to record the result of a command. This is so that the correct message is displayed in the testbox.
        public enum STATUSES{VARIABLE_ASSIGNED,PLOT_FUNCTION_CALLED,UNASSIGNED_RESULT};
        public STATUSES status = STATUSES.UNASSIGNED_RESULT;

        /// <summary>
        /// Create a new parser object. This implementation has the WRITE_TO_FILE flag set to false.
        /// </summary>
        public Parser()
        {
            variables = new Hashtable();
            WRITE_TO_FILE = false;
        }

        /// <summary>
        /// Create a new parser object. This implementation has the WRITE_TO_FILE flag set to true.
        /// </summary>
        public Parser(Hashtable variables)
        {
            this.variables = variables;
            WRITE_TO_FILE = true;
        }

        /// <summary>
        /// Create a new parser object. This implementation has the WRITE_TO_FILE flag set to true.
        /// </summary>
        public Parser(Hashtable variables,ref LiveChartsDrawer l)
        {
            this.variables = variables;
            this.l = l;
            WRITE_TO_FILE = true;
        }

        /// <summary>
        /// Method called by the interpreter to analyse the set of tokens gathered by the lexer.
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns> the value of the given expression.</returns>
        public double AnalyseTokens(List<Token> tokens)
        {
            //check if there are any function definitions present.
            for(int i = 0;i<tokens.Count;i++)
            {
                //plot func has the following syntax:
                //plot(Y=X,Xmin,Xmax,inc)
                //So need to gather the algebraic function, Xmin, Xmax and the increment
                //And pass it into a PlotFunction object for processing.
                if(tokens[i].GetType() == Globals.SUPPORTED_TOKENS.PLOT)
                {
                    //the plot function must be the first token.
                    if (i != 0)
                    {
                        throw new ItemsBeforePlotFunctionException("Plot function found in unexpected token position " + i);
                    }

                    PlotFunction plot = PlotFunction.plotFunctionHandle(tokens, i);

                    if(plot.curIndex != (tokens.Count - 1))
                    {
                        throw new ItemsAfterPlotFunctionException("Nothing can follow after the plot function definition.");
                    }
                    plot.getValues();

                    /////////////////////////////////////////////////////////////////
                    //Draw onto the canvas.
                    this.g = new GraphDrawer(plot, l.window);
                    if (Globals.SHOW_GRAPH_CANVAS)
                    {
                        g.Draw();
                    }
                    /////////////////////////////////////////////////////////////////

                    //Draw it also onto LiveCharts.
                    l.dataPoints = g.plotFunc.dataPoints;
                    l.canvasXLabels = g.Xlabels;
                    l.Xname = g.plotFunc.X;
                    l.Yname = g.plotFunc.Y;
                    if(Globals.SHOW_LIVE_CHARTS)
                    {
                        l.Draw();
                    }

                    status = STATUSES.PLOT_FUNCTION_CALLED;
                    return double.NaN;
                } else if(tokens[i].GetType() == Globals.SUPPORTED_TOKENS.SIN)
                {
                    //create the sin function and find a value.
                    SinFunction s = new SinFunction(tokens, i,false);
                    tokens = s.getNewEquation();
                }
                else if (tokens[i].GetType() == Globals.SUPPORTED_TOKENS.COS)
                {
                    CosFunction c = new CosFunction(tokens, i, false);
                    tokens = c.getNewEquation();
                }
                else if (tokens[i].GetType() == Globals.SUPPORTED_TOKENS.TAN)
                {
                    TanFunction t = new TanFunction(tokens, i, false);
                    tokens = t.getNewEquation();
                }
                else if (tokens[i].GetType() == Globals.SUPPORTED_TOKENS.LOG)
                {
                    LogFunction l = new LogFunction(tokens, i, true);
                    tokens = l.getNewEquation();
                }
                else if (tokens[i].GetType() == Globals.SUPPORTED_TOKENS.LN)
                {
                    LnFunction l = new LnFunction(tokens, i, false);
                    tokens = l.getNewEquation();
                }
                else if (tokens[i].GetType() == Globals.SUPPORTED_TOKENS.SQRT)
                {
                    SqrtFunction s = new SqrtFunction(tokens, i, false);
                    tokens = s.getNewEquation();
                }
                else if (tokens[i].GetType() == Globals.SUPPORTED_TOKENS.ROOT)
                {
                    RootFunction r = new RootFunction(tokens, i, true);
                    tokens = r.getNewEquation();
                }
                else if (tokens[i].GetType() == Globals.SUPPORTED_TOKENS.ABS)
                {
                    AbsFunction a = new AbsFunction(tokens, i, false);
                    tokens = a.getNewEquation();
                } else if (tokens[i].GetType() == Globals.SUPPORTED_TOKENS.FIBONACCI)
                {
                    FibonacciFunction f = new FibonacciFunction(tokens, i, false);
                    tokens = f.getNewEquation();
                }
                else if (tokens[i].GetType() == Globals.SUPPORTED_TOKENS.FACTORIAL)
                {
                    FactorialFunction f = new FactorialFunction(tokens, i, false);
                    tokens = f.getNewEquation();
                }
            }
            return processTokens(tokens);
        }

        /// <summary>
        /// Method called to process an expression.
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        private double processTokens(List<Token> tokens)
        {
            this.tokens = tokens;

            //Analyse the syntax to see if it is valid.
            SyntaxAnalyser s = new SyntaxAnalyser(tokens);
            s.checkTokens();

            //If it is then do some preprocessing on the input.
            Preprocessor p = new Preprocessor(tokens);
            this.tokens = p.processTokens();

            getNextToken();

            double value = analyseExpressions(double.NaN);

            if(WRITE_TO_FILE)
            {
                VariableFileHandle.saveVariables(variables);
            }
            
            return value;
        }

        /// <summary>
        /// Method called to process the expression. It assumes that it is in a universal format
        /// as the checking is done by the preprocessor and syntax analyser.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private double analyseExpressions(double value)
        {
            while (nextToken != null)
            {
                Globals.SUPPORTED_TOKENS tokenType = nextToken.GetType();
                if (tokenType == Globals.SUPPORTED_TOKENS.CONSTANT)
                {
                    //If the next token is an int, then just collect the value
                    value = Convert.ToDouble(nextToken.GetValue());
                    getNextToken();
                }
                else if (tokenType == Globals.SUPPORTED_TOKENS.DIVISION || tokenType == Globals.SUPPORTED_TOKENS.MULTIPLICATION ||tokenType == Globals.SUPPORTED_TOKENS.INDICIES)
                {
                    //If its division or multiplication, run the appropriate method.
                    value = divisionAndMultHandle(value);
                }
                else if (tokenType == Globals.SUPPORTED_TOKENS.PLUS || tokenType == Globals.SUPPORTED_TOKENS.MINUS)
                {
                    //same for plus and minus.
                    value = plusAndMinusHandle(value);
                }
                else if (tokenType == Globals.SUPPORTED_TOKENS.OPEN_BRACKET)
                {
                    //if its an open bracket then recursively call on the expression encapsulated inside
                    //the brackets.
                    getNextToken();
                    value = analyseExpressions(value);
                }
                else if (tokenType == Globals.SUPPORTED_TOKENS.CLOSE_BRACKET)
                {
                    //if its a close bracket then just return.
                    getNextToken();
                    return value;
                }
                else if (tokenType == Globals.SUPPORTED_TOKENS.VARIABLE_NAME)
                {
                    value = variableHandle(value);
                }
            }
            return value;
        }

        /// <summary>
        /// Function for handling when a new variable is being declared.
        /// </summary>
        private double variableHandle(double value)
        {
            //next token will be its name
            string name = nextToken.GetValue();
            Token peekToken = peek();

            //check if its the last token or is not an assignment operation
            if(peekToken is null || !(peekToken.GetType() == Globals.SUPPORTED_TOKENS.ASSIGNMENT))
            {
                //if not then see if it has been assigned.
                getNextToken();
                try
                {
                    VariableRef var = new VariableRef(name);
                    value = var.Evaluate(variables);
                    return value;
                }
                catch (Exception e)
                {
                    //this means it hasn't been assigned yet.
                    throw new VariableReferenceException(e.Message);
                }
            } else
            {
                getNextToken();
                //now pointing to assignment operator.
                getNextToken();
                //now pointing to the start of the expression.

                //get the expression.
                double val = analyseExpressions(double.NaN);
                string varValue = null;

                varValue = Convert.ToString(val);
                
                variables[name] = varValue;

                varName = name;

                status = STATUSES.VARIABLE_ASSIGNED;

                return value;
            }
        }

        /// <summary>
        /// Function for handling division and multiplication when that operator has been found.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>  Returns the resuling value of that operator. </returns>
        private double divisionAndMultHandle(double value)
        {

            Expression left;
            Expression right;
            Globals.SUPPORTED_TOKENS op = nextToken.GetType();

            //Find the left and right hand side of the expression.
            left = new Constant(value);

            //get the next token
            getNextToken();

            double rightValue = 0.0;

            //If a bracket has been found then analyse the expression inside the bracket first.
            if (nextToken.GetType() == Globals.SUPPORTED_TOKENS.OPEN_BRACKET)
            {
                getNextToken();
                rightValue = analyseExpressions(value);
            }
            else
            {
                if(nextToken.GetType() == Globals.SUPPORTED_TOKENS.CONSTANT)
                {
                    //its just a number so extract the value of it.
                    rightValue = Convert.ToDouble(nextToken.GetValue());
                } else
                {
                    //Gathered token is a variable reference
                    VariableRef v = new VariableRef(nextToken.GetValue());
                    try
                    {
                        rightValue = v.Evaluate(variables);
                    } catch (Exception e)
                    {
                        throw new VariableReferenceException(e.Message);
                    }
                }
            }

            right = new Constant(rightValue);

            Expression ne = new Operation(left, op, right);

            getNextToken();

            return ne.Evaluate(variables);
        }

        /// <summary>
        /// Function for handling the plus and minus operator when one has been found.
        /// </summary>
        /// <param name="value"></param>
        /// <returns> Returns the new value of the resulting calculation. </returns>
        private double plusAndMinusHandle(double value)
        {
            Expression left = null;
            Expression right = null;
            Globals.SUPPORTED_TOKENS op = nextToken.GetType();

            //Get the left hand side of the expression.
            left = new Constant(value);

            //Now look at the token after the operator
            getNextToken();

            if (nextToken.GetType() == Globals.SUPPORTED_TOKENS.CONSTANT)
            {
                //A number so just extract the value from it.
                right = new Constant(Convert.ToDouble(nextToken.GetValue()));
                //Then call get next token.
                getNextToken();
            }
            else if (nextToken.GetType() == Globals.SUPPORTED_TOKENS.OPEN_BRACKET)
            {
                getNextToken();
                //recursively call to analyse the expression enclosed in brackets.
                right = new Constant(analyseExpressions(value));
            } else if (nextToken.GetType() == Globals.SUPPORTED_TOKENS.VARIABLE_NAME)
            {
                //Found a variable reference.
                VariableRef v = new VariableRef(nextToken.GetValue());
                try
                {
                    double rightValue = v.Evaluate(variables);
                    right = new Constant(rightValue);
                }
                catch (Exception e)
                {
                    throw new VariableReferenceException(e.Message);
                }
                getNextToken();
            }

            Expression ne = new Operation(left, op, right);
            return ne.Evaluate(variables);
        }

        /// <summary>
        /// Private function for getting the next token identified by the lexer.
        /// Sets the nextToken as null if there is no next token.
        /// </summary>
        private void getNextToken()
        {
            if (index < tokens.Count)
            {
                nextToken = tokens[index];
                index++;
            }
            else
            {
                nextToken = null;
            }
        }

        /// <summary>
        /// Private function to peek at the next token. Returns the next token without
        /// setting nextToken. Returns null if there is no next token.
        /// </summary>
        /// <returns></returns>
        private Token peek()
        {
            if (index < tokens.Count)
            {
                return tokens[index];
            }
            else
            {
                return null;
            }
        }
    }
}
