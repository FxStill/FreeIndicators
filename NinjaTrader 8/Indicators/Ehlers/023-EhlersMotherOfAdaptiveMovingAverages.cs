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
	public class EhlersMotherOfAdaptiveMovingAverages : Indicator
	{
		private Series<double> phase;
		private int MINBAR = 5;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"The Mother Of Adaptive Moving Averages: John Ehlers,  Rocket Science For Traders, pg.182-183";
				Name										= "EhlersMotherOfAdaptiveMovingAverages";
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
				FastLimit					= 0.5;
				SlowLimit					= 0.05;
				ColorD					= Brushes.Red;
				ColorU					= Brushes.LimeGreen;
				
				AddPlot(Brushes.DarkBlue, "Ma");
				AddPlot(Brushes.DodgerBlue, "Fama");
			}
			else if (State == State.Configure)
			{
			}
			else if (State == State.DataLoaded)
			{				
				phase = new Series<double>(this);
			}
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar <= MINBAR) return;
			
			double i1 = EhlersHilbertTransform(Input).I1[0];
			double q1 = EhlersHilbertTransform(Input).Q1[0];
			
   			phase[0] = (i1 != 0)? Math.Atan(q1 / i1) : 0;
   			double deltaPhase = phase[1] - phase[0];
   			deltaPhase = (deltaPhase < 1)? 1 : deltaPhase;

   			double alpha = FastLimit / deltaPhase;
   			alpha = (alpha < SlowLimit)? SlowLimit : alpha;			
			
   			Ma[0] = alpha * Median[0] + (1 - alpha) * Ma[1];
   			Fama[0] = 0.5 * alpha * Ma[0] + (1 - 0.5 * alpha) * Fama[1];  	

   			if (Ma[0] < Fama[0]) PlotBrushes[0][0] = ColorD;
   			else
      			if (Ma[0] > Fama[0]) PlotBrushes[0][0] = ColorU;  			
			
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(0.1, double.MaxValue)]
		[Display(Name="FastLimit", Order=1, GroupName="Parameters")]
		public double FastLimit
		{ get; set; }

		[NinjaScriptProperty]
		[Range(0.01, double.MaxValue)]
		[Display(Name="SlowLimit", Order=2, GroupName="Parameters")]
		public double SlowLimit
		{ get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> Ma
		{
			get { return Values[0]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> Fama
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
		private AUN_Indi.Ehlers.EhlersMotherOfAdaptiveMovingAverages[] cacheEhlersMotherOfAdaptiveMovingAverages;
		public AUN_Indi.Ehlers.EhlersMotherOfAdaptiveMovingAverages EhlersMotherOfAdaptiveMovingAverages(double fastLimit, double slowLimit, Brush colorD, Brush colorU)
		{
			return EhlersMotherOfAdaptiveMovingAverages(Input, fastLimit, slowLimit, colorD, colorU);
		}

		public AUN_Indi.Ehlers.EhlersMotherOfAdaptiveMovingAverages EhlersMotherOfAdaptiveMovingAverages(ISeries<double> input, double fastLimit, double slowLimit, Brush colorD, Brush colorU)
		{
			if (cacheEhlersMotherOfAdaptiveMovingAverages != null)
				for (int idx = 0; idx < cacheEhlersMotherOfAdaptiveMovingAverages.Length; idx++)
					if (cacheEhlersMotherOfAdaptiveMovingAverages[idx] != null && cacheEhlersMotherOfAdaptiveMovingAverages[idx].FastLimit == fastLimit && cacheEhlersMotherOfAdaptiveMovingAverages[idx].SlowLimit == slowLimit && cacheEhlersMotherOfAdaptiveMovingAverages[idx].ColorD == colorD && cacheEhlersMotherOfAdaptiveMovingAverages[idx].ColorU == colorU && cacheEhlersMotherOfAdaptiveMovingAverages[idx].EqualsInput(input))
						return cacheEhlersMotherOfAdaptiveMovingAverages[idx];
			return CacheIndicator<AUN_Indi.Ehlers.EhlersMotherOfAdaptiveMovingAverages>(new AUN_Indi.Ehlers.EhlersMotherOfAdaptiveMovingAverages(){ FastLimit = fastLimit, SlowLimit = slowLimit, ColorD = colorD, ColorU = colorU }, input, ref cacheEhlersMotherOfAdaptiveMovingAverages);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersMotherOfAdaptiveMovingAverages EhlersMotherOfAdaptiveMovingAverages(double fastLimit, double slowLimit, Brush colorD, Brush colorU)
		{
			return indicator.EhlersMotherOfAdaptiveMovingAverages(Input, fastLimit, slowLimit, colorD, colorU);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersMotherOfAdaptiveMovingAverages EhlersMotherOfAdaptiveMovingAverages(ISeries<double> input , double fastLimit, double slowLimit, Brush colorD, Brush colorU)
		{
			return indicator.EhlersMotherOfAdaptiveMovingAverages(input, fastLimit, slowLimit, colorD, colorU);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersMotherOfAdaptiveMovingAverages EhlersMotherOfAdaptiveMovingAverages(double fastLimit, double slowLimit, Brush colorD, Brush colorU)
		{
			return indicator.EhlersMotherOfAdaptiveMovingAverages(Input, fastLimit, slowLimit, colorD, colorU);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersMotherOfAdaptiveMovingAverages EhlersMotherOfAdaptiveMovingAverages(ISeries<double> input , double fastLimit, double slowLimit, Brush colorD, Brush colorU)
		{
			return indicator.EhlersMotherOfAdaptiveMovingAverages(input, fastLimit, slowLimit, colorD, colorU);
		}
	}
}

#endregion
