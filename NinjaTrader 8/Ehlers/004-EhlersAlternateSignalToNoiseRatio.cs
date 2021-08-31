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
	public class EhlersAlternateSignalToNoiseRatio : Indicator
	{
		private Series<double> range;   //Noise 
		private int MinBar;
		private double k1;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"The Alternate Signal To Noise Ratio: John Ehlers, Rocket Science For Traders,  pg.84-85";
				Name										= "EhlersAlternateSignalToNoiseRatio";
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
				IsAutoScale                                 = true;
				BColor					= Brushes.DodgerBlue;
				SColor					= Brushes.Crimson;
				AddPlot(Brushes.ForestGreen, "Snr");
			}
			else if (State == State.Configure)
			{
				range =  new Series<double>(this);
				MinBar = 3;
				k1 = 10 / Math.Log(10);

			}
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar <= MinBar) return;
			
		    range[0] = 0.1 * (High[0] - Low[0]) + 0.9 * range[1];
			double s1 = 0;
		    if (range[0] <= 0) 
				Snr[0] = 0;
   			else {
				s1 = Snr.IsValidDataPoint(1)? Snr[1]: 0;
				double e = EhlersHilbertTransform(Input).CycleRe[0] + EhlersHilbertTransform(Input).CycleIm[0];
				if (e == 0) 
					Snr[0] = 0;
				else
				   Snr[0] =  0.25 * (k1 * Math.Log(e / Math.Pow(range[0], 2)) + 6) + 0.75 * s1 ;				
   			}
		}

		#region Properties
		[NinjaScriptProperty]
		[XmlIgnore]
		[Display(Name="BColor", Description="Buy Color", Order=1, GroupName="Parameters")]
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
		[Display(Name="SColor", Description="Sell Color", Order=2, GroupName="Parameters")]
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
		public Series<double> Snr      //Signal to Noise Ratio
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
		private AUN_Indi.Ehlers.EhlersAlternateSignalToNoiseRatio[] cacheEhlersAlternateSignalToNoiseRatio;
		public AUN_Indi.Ehlers.EhlersAlternateSignalToNoiseRatio EhlersAlternateSignalToNoiseRatio(Brush bColor, Brush sColor)
		{
			return EhlersAlternateSignalToNoiseRatio(Input, bColor, sColor);
		}

		public AUN_Indi.Ehlers.EhlersAlternateSignalToNoiseRatio EhlersAlternateSignalToNoiseRatio(ISeries<double> input, Brush bColor, Brush sColor)
		{
			if (cacheEhlersAlternateSignalToNoiseRatio != null)
				for (int idx = 0; idx < cacheEhlersAlternateSignalToNoiseRatio.Length; idx++)
					if (cacheEhlersAlternateSignalToNoiseRatio[idx] != null && cacheEhlersAlternateSignalToNoiseRatio[idx].BColor == bColor && cacheEhlersAlternateSignalToNoiseRatio[idx].SColor == sColor && cacheEhlersAlternateSignalToNoiseRatio[idx].EqualsInput(input))
						return cacheEhlersAlternateSignalToNoiseRatio[idx];
			return CacheIndicator<AUN_Indi.Ehlers.EhlersAlternateSignalToNoiseRatio>(new AUN_Indi.Ehlers.EhlersAlternateSignalToNoiseRatio(){ BColor = bColor, SColor = sColor }, input, ref cacheEhlersAlternateSignalToNoiseRatio);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersAlternateSignalToNoiseRatio EhlersAlternateSignalToNoiseRatio(Brush bColor, Brush sColor)
		{
			return indicator.EhlersAlternateSignalToNoiseRatio(Input, bColor, sColor);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersAlternateSignalToNoiseRatio EhlersAlternateSignalToNoiseRatio(ISeries<double> input , Brush bColor, Brush sColor)
		{
			return indicator.EhlersAlternateSignalToNoiseRatio(input, bColor, sColor);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersAlternateSignalToNoiseRatio EhlersAlternateSignalToNoiseRatio(Brush bColor, Brush sColor)
		{
			return indicator.EhlersAlternateSignalToNoiseRatio(Input, bColor, sColor);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersAlternateSignalToNoiseRatio EhlersAlternateSignalToNoiseRatio(ISeries<double> input , Brush bColor, Brush sColor)
		{
			return indicator.EhlersAlternateSignalToNoiseRatio(input, bColor, sColor);
		}
	}
}

#endregion
