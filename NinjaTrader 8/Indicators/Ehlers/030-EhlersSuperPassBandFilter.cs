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
	public class EhlersSuperPassBandFilter : Indicator
	{
		private int MINBAR;
		private double a1, a2;
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"The Super PassBand Filter: John Ehlers, Stocks & Commodities V. 34:07, pg.10-13";
				Name										= "EhlersSuperPassBandFilter";
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
				Eshort					= 40;
				Elong					= 60;
				Rms					= 50;
				ColorD					= Brushes.Red;
				ColorU					= Brushes.LimeGreen;
				
				AddPlot(Brushes.DarkBlue, "Spbf");
				AddPlot(Brushes.Orange, "Rms1");
				AddPlot(Brushes.Orange, "Rms2");
				AddLine(Brushes.Goldenrod, 0, "ZerroLone");
			}
			else if (State == State.Configure)
			{
				MINBAR = Rms;
   				a1 = 5.0 / Eshort;
   				a2 = 5.0 / Elong;				
			}
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar <= MINBAR) return;
			
   			Spbf[0] = (a1 - a2) * Close[0] + 
               ( a2 * (1 - a1) - a1 * (1 - a2) ) * Close[1] + 
               (2 - a1  - a2) * Spbf[1] - 
               (1 - a1) * (1 - a2) * Spbf[2];			
   			double t = 0;
   			for (int i = 0; i < Rms; ++i) {
//      			if (spbf[shift + i] == EMPTY_VALUE) continue;
      			t += Math.Pow(Spbf[i], 2);
   			}
   			Rms1[0] = Math.Sqrt(t / Rms);
   			Rms2[0] = -Rms1[0];  	
			
   			if (Spbf[0] < 0) PlotBrushes[0][0] = ColorD;	
   			else
      			if (Spbf[0] > 0) PlotBrushes[0][0] = ColorU;				
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Eshort", Order=1, GroupName="Parameters")]
		public int Eshort
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Elong", Order=2, GroupName="Parameters")]
		public int Elong
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Rms", Order=3, GroupName="Parameters")]
		public int Rms
		{ get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> Spbf
		{
			get { return Values[0]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> Rms1
		{
			get { return Values[1]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> Rms2
		{
			get { return Values[2]; }
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
		private AUN_Indi.Ehlers.EhlersSuperPassBandFilter[] cacheEhlersSuperPassBandFilter;
		public AUN_Indi.Ehlers.EhlersSuperPassBandFilter EhlersSuperPassBandFilter(int eshort, int elong, int rms, Brush colorD, Brush colorU)
		{
			return EhlersSuperPassBandFilter(Input, eshort, elong, rms, colorD, colorU);
		}

		public AUN_Indi.Ehlers.EhlersSuperPassBandFilter EhlersSuperPassBandFilter(ISeries<double> input, int eshort, int elong, int rms, Brush colorD, Brush colorU)
		{
			if (cacheEhlersSuperPassBandFilter != null)
				for (int idx = 0; idx < cacheEhlersSuperPassBandFilter.Length; idx++)
					if (cacheEhlersSuperPassBandFilter[idx] != null && cacheEhlersSuperPassBandFilter[idx].Eshort == eshort && cacheEhlersSuperPassBandFilter[idx].Elong == elong && cacheEhlersSuperPassBandFilter[idx].Rms == rms && cacheEhlersSuperPassBandFilter[idx].ColorD == colorD && cacheEhlersSuperPassBandFilter[idx].ColorU == colorU && cacheEhlersSuperPassBandFilter[idx].EqualsInput(input))
						return cacheEhlersSuperPassBandFilter[idx];
			return CacheIndicator<AUN_Indi.Ehlers.EhlersSuperPassBandFilter>(new AUN_Indi.Ehlers.EhlersSuperPassBandFilter(){ Eshort = eshort, Elong = elong, Rms = rms, ColorD = colorD, ColorU = colorU }, input, ref cacheEhlersSuperPassBandFilter);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersSuperPassBandFilter EhlersSuperPassBandFilter(int eshort, int elong, int rms, Brush colorD, Brush colorU)
		{
			return indicator.EhlersSuperPassBandFilter(Input, eshort, elong, rms, colorD, colorU);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersSuperPassBandFilter EhlersSuperPassBandFilter(ISeries<double> input , int eshort, int elong, int rms, Brush colorD, Brush colorU)
		{
			return indicator.EhlersSuperPassBandFilter(input, eshort, elong, rms, colorD, colorU);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersSuperPassBandFilter EhlersSuperPassBandFilter(int eshort, int elong, int rms, Brush colorD, Brush colorU)
		{
			return indicator.EhlersSuperPassBandFilter(Input, eshort, elong, rms, colorD, colorU);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersSuperPassBandFilter EhlersSuperPassBandFilter(ISeries<double> input , int eshort, int elong, int rms, Brush colorD, Brush colorU)
		{
			return indicator.EhlersSuperPassBandFilter(input, eshort, elong, rms, colorD, colorU);
		}
	}
}

#endregion
