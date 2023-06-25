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
	public class EhlersRestoringPullIndicator : Indicator
	{
		private double p2;
		private int MINBAR;
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"The Restoring Pull Indicator: John Ehlers, Stocks & Commodities V.11:10 (395-400)";
				Name										= "EhlersRestoringPullIndicator";
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
				MinPeriod					= 8;
				MaxPeriod					= 50;
				HpPeriod					= 40;
				MedianPeriod					= 10;
				DecibelPeriod					= 20;
				
				AddPlot(Brushes.Red, "V1");
				AddPlot(Brushes.DodgerBlue, "V2");
			}
			else if (State == State.Configure)
			{
				p2 = 2 * Math.PI;
				MINBAR = MinPeriod + 1;
				Print(V1.Count);
			}
		}

		protected override void OnBarUpdate()
		{
		    if (CurrentBar < MINBAR) {return;}
			double dc    = EhlersBandPassFilter(Input, MinPeriod, MaxPeriod, HpPeriod, MedianPeriod, DecibelPeriod)[0];
			if (dc == 0.0) return;
			double v     = VOL(Input)[0];		
			       V1[0] = Math.Pow(p2 / dc, 2) * v;
				   V2[0] = SMA(V1, MinPeriod)[0];	
		}

		#region Properties

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="MinPeriod", Description="MinPeriod", Order=1, GroupName="Parameters")]
		public int MinPeriod
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="MaxPeriod", Description="MaxPeriod", Order=2, GroupName="Parameters")]
		public int MaxPeriod
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="HpPeriod", Description="HpPeriod", Order=3, GroupName="Parameters")]
		public int HpPeriod
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="MedianPeriod", Description="MedianPeriod", Order=4, GroupName="Parameters")]
		public int MedianPeriod
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="DecibelPeriod", Description="DecibelPeriod", Order=5, GroupName="Parameters")]
		public int DecibelPeriod
		{ get; set; }		
		
		[Browsable(false)]
		[XmlIgnore]
		public Series<double> V1
		{
			get { return Values[0]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> V2
		{
			get { return Values[1]; }
		}
		#endregion

	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private AUN_Indi.Ehlers.EhlersRestoringPullIndicator[] cacheEhlersRestoringPullIndicator;
		public AUN_Indi.Ehlers.EhlersRestoringPullIndicator EhlersRestoringPullIndicator(int minPeriod, int maxPeriod, int hpPeriod, int medianPeriod, int decibelPeriod)
		{
			return EhlersRestoringPullIndicator(Input, minPeriod, maxPeriod, hpPeriod, medianPeriod, decibelPeriod);
		}

		public AUN_Indi.Ehlers.EhlersRestoringPullIndicator EhlersRestoringPullIndicator(ISeries<double> input, int minPeriod, int maxPeriod, int hpPeriod, int medianPeriod, int decibelPeriod)
		{
			if (cacheEhlersRestoringPullIndicator != null)
				for (int idx = 0; idx < cacheEhlersRestoringPullIndicator.Length; idx++)
					if (cacheEhlersRestoringPullIndicator[idx] != null && cacheEhlersRestoringPullIndicator[idx].MinPeriod == minPeriod && cacheEhlersRestoringPullIndicator[idx].MaxPeriod == maxPeriod && cacheEhlersRestoringPullIndicator[idx].HpPeriod == hpPeriod && cacheEhlersRestoringPullIndicator[idx].MedianPeriod == medianPeriod && cacheEhlersRestoringPullIndicator[idx].DecibelPeriod == decibelPeriod && cacheEhlersRestoringPullIndicator[idx].EqualsInput(input))
						return cacheEhlersRestoringPullIndicator[idx];
			return CacheIndicator<AUN_Indi.Ehlers.EhlersRestoringPullIndicator>(new AUN_Indi.Ehlers.EhlersRestoringPullIndicator(){ MinPeriod = minPeriod, MaxPeriod = maxPeriod, HpPeriod = hpPeriod, MedianPeriod = medianPeriod, DecibelPeriod = decibelPeriod }, input, ref cacheEhlersRestoringPullIndicator);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersRestoringPullIndicator EhlersRestoringPullIndicator(int minPeriod, int maxPeriod, int hpPeriod, int medianPeriod, int decibelPeriod)
		{
			return indicator.EhlersRestoringPullIndicator(Input, minPeriod, maxPeriod, hpPeriod, medianPeriod, decibelPeriod);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersRestoringPullIndicator EhlersRestoringPullIndicator(ISeries<double> input , int minPeriod, int maxPeriod, int hpPeriod, int medianPeriod, int decibelPeriod)
		{
			return indicator.EhlersRestoringPullIndicator(input, minPeriod, maxPeriod, hpPeriod, medianPeriod, decibelPeriod);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersRestoringPullIndicator EhlersRestoringPullIndicator(int minPeriod, int maxPeriod, int hpPeriod, int medianPeriod, int decibelPeriod)
		{
			return indicator.EhlersRestoringPullIndicator(Input, minPeriod, maxPeriod, hpPeriod, medianPeriod, decibelPeriod);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersRestoringPullIndicator EhlersRestoringPullIndicator(ISeries<double> input , int minPeriod, int maxPeriod, int hpPeriod, int medianPeriod, int decibelPeriod)
		{
			return indicator.EhlersRestoringPullIndicator(input, minPeriod, maxPeriod, hpPeriod, medianPeriod, decibelPeriod);
		}
	}
}

#endregion
