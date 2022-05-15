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
	public class EhlersLaguerreFilter : Indicator
	{
		private Series<double> l0;
		private Series<double> l1;
		private Series<double> l2;
		private Series<double> l3;
		private int MINBAR = 5;


		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"The Laguerre Filter: John Ehlers, Cybernetic Analysis For Stocks And Futures, pg.216";
				Name										= "EhlersLaguerreFilter";
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
				Gamma					= 0.8;
				ColorD					= Brushes.Red;
				ColorU					= Brushes.LimeGreen;					
				AddPlot(Brushes.DarkBlue, "Filt");
				AddPlot(Brushes.DodgerBlue, "Fir");
			}
			else if (State == State.Configure)
			{
			}
			else if (State == State.DataLoaded)
			{				
				l0 = new Series<double>(this);
				l1 = new Series<double>(this);
				l2 = new Series<double>(this);
				l3 = new Series<double>(this);
			}
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar <= MINBAR) return;
			
   			l0[0] = (1 - Gamma) * Median[0] + Gamma * l0[1];
   			l1[0] = -Gamma * l0[0] + l0[1] + Gamma * l1[1];
   			l2[0] = -Gamma * l1[0] + l1[1] + Gamma * l2[1];
   			l3[0] = -Gamma * l2[0] + l2[1] + Gamma * l3[1];		
			
   			Filt[0] = (l0[0] + 2 * l1[0] + 2 * l2[0] + l3[0]) / 6;
   			Fir[0]  = (Median[0] + 2 * Median[1] + 2 * Median[2] + Median[3]) / 6;   			
			
   			if (Filt[0] < Fir[0]) PlotBrushes[1][0] = ColorU; 
   			else
      			if (Filt[0] > Fir[0]) PlotBrushes[1][0] = ColorD; 		
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(0.1, double.MaxValue)]
		[Display(Name="Gamma", Order=1, GroupName="Parameters")]
		public double Gamma
		{ get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> Filt
		{
			get { return Values[0]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> Fir
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
		private AUN_Indi.Ehlers.EhlersLaguerreFilter[] cacheEhlersLaguerreFilter;
		public AUN_Indi.Ehlers.EhlersLaguerreFilter EhlersLaguerreFilter(double gamma, Brush colorD, Brush colorU)
		{
			return EhlersLaguerreFilter(Input, gamma, colorD, colorU);
		}

		public AUN_Indi.Ehlers.EhlersLaguerreFilter EhlersLaguerreFilter(ISeries<double> input, double gamma, Brush colorD, Brush colorU)
		{
			if (cacheEhlersLaguerreFilter != null)
				for (int idx = 0; idx < cacheEhlersLaguerreFilter.Length; idx++)
					if (cacheEhlersLaguerreFilter[idx] != null && cacheEhlersLaguerreFilter[idx].Gamma == gamma && cacheEhlersLaguerreFilter[idx].ColorD == colorD && cacheEhlersLaguerreFilter[idx].ColorU == colorU && cacheEhlersLaguerreFilter[idx].EqualsInput(input))
						return cacheEhlersLaguerreFilter[idx];
			return CacheIndicator<AUN_Indi.Ehlers.EhlersLaguerreFilter>(new AUN_Indi.Ehlers.EhlersLaguerreFilter(){ Gamma = gamma, ColorD = colorD, ColorU = colorU }, input, ref cacheEhlersLaguerreFilter);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersLaguerreFilter EhlersLaguerreFilter(double gamma, Brush colorD, Brush colorU)
		{
			return indicator.EhlersLaguerreFilter(Input, gamma, colorD, colorU);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersLaguerreFilter EhlersLaguerreFilter(ISeries<double> input , double gamma, Brush colorD, Brush colorU)
		{
			return indicator.EhlersLaguerreFilter(input, gamma, colorD, colorU);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersLaguerreFilter EhlersLaguerreFilter(double gamma, Brush colorD, Brush colorU)
		{
			return indicator.EhlersLaguerreFilter(Input, gamma, colorD, colorU);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersLaguerreFilter EhlersLaguerreFilter(ISeries<double> input , double gamma, Brush colorD, Brush colorU)
		{
			return indicator.EhlersLaguerreFilter(input, gamma, colorD, colorU);
		}
	}
}

#endregion
