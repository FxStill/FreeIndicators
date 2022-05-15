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
	public class EhlersHilbertOscillator : Indicator
	{
		private Series<double> q3;
		private int MINBAR = 5;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"The Hilbert Oscillator: John Ehlers, Rocket Science For Traders, pg.90-91";
				Name										= "EhlersHilbertOscillator";
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
				AddPlot(Brushes.DodgerBlue, "V3");
				AddPlot(Brushes.Red, "I3");
			}
			else if (State == State.Configure)
			{
			}
			else if (State == State.DataLoaded)
			{				
				q3 = new Series<double>(this);
			}
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar <= MINBAR) return;
			
			double pr = EhlersHilbertTransform(Input).SmoothPeriod[0];
		    if (pr == 0.0) return;
		    int sp2 = (int)Math.Ceiling(pr / 2);
		    if (sp2 < 3) sp2 = 3;			
		    if (CurrentBar <= sp2 + 2) return;
			
   			q3[0] = 0.5 * (EhlersHilbertTransform(Input).Smooth[0] - 
			               EhlersHilbertTransform(Input).Smooth[2]) * (0.1759 * pr + 0.4607);
			
		    for (int i = 0; i < sp2; i++) 
      			I3[0] += q3[i];
   			I3[0] = (1.57 * I3[0]) / sp2;   
   
		    double sp4 = Math.Ceiling(pr / 4);
   			for (int i = 0; i < sp4; i++)
      			V1[0] += q3[i];
   			V1[0] = 1.25 * V1[0] / sp4;   
		}

		#region Properties

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> V1
		{
			get { return Values[0]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> I3
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
		private AUN_Indi.Ehlers.EhlersHilbertOscillator[] cacheEhlersHilbertOscillator;
		public AUN_Indi.Ehlers.EhlersHilbertOscillator EhlersHilbertOscillator()
		{
			return EhlersHilbertOscillator(Input);
		}

		public AUN_Indi.Ehlers.EhlersHilbertOscillator EhlersHilbertOscillator(ISeries<double> input)
		{
			if (cacheEhlersHilbertOscillator != null)
				for (int idx = 0; idx < cacheEhlersHilbertOscillator.Length; idx++)
					if (cacheEhlersHilbertOscillator[idx] != null &&  cacheEhlersHilbertOscillator[idx].EqualsInput(input))
						return cacheEhlersHilbertOscillator[idx];
			return CacheIndicator<AUN_Indi.Ehlers.EhlersHilbertOscillator>(new AUN_Indi.Ehlers.EhlersHilbertOscillator(), input, ref cacheEhlersHilbertOscillator);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersHilbertOscillator EhlersHilbertOscillator()
		{
			return indicator.EhlersHilbertOscillator(Input);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersHilbertOscillator EhlersHilbertOscillator(ISeries<double> input )
		{
			return indicator.EhlersHilbertOscillator(input);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersHilbertOscillator EhlersHilbertOscillator()
		{
			return indicator.EhlersHilbertOscillator(Input);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersHilbertOscillator EhlersHilbertOscillator(ISeries<double> input )
		{
			return indicator.EhlersHilbertOscillator(input);
		}
	}
}

#endregion
