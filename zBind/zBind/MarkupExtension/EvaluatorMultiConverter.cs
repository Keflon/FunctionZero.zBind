﻿using FunctionZero.ExpressionParserZero.Operands;
using FunctionZero.ExpressionParserZero.Parser;
using System;
using System.Collections.Generic;
using System.Globalization;
using Xamarin.Forms;

namespace zBind.MarkupExtension
{
    internal class EvaluatorMultiConverter : IMultiValueConverter
    {
        private readonly VariableEvaluator _evaluator;
        private readonly TokenList _compiledExpression;

        public EvaluatorMultiConverter(ICollection<string> keys, TokenList compiledExpression)
        {
            _evaluator = new VariableEvaluator(new List<string>(keys));
            _compiledExpression = compiledExpression;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            _evaluator.SetValues(values);

            try
            {
                var stack = _compiledExpression.Evaluate(_evaluator);

                var operand = stack.Pop();

                if (operand.Type == OperandType.Variable)
                {
                    var valueAndType = _evaluator.GetValue((string)operand.GetValue());

                    return valueAndType.value;
                }
                return operand.GetValue();

            }
            catch(Exception ex)
            {
                if (targetType.IsValueType && Nullable.GetUnderlyingType(targetType) == null)
                    return Activator.CreateInstance(targetType);
                else
                    return null;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}