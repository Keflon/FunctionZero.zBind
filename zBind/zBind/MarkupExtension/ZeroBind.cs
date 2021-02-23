using FunctionZero.ExpressionParserZero.Operands;
using FunctionZero.ExpressionParserZero.Tokens;
using System;
using System.Collections.Generic;
using System.Xml;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace zBind.MarkupExtension
{
    [ContentProperty("Expression")]
    public class Bind : IMarkupExtension<BindingBase>
    {
        public string Expression { set; get; }
        public BindingMode Mode { get; set; }

        private IList<string> _bindingLookup;

        public Bind()
        {
        }

        private MultiBinding _multiBind;

        public BindingBase ProvideValue(IServiceProvider serviceProvider)
        {
            _bindingLookup = new List<string>();

            if (String.IsNullOrEmpty(Expression))
            {
                IXmlLineInfo lineInfo = (serviceProvider.GetService(typeof(IXmlLineInfoProvider)) is IXmlLineInfoProvider lineInfoProvider) ? lineInfoProvider.XmlLineInfo : new XmlLineInfo();
                throw new XamlParseException("ZeroBind requires 'Expression' property to be set", lineInfo);
            }

            IProvideValueTarget pvt = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            BindableObject bindableTarget = pvt.TargetObject as BindableObject;
            //BindableProperty bindableProperty = pvt.TargetProperty as BindableProperty;

            //bindableTarget.BindingContextChanged += TargetOnBindingContextChanged;
            //bindableTarget.SetValue(bindableProperty, 32);

            var ep = ExpressionParserFactory.GetExpressionParser();

            var compiledExpression = ep.Parse(Expression);

            _multiBind = new MultiBinding();

            foreach (IToken item in compiledExpression)
            {
                if(item is Operand op)
                {
                    if(op.Type == OperandType.Variable)
                    {
                        if(_bindingLookup.Contains(op.ToString()) == false)
                        {
                            var binding = new Binding(op.ToString(), BindingMode.TwoWay, null, null, null, bindableTarget.BindingContext);
                            _bindingLookup.Add(op.ToString());
                            _multiBind.Bindings.Add(binding);
                        }
                    }
                }
            }

            _multiBind.Converter = new EvaluatorMultiConverter(_bindingLookup, compiledExpression);

            return _multiBind;
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        {
            return (this as IMarkupExtension<BindingBase>).ProvideValue(serviceProvider);
        }
    }
}
