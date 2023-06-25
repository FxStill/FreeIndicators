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
	public class EhlersSignalToNoiseRatio : Indicator
	{
		private Series<double> _range;
		private int MINBAR = 5;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"The Signal To Noise Ratio: John Ehlers, Rocket Science For Traders, pg.81-82";
				Name										= "EhlersSignalToNoiseRatio";
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
				ColorD					= Brushes.Red;
				ColorU					= Brushes.LimeGreen;				
				AddLine(Brushes.Goldenrod, 6, "ZerroLine");
				AddPlot(Brushes.DarkBlue, "Snr");
			}
			else if (State == State.Configure)
			{
			}
			else if (State == State.DataLoaded)
			{				
				_range = new Series<double>(this);
			}
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar <= MINBAR) return;
			
			double I1     = EhlersHilbertTransform(Input).I1[0];
			double Q1     = EhlersHilbertTransform(Input).Q1[0];
			double Smooth = EhlersHilbertTransform(Input).Smooth[0];
			if (I1 == 0.0 && Q1 == 0.0) return;
			
			_range[0] = 0.1 * (High[0] - Low[0]) + 0.9 * _range[1];
			
			if (_range[0] <= 0) Snr[0] = 0;
			else {
				double t1 = Math.Log((Math.Pow(I1, 2) + Math.Pow(Q1, 2)) / Math.Pow(_range[0], 2));
				Snr[0] = 0.25 * (10 * t1 / Math.Log(10) + 6) + 0.75 * Snr[1];			
			}	
   			if (Median[0] < Smooth) PlotBrushes[0][0] = ColorD;
   			else
      			if (Median[0] > Smooth) PlotBrushes[0][0] = ColorU;			
		}

		#region Properties


		[Browsable(false)]
		[XmlIgnore]
		public Series<double> Snr
		{
			get { return Values[0]; }
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
		private AUN_Indi.Ehlers.EhlersSignalToNoiseRatio[] cacheEhlersSignalToNoiseRatio;
		public AUN_Indi.Ehlers.EhlersSignalToNoiseRatio EhlersSignalToNoiseRatio(Brush colorD, Brush colorU)
		{
			return EhlersSignalToNoiseRatio(Input, colorD, colorU);
		}

		public AUN_Indi.Ehlers.EhlersSignalToNoiseRatio EhlersSignalToNoiseRatio(ISeries<double> input, Brush colorD, Brush colorU)
		{
			if (cacheEhlersSignalToNoiseRatio != null)
				for (int idx = 0; idx < cacheEhlersSignalToNoiseRatio.Length; idx++)
					if (cacheEhlersSignalToNoiseRatio[idx] != null && cacheEhlersSignalToNoiseRatio[idx].ColorD == colorD && cacheEhlersSignalToNoiseRatio[idx].ColorU == colorU && cacheEhlersSignalToNoiseRatio[idx].EqualsInput(input))
						return cacheEhlersSignalToNoiseRatio[idx];
			return CacheIndicator<AUN_Indi.Ehlers.EhlersSignalToNoiseRatio>(new AUN_Indi.Ehlers.EhlersSignalToNoiseRatio(){ ColorD = colorD, ColorU = colorU }, input, ref cacheEhlersSignalToNoiseRatio);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersSignalToNoiseRatio EhlersSignalToNoiseRatio(Brush colorD, Brush colorU)
		{
			return indicator.EhlersSignalToNoiseRatio(Input, colorD, colorU);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersSignalToNoiseRatio EhlersSignalToNoiseRatio(ISeries<double> input , Brush colorD, Brush colorU)
		{
			return indicator.EhlersSignalToNoiseRatio(input, colorD, colorU);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersSignalToNoiseRatio EhlersSignalToNoiseRatio(Brush colorD, Brush colorU)
		{
			return indicator.EhlersSignalToNoiseRatio(Input, colorD, colorU);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersSignalToNoiseRatio EhlersSignalToNoiseRatio(ISeries<double> input , Brush colorD, Brush colorU)
		{
			return indicator.EhlersSignalToNoiseRatio(input, colorD, colorU);
		}
	}
}

#endregion
