#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

///+--------------------------------------------------------------------------------------------------+
///|   Site:     https://fxstill.com                                                                  |
///|   Telegram: https://t.me/fxstill (Literature on cryptocurrencies, development and code. )        |
///|                                   Don't forget to subscribe!                                     |
///+--------------------------------------------------------------------------------------------------+

//This namespace holds Indicators in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Indicators.AUN_Indi.Ehlers
{
	public class Ehlers2PoleButterworthFilter : Indicator
	{
		public double a, b, c1, c2, c3;
		private int MinBar;
//		private double ml1;	
//		private double pi2;
//		private double pi4;
		private double sqrt2;		

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"The 2 Pole Butterworth Filter:\nJohn Ehlers, Cybernetic Analysis For Stocks And Futures, pg.192";
				Name										= "Ehlers2PoleButterworthFilter";
				Calculate									= Calculate.OnBarClose;
				IsOverlay									= true;
				DisplayInDataBox							= true;
				DrawOnPricePanel							= true;
				DrawHorizontalGridLines						= true;
				DrawVerticalGridLines						= true;
				PaintPriceMarkers							= true;
				ScaleJustification							= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				//Disable this property if your indicator requires custom values that cumulate with each new market data event. 
				//See Help Guide for additional information.
				IsSuspendedWhileInactive					= true;
				Length                                      = 15;
			    bColor                                      = Brushes.DodgerBlue;
				sColor                                      = Brushes.Crimson;				
				AddPlot(Brushes.ForestGreen, "Butterworth2Filter");
			}
			else if (State == State.Configure)
			{
				sqrt2 = Math.Sqrt(2);					
   				a = Math.Exp(-sqrt2 * Math.PI / Length);
   				b = 2 * a * Math.Cos(sqrt2 * Math.PI / Length);
   				c2 = b;
   				c3 = - Math.Pow(a, 2);
   				c1 = (1 - b + Math.Pow(a, 2)) / 4;	
				MinBar = 5;

//				ml1 = Math.Log(10);	
//				pi2 = 2 * Math.PI;
//				pi4 = 4 * Math.PI;
			}
		}

		protected override void OnBarUpdate()
		{
			   if (CurrentBar <= MinBar) return;
			   bf[0] = c1 * (Median[0] + 2 * Median[1] + Median[3]) + c2 * bf[1] + c3 * bf[2];
   			   if (bf[0] < Median[0]) PlotBrushes[0][0] = bColor;
   			   else
      		   	if (bf[0] > Median[0]) 	   
			   		PlotBrushes[0][0] = sColor;
		}

		#region Properties

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> bf
		{
			get { return Values[0]; }
		}
		
		[Range(5, int.MaxValue)]
		[Display(Name="Length", Description="Length / Period", Order=1, GroupName="Indicator")]
		public int Length		{ 	get ; set;	}	
		
        [NinjaScriptProperty]
		[XmlIgnore] 
		[Display(ResourceType = typeof(Custom.Resource), Name="Buy Color", Description="Buy Color", Order=1, GroupName="View")]		
		public  Brush bColor
		{ get; set; }
		[Browsable(false)]
		public string bColorSerialize
		{
			get { return Serialize.BrushToString(bColor); }
			set { bColor = Serialize.StringToBrush(value); }
		}	
		
        [NinjaScriptProperty]
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name="Sell Color", Description="Sell Color", Order=2, GroupName="View")]		
		public  Brush sColor
		{get; set;}				
		[Browsable(false)]
		public string sColorSerialize
		{
			get { return Serialize.BrushToString(sColor); }
			set { sColor = Serialize.StringToBrush(value); }
		}		
		
		#endregion

	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private AUN_Indi.Ehlers.Ehlers2PoleButterworthFilter[] cacheEhlers2PoleButterworthFilter;
		public AUN_Indi.Ehlers.Ehlers2PoleButterworthFilter Ehlers2PoleButterworthFilter(Brush bColor, Brush sColor)
		{
			return Ehlers2PoleButterworthFilter(Input, bColor, sColor);
		}

		public AUN_Indi.Ehlers.Ehlers2PoleButterworthFilter Ehlers2PoleButterworthFilter(ISeries<double> input, Brush bColor, Brush sColor)
		{
			if (cacheEhlers2PoleButterworthFilter != null)
				for (int idx = 0; idx < cacheEhlers2PoleButterworthFilter.Length; idx++)
					if (cacheEhlers2PoleButterworthFilter[idx] != null && cacheEhlers2PoleButterworthFilter[idx].bColor == bColor && cacheEhlers2PoleButterworthFilter[idx].sColor == sColor && cacheEhlers2PoleButterworthFilter[idx].EqualsInput(input))
						return cacheEhlers2PoleButterworthFilter[idx];
			return CacheIndicator<AUN_Indi.Ehlers.Ehlers2PoleButterworthFilter>(new AUN_Indi.Ehlers.Ehlers2PoleButterworthFilter(){ bColor = bColor, sColor = sColor }, input, ref cacheEhlers2PoleButterworthFilter);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.AUN_Indi.Ehlers.Ehlers2PoleButterworthFilter Ehlers2PoleButterworthFilter(Brush bColor, Brush sColor)
		{
			return indicator.Ehlers2PoleButterworthFilter(Input, bColor, sColor);
		}

		public Indicators.AUN_Indi.Ehlers.Ehlers2PoleButterworthFilter Ehlers2PoleButterworthFilter(ISeries<double> input , Brush bColor, Brush sColor)
		{
			return indicator.Ehlers2PoleButterworthFilter(input, bColor, sColor);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.AUN_Indi.Ehlers.Ehlers2PoleButterworthFilter Ehlers2PoleButterworthFilter(Brush bColor, Brush sColor)
		{
			return indicator.Ehlers2PoleButterworthFilter(Input, bColor, sColor);
		}

		public Indicators.AUN_Indi.Ehlers.Ehlers2PoleButterworthFilter Ehlers2PoleButterworthFilter(ISeries<double> input , Brush bColor, Brush sColor)
		{
			return indicator.Ehlers2PoleButterworthFilter(input, bColor, sColor);
		}
	}
}

#endregion
