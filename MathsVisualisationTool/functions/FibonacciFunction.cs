﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataDomain;
using MathFunctionsFSharp;

namespace MathsVisualisationTool
{
    class FibonacciFunction : MathFunction
    {
        /// <summary>
        /// Constructor is the same for the TrigFunction class.
        /// </summary>
        /// <param name="equation"></param>
        public FibonacciFunction(List<Token> equation, int index, bool hasSecondParameter)
            : base(equation, index, hasSecondParameter)
        {
        }

        protected override double Evaluate()
        {
            return MathFunctions.fibonacci((int) Math.Round(Convert.ToDouble(firstParameter.GetValue())));
        }
    }
}
