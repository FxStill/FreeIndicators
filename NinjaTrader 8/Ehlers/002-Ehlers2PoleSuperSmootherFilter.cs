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
	public class Ehlers2PoleSuperSmootherFilter : Indicator
	{
		private double c1, c2, c3, sqrt2;
		private int MinBar;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"The 2 Pole Super Smoother Filter: John Ehlers, Cybernetic Analysis For Stocks And Futures,  pg.202";
				Name										= "Ehlers2PoleSuperSmootherFilter";
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
				Length					                    = 15;
			    bColor                                      = Brushes.DodgerBlue;
				sColor                                      = Brushes.Crimson;				
				AddPlot(Brushes.OliveDrab, "PoleSuperSmootherFilter");
			}
			else if (State == State.Configure)
			{
				sqrt2 = Math.Sqrt(2);
   				double a1 = Math.Exp(-sqrt2 * Math.PI / Length);
   				double b1 = 2 * a1 * Math.Cos(sqrt2 * Math.PI / Length);
   				c2 = b1;
   				c3 = -Math.Pow(a1, 2);
   				c1 = 1 - c2 - c3;				
				MinBar = 3;
			}
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar <= MinBar) return;
		    ssf[0] = c1 * Median[0] + c2 * ssf[1] + c3 * ssf[2];
		    if (ssf[0] < Median[0]) PlotBrushes[0][0] = bColor;
    		else
      		   	if (ssf[0] > Median[0]) 	   
			   		PlotBrushes[0][0] = sColor;			

		}

		#region Properties
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Length", Description="Length / Period", Order=1, GroupName="Parameters")]
		public int Length
		{ get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> ssf
		{
			get { return Values[0]; }
		}
		
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
		private AUN_Indi.Ehlers.Ehlers2PoleSuperSmootherFilter[] cacheEhlers2PoleSuperSmootherFilter;
		public AUN_Indi.Ehlers.Ehlers2PoleSuperSmootherFilter Ehlers2PoleSuperSmootherFilter(int length, Brush bColor, Brush sColor)
		{
			return Ehlers2PoleSuperSmootherFilter(Input, length, bColor, sColor);
		}

		public AUN_Indi.Ehlers.Ehlers2PoleSuperSmootherFilter Ehlers2PoleSuperSmootherFilter(ISeries<double> input, int length, Brush bColor, Brush sColor)
		{
			if (cacheEhlers2PoleSuperSmootherFilter != null)
				for (int idx = 0; idx < cacheEhlers2PoleSuperSmootherFilter.Length; idx++)
					if (cacheEhlers2PoleSuperSmootherFilter[idx] != null && cacheEhlers2PoleSuperSmootherFilter[idx].Length == length && cacheEhlers2PoleSuperSmootherFilter[idx].bColor == bColor && cacheEhlers2PoleSuperSmootherFilter[idx].sColor == sColor && cacheEhlers2PoleSuperSmootherFilter[idx].EqualsInput(input))
						return cacheEhlers2PoleSuperSmootherFilter[idx];
			return CacheIndicator<AUN_Indi.Ehlers.Ehlers2PoleSuperSmootherFilter>(new AUN_Indi.Ehlers.Ehlers2PoleSuperSmootherFilter(){ Length = length, bColor = bColor, sColor = sColor }, input, ref cacheEhlers2PoleSuperSmootherFilter);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.AUN_Indi.Ehlers.Ehlers2PoleSuperSmootherFilter Ehlers2PoleSuperSmootherFilter(int length, Brush bColor, Brush sColor)
		{
			return indicator.Ehlers2PoleSuperSmootherFilter(Input, length, bColor, sColor);
		}

		public Indicators.AUN_Indi.Ehlers.Ehlers2PoleSuperSmootherFilter Ehlers2PoleSuperSmootherFilter(ISeries<double> input , int length, Brush bColor, Brush sColor)
		{
			return indicator.Ehlers2PoleSuperSmootherFilter(input, length, bColor, sColor);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.AUN_Indi.Ehlers.Ehlers2PoleSuperSmootherFilter Ehlers2PoleSuperSmootherFilter(int length, Brush bColor, Brush sColor)
		{
			return indicator.Ehlers2PoleSuperSmootherFilter(Input, length, bColor, sColor);
		}

		public Indicators.AUN_Indi.Ehlers.Ehlers2PoleSuperSmootherFilter Ehlers2PoleSuperSmootherFilter(ISeries<double> input , int length, Brush bColor, Brush sColor)
		{
			return indicator.Ehlers2PoleSuperSmootherFilter(input, length, bColor, sColor);
		}
	}
}

#endregion
