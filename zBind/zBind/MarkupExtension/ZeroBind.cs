using FunctionZero.ExpressionParserZero;
using FunctionZero.ExpressionParserZero.BackingStore;
using FunctionZero.ExpressionParserZero.Operands;
using FunctionZero.ExpressionParserZero.Parser;
using FunctionZero.ExpressionParserZero.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace zBind.MarkupExtension
{
    [ContentProperty("Expression")]
    public class Bind : IMarkupExtension<BindingBase>, INotifyPropertyChanged
    {
        public string Expression { set; get; }
        public BindingMode Mode { get; set; }

        private IList<string> _bindingLookup;

        public Bind()
        {
        }

        /// <summary>
        /// This property acts as a relay.
        /// Xaml properties are bound to it and the markup-extensions interact with
        /// the xaml properties by interacting with this property.
        /// </summary>
        private object _value;
        private MultiBinding _multiBind;

        public event PropertyChangedEventHandler PropertyChanged;

        public object Value
        {
            get => _value;
            set
            {
                if (Equals(value, _value)) return;
                _value = value;
                OnPropertyChanged();
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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
            BindableProperty bindableProperty = pvt.TargetProperty as BindableProperty;

            //bindableTarget.BindingContextChanged += TargetOnBindingContextChanged;

            //bindableTarget.SetValue(bindableProperty, 32);

            var ep = new ExpressionParser();
            ep.RegisterFunction("Sin", DoSin, 1, 1);
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
                            //var binding = new Binding(op.ToString(), BindingMode.TwoWay, null, null, null, bindableTarget);
                            //var binding = new Binding(op.ToString(), BindingMode.OneWay);
                            _bindingLookup.Add(op.ToString());
                            _multiBind.Bindings.Add(binding);
                        }
                    }
                }
            }

            //_multiBind.Bindings = _bindingLookup.Values.ToList();
            _multiBind.Converter = new EvaluatorMultiConverter(_bindingLookup, compiledExpression);

            //TargetOnBindingContextChanged(bindableTarget, EventArgs.Empty);

            return _multiBind;
        }

        private void DoSin(Stack<IOperand> stack, IBackingStore store, long paramCount)
        {
            IOperand first = OperatorActions.PopAndResolve(stack, store);
            double val = (double)first.GetValue();
            var result = Math.Sin(val);
            stack.Push(new Operand(-1, OperandType.Double, result));

        }

        //private void TargetOnBindingContextChanged(object sender, EventArgs e)
        //{
        //    List<BindingBase> bindings = new List<BindingBase>();

        //    foreach(string item in _bindingLookup)
        //    {
        //        bindings.Add(new Binding(item, BindingMode.OneWay, null, null, null, ((BindableObject)sender).BindingContext));
        //    }
        //    _multiBind.Bindings = bindings;
        //}

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        {
            return (this as IMarkupExtension<BindingBase>).ProvideValue(serviceProvider);
        }
    }
}
