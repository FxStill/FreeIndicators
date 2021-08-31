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
	public class EhlersDecycler : Indicator
	{
			private double a, a1, t;
			private int MINBAR = 5;
		
		protected override void OnStateChange()
		{

			if (State == State.SetDefaults)
			{
				Description									= @"The Decycler: John Ehlers, Cycle Analytics For Traders, pg.40-41";
				Name										= "EhlersDecycler";
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
				Length					= 20;
				ColorD					= Brushes.Red;
				ColorU					= Brushes.LimeGreen;
				AddPlot(Brushes.ForestGreen, "Decycler");
			}
			else if (State == State.Configure)
			{
			    t = 2 * Math.PI / Length;
    			a = (Math.Cos(t) + Math.Sin(t) - 1) / Math.Cos(t);
    			a1 = 1 - a;
    			a /= 2;				
			}
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar <= MINBAR) return;
			Decycler[0] = a * (Close[0] + Close[1]) + a1 * Decycler[1];
			
   			if (Decycler[0] < Close[0]) PlotBrushes[0][0] = ColorU; 
   			else
      			if (Decycler[0] > Close[0]) PlotBrushes[0][0] = ColorD; 			
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Length", Description="Length", Order=1, GroupName="Parameters")]
		public int Length
		{ get; set; }

		[NinjaScriptProperty]
		[XmlIgnore]
		[Display(Name="ColorD", Description="Color for bear", Order=2, GroupName="Parameters")]
		public Brush ColorD
		{ get; set; }

		[Browsable(false)]
		public string ColorDSerializable
		{
			get { return Serialize.BrushToString(ColorD); }
			set { ColorD = Serialize.StringToBrush(value); }
		}			

		[NinjaScriptProperty]
		[XmlIgnore]
		[Display(Name="ColorU", Description="Color for bool", Order=3, GroupName="Parameters")]
		public Brush ColorU
		{ get; set; }

		[Browsable(false)]
		public string ColorUSerializable
		{
			get { return Serialize.BrushToString(ColorU); }
			set { ColorU = Serialize.StringToBrush(value); }
		}			

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> Decycler
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
		private AUN_Indi.Ehlers.EhlersDecycler[] cacheEhlersDecycler;
		public AUN_Indi.Ehlers.EhlersDecycler EhlersDecycler(int length, Brush colorD, Brush colorU)
		{
			return EhlersDecycler(Input, length, colorD, colorU);
		}

		public AUN_Indi.Ehlers.EhlersDecycler EhlersDecycler(ISeries<double> input, int length, Brush colorD, Brush colorU)
		{
			if (cacheEhlersDecycler != null)
				for (int idx = 0; idx < cacheEhlersDecycler.Length; idx++)
					if (cacheEhlersDecycler[idx] != null && cacheEhlersDecycler[idx].Length == length && cacheEhlersDecycler[idx].ColorD == colorD && cacheEhlersDecycler[idx].ColorU == colorU && cacheEhlersDecycler[idx].EqualsInput(input))
						return cacheEhlersDecycler[idx];
			return CacheIndicator<AUN_Indi.Ehlers.EhlersDecycler>(new AUN_Indi.Ehlers.EhlersDecycler(){ Length = length, ColorD = colorD, ColorU = colorU }, input, ref cacheEhlersDecycler);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersDecycler EhlersDecycler(int length, Brush colorD, Brush colorU)
		{
			return indicator.EhlersDecycler(Input, length, colorD, colorU);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersDecycler EhlersDecycler(ISeries<double> input , int length, Brush colorD, Brush colorU)
		{
			return indicator.EhlersDecycler(input, length, colorD, colorU);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersDecycler EhlersDecycler(int length, Brush colorD, Brush colorU)
		{
			return indicator.EhlersDecycler(Input, length, colorD, colorU);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersDecycler EhlersDecycler(ISeries<double> input , int length, Brush colorD, Brush colorU)
		{
			return indicator.EhlersDecycler(input, length, colorD, colorU);
		}
	}
}

#endregion
