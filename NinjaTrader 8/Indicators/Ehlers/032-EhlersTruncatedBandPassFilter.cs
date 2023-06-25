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
	public class EhlersTruncatedBandPassFilter : Indicator
	{
		private int MINBAR;
		private double l1, g1, s1, s11, s12;
		private double[] trunc;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"The Truncated BandPass Filter: John Ehlers, Stocks & Commodities July 2020, pg.48";
				Name										= "EhlersTruncatedBandPassFilter";
				Calculate									= Calculate.OnBarClose;
				IsOverlay									= false;
				DisplayInDataBox							= true;
				DrawOnPricePanel							= true;
				DrawHorizontalGridLines						= true;
				DrawVerticalGridLines						= true;
				PaintPriceMarkers							= true;
				ScaleJustification							= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				//Disable this property if your indicator requires custom values that cumulate with each new market data event. 
				//See Help Guide for additional information.
				IsSuspendedWhileInactive					= true;
				Period					= 20;
				Bandwidth					= 0.1;
				Length					= 10;
				ColorD					= Brushes.Red;
				ColorU					= Brushes.LimeGreen;
				
				AddPlot(Brushes.DarkBlue, "Bpt");
				AddPlot(Brushes.DodgerBlue, "Bp");
				AddLine(Brushes.Goldenrod, 0, "ZerroLine");
			}
			else if (State == State.Configure)
			{
				MINBAR = Length + 3;
				trunc = new double[Period + 3];
				l1    = Math.Cos(2 * Math.PI / Period);
				g1    = Math.Cos(Bandwidth * 2 * Math.PI / Period);
   				s1    = 1 / g1 - Math.Sqrt( (1 / Math.Pow(g1, 2)) - 1);  				
				s11   = 0.5 * (1 - s1);
				s12   = l1 * (1 + s1);
			}
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar <= MINBAR) return;
			
			Bp[0] = s11 * (Close[0] - Close[2]) + s12 * Bp[1] - s1 * Bp[2];
			
			for ( int count = Length; count >= 1; count--) {
				trunc[count] = s11 * (Close[count - 1] - Close[count + 1]) +
			                   s12 * trunc[count + 1] - s1 * trunc[count + 2];
			}		
			Bpt[0] = trunc[1];	
			
	       if (Bpt[0] < 0) PlotBrushes[0][0] = ColorD; 
      	   else
         	 if (Bpt[0] > 0) PlotBrushes[0][0] = ColorU;  			
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Period", Order=1, GroupName="Parameters")]
		public int Period
		{ get; set; }

		[NinjaScriptProperty]
		[Range(0.01, double.MaxValue)]
		[Display(Name="Bandwidth", Order=2, GroupName="Parameters")]
		public double Bandwidth
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Length", Order=3, GroupName="Parameters")]
		public int Length
		{ get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> Bpt
		{
			get { return Values[0]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> Bp
		{
			get { return Values[1]; }
		}
		
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
		#endregion

	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private AUN_Indi.Ehlers.EhlersTruncatedBandPassFilter[] cacheEhlersTruncatedBandPassFilter;
		public AUN_Indi.Ehlers.EhlersTruncatedBandPassFilter EhlersTruncatedBandPassFilter(int period, double bandwidth, int length, Brush colorD, Brush colorU)
		{
			return EhlersTruncatedBandPassFilter(Input, period, bandwidth, length, colorD, colorU);
		}

		public AUN_Indi.Ehlers.EhlersTruncatedBandPassFilter EhlersTruncatedBandPassFilter(ISeries<double> input, int period, double bandwidth, int length, Brush colorD, Brush colorU)
		{
			if (cacheEhlersTruncatedBandPassFilter != null)
				for (int idx = 0; idx < cacheEhlersTruncatedBandPassFilter.Length; idx++)
					if (cacheEhlersTruncatedBandPassFilter[idx] != null && cacheEhlersTruncatedBandPassFilter[idx].Period == period && cacheEhlersTruncatedBandPassFilter[idx].Bandwidth == bandwidth && cacheEhlersTruncatedBandPassFilter[idx].Length == length && cacheEhlersTruncatedBandPassFilter[idx].ColorD == colorD && cacheEhlersTruncatedBandPassFilter[idx].ColorU == colorU && cacheEhlersTruncatedBandPassFilter[idx].EqualsInput(input))
						return cacheEhlersTruncatedBandPassFilter[idx];
			return CacheIndicator<AUN_Indi.Ehlers.EhlersTruncatedBandPassFilter>(new AUN_Indi.Ehlers.EhlersTruncatedBandPassFilter(){ Period = period, Bandwidth = bandwidth, Length = length, ColorD = colorD, ColorU = colorU }, input, ref cacheEhlersTruncatedBandPassFilter);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersTruncatedBandPassFilter EhlersTruncatedBandPassFilter(int period, double bandwidth, int length, Brush colorD, Brush colorU)
		{
			return indicator.EhlersTruncatedBandPassFilter(Input, period, bandwidth, length, colorD, colorU);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersTruncatedBandPassFilter EhlersTruncatedBandPassFilter(ISeries<double> input , int period, double bandwidth, int length, Brush colorD, Brush colorU)
		{
			return indicator.EhlersTruncatedBandPassFilter(input, period, bandwidth, length, colorD, colorU);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersTruncatedBandPassFilter EhlersTruncatedBandPassFilter(int period, double bandwidth, int length, Brush colorD, Brush colorU)
		{
			return indicator.EhlersTruncatedBandPassFilter(Input, period, bandwidth, length, colorD, colorU);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersTruncatedBandPassFilter EhlersTruncatedBandPassFilter(ISeries<double> input , int period, double bandwidth, int length, Brush colorD, Brush colorU)
		{
			return indicator.EhlersTruncatedBandPassFilter(input, period, bandwidth, length, colorD, colorU);
		}
	}
}

#endregion
