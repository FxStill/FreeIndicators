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
	public class EhlersDecyclerOscillator : Indicator
	{
		private double a1, a1k, a1l, a2, a2k, a2l;
		private int MINBAR = 5;
		private Series<double> hp1, hp2;
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"The Decycler Oscillator: John Ehlers, Cycle Analytics For Traders, pg.43-44";
				Name										= "EhlersDecyclerOscillator";
				Calculate									= Calculate.OnBarClose;
				IsOverlay									= false;
				DisplayInDataBox							= true;
				DrawOnPricePanel							= false;
				DrawHorizontalGridLines						= true;
				DrawVerticalGridLines						= true;
				PaintPriceMarkers							= true;
				ScaleJustification							= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				//Disable this property if your indicator requires custom values that cumulate with each new market data event. 
				//See Help Guide for additional information.
				IsSuspendedWhileInactive					= true;
				L1					= 10;
				L2					= 20;
				AddPlot(Brushes.Red, "Oscillator");
			}
			else if (State == State.Configure)
			{
					   hp1 = new Series<double>(this);
					   hp2 = new Series<double>(this);
   				double k   = (1 / Math.Sqrt(2)) * 2 * Math.PI;
   				double t1  = (Math.Cos(k / L1) + Math.Sin (k / L1) - 1) / Math.Cos(k / L1); 
   					   a1  = 1 - t1; a1k = 1 - t1 / 2; a1k *= a1k; a1l = Math.Pow(a1, 2); a1 *= 2;
   				double t2  = (Math.Cos(k / L2) + Math.Sin (k / L2) - 1) / Math.Cos(k / L2);
   					   a2  = 1 - t2; a2k = 1 - t2 / 2; a2k *= a2k; a2l = Math.Pow(a2, 2); a2 *= 2;				
			}
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar <= MINBAR) return;
   			hp1[0] = a1k * (Close[0] - 2 * Close[1] + Close[2]) + a1 * hp1[1] - a1l * hp1[2];
   			hp2[0] = a2k * (Close[0] - 2 * Close[1] + Close[2]) + a2 * hp2[1] - a2l * hp2[2];
   			Oscillator[0] = hp2[0] - hp1[0];  
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="L1", Description="Period 1", Order=1, GroupName="Parameters")]
		public int L1
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="L2", Description="Period 2", Order=2, GroupName="Parameters")]
		public int L2
		{ get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> Oscillator
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
		private AUN_Indi.Ehlers.EhlersDecyclerOscillator[] cacheEhlersDecyclerOscillator;
		public AUN_Indi.Ehlers.EhlersDecyclerOscillator EhlersDecyclerOscillator(int l1, int l2)
		{
			return EhlersDecyclerOscillator(Input, l1, l2);
		}

		public AUN_Indi.Ehlers.EhlersDecyclerOscillator EhlersDecyclerOscillator(ISeries<double> input, int l1, int l2)
		{
			if (cacheEhlersDecyclerOscillator != null)
				for (int idx = 0; idx < cacheEhlersDecyclerOscillator.Length; idx++)
					if (cacheEhlersDecyclerOscillator[idx] != null && cacheEhlersDecyclerOscillator[idx].L1 == l1 && cacheEhlersDecyclerOscillator[idx].L2 == l2 && cacheEhlersDecyclerOscillator[idx].EqualsInput(input))
						return cacheEhlersDecyclerOscillator[idx];
			return CacheIndicator<AUN_Indi.Ehlers.EhlersDecyclerOscillator>(new AUN_Indi.Ehlers.EhlersDecyclerOscillator(){ L1 = l1, L2 = l2 }, input, ref cacheEhlersDecyclerOscillator);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersDecyclerOscillator EhlersDecyclerOscillator(int l1, int l2)
		{
			return indicator.EhlersDecyclerOscillator(Input, l1, l2);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersDecyclerOscillator EhlersDecyclerOscillator(ISeries<double> input , int l1, int l2)
		{
			return indicator.EhlersDecyclerOscillator(input, l1, l2);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersDecyclerOscillator EhlersDecyclerOscillator(int l1, int l2)
		{
			return indicator.EhlersDecyclerOscillator(Input, l1, l2);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersDecyclerOscillator EhlersDecyclerOscillator(ISeries<double> input , int l1, int l2)
		{
			return indicator.EhlersDecyclerOscillator(input, l1, l2);
		}
	}
}

#endregion
