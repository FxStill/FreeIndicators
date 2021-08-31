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
	public class Ehlers3PoleButterworthFilter : Indicator
	{
		private double a, b, c, cf1, cf2, cf3, cf4;
		private int MinBar;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"The 3 Pole Butterworth Filter: John Ehlers, Cybernetic Analysis For Stocks And Futures,  pg.192";
				Name										= "Ehlers3PoleButterworthFilter";
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
				Length					= 15;
				BColor					= Brushes.DodgerBlue;
				SColor					= Brushes.Crimson;
				AddPlot(Brushes.ForestGreen, "Bf");
			}
			else if (State == State.Configure)
			{
				a   = Math.Exp(-Math.PI / Length);
   				b   = 2 * a * Math.Cos(Math.Sqrt(3) * 180 / Length);
   				c   = Math.Pow(a, 2);
   				cf2 = b + c;
   				cf3 = -(c + b * c);
   				cf4 = Math.Pow(c, 2);
   				cf1 = (1 - b + c) * (1 - c) / 8;
				MinBar = 5;
				
			}
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar <= MinBar) return;
   			Bf[0] = cf1 * (Median[0] + 3 * Median[1] + 3 * Median[2] + Median[3]) + 
                    cf2 * Bf[1] + cf3 * Bf[2] + cf4 * Bf[3];			
		    if (Bf[0] < Median[0]) PlotBrushes[0][0] = BColor;
    		else
      		   	if (Bf[0] > Median[0]) 	   
			   		PlotBrushes[0][0] = SColor;	
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Length", Description="Length / Period", Order=1, GroupName="Parameters")]
		public int Length
		{ get; set; }

		[NinjaScriptProperty]
		[XmlIgnore]
		[Display(Name="BColor", Description="Buy Color", Order=2, GroupName="Parameters")]
		public Brush BColor
		{ get; set; }

		[Browsable(false)]
		public string BColorSerializable
		{
			get { return Serialize.BrushToString(BColor); }
			set { BColor = Serialize.StringToBrush(value); }
		}			

		[NinjaScriptProperty]
		[XmlIgnore]
		[Display(Name="SColor", Description="Sell Color", Order=3, GroupName="Parameters")]
		public Brush SColor
		{ get; set; }

		[Browsable(false)]
		public string SColorSerializable
		{
			get { return Serialize.BrushToString(SColor); }
			set { SColor = Serialize.StringToBrush(value); }
		}			

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> Bf
		{
			get { return Values[0]; }
		}
		#endregion

	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private AUN_Indi.Ehlers.Ehlers3PoleButterworthFilter[] cacheEhlers3PoleButterworthFilter;
		public AUN_Indi.Ehlers.Ehlers3PoleButterworthFilter Ehlers3PoleButterworthFilter(int length, Brush bColor, Brush sColor)
		{
			return Ehlers3PoleButterworthFilter(Input, length, bColor, sColor);
		}

		public AUN_Indi.Ehlers.Ehlers3PoleButterworthFilter Ehlers3PoleButterworthFilter(ISeries<double> input, int length, Brush bColor, Brush sColor)
		{
			if (cacheEhlers3PoleButterworthFilter != null)
				for (int idx = 0; idx < cacheEhlers3PoleButterworthFilter.Length; idx++)
					if (cacheEhlers3PoleButterworthFilter[idx] != null && cacheEhlers3PoleButterworthFilter[idx].Length == length && cacheEhlers3PoleButterworthFilter[idx].BColor == bColor && cacheEhlers3PoleButterworthFilter[idx].SColor == sColor && cacheEhlers3PoleButterworthFilter[idx].EqualsInput(input))
						return cacheEhlers3PoleButterworthFilter[idx];
			return CacheIndicator<AUN_Indi.Ehlers.Ehlers3PoleButterworthFilter>(new AUN_Indi.Ehlers.Ehlers3PoleButterworthFilter(){ Length = length, BColor = bColor, SColor = sColor }, input, ref cacheEhlers3PoleButterworthFilter);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.AUN_Indi.Ehlers.Ehlers3PoleButterworthFilter Ehlers3PoleButterworthFilter(int length, Brush bColor, Brush sColor)
		{
			return indicator.Ehlers3PoleButterworthFilter(Input, length, bColor, sColor);
		}

		public Indicators.AUN_Indi.Ehlers.Ehlers3PoleButterworthFilter Ehlers3PoleButterworthFilter(ISeries<double> input , int length, Brush bColor, Brush sColor)
		{
			return indicator.Ehlers3PoleButterworthFilter(input, length, bColor, sColor);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.AUN_Indi.Ehlers.Ehlers3PoleButterworthFilter Ehlers3PoleButterworthFilter(int length, Brush bColor, Brush sColor)
		{
			return indicator.Ehlers3PoleButterworthFilter(Input, length, bColor, sColor);
		}

		public Indicators.AUN_Indi.Ehlers.Ehlers3PoleButterworthFilter Ehlers3PoleButterworthFilter(ISeries<double> input , int length, Brush bColor, Brush sColor)
		{
			return indicator.Ehlers3PoleButterworthFilter(input, length, bColor, sColor);
		}
	}
}

#endregion
