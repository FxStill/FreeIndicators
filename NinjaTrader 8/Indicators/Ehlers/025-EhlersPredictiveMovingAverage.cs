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
	public class EhlersPredictiveMovingAverage : Indicator
	{
		private Series<double> wma1;
		private Series<double> wma2;
		private int MINBAR = 13;


		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"The Predictive Moving Average: John Ehlers, Rocket Science For Traders, pg.212";
				Name										= "EhlersPredictiveMovingAverage";
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
				ColorD					= Brushes.Red;
				ColorU					= Brushes.LimeGreen;
				
				AddPlot(Brushes.DodgerBlue, "Predict");
				AddPlot(Brushes.Cyan, "Trigger");
			}
			else if (State == State.Configure)
			{
			}
			else if (State == State.DataLoaded)
			{				
				wma1 = new Series<double>(this);
				wma2 = new Series<double>(this);
			}
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar <= MINBAR) return;
			
   			wma1[0] = (7 * Median[0] + 6 * Median[1] + 5 * Median[2] + 4 * Median[3] + 3 * Median[4] + 2 * Median[5] + Median[6]) / 28;
   			wma2[0] = (7 * wma1[0] +  6 * wma1[1] +  5 * wma1[2] +  4 * wma1[3] +  3 * wma1[4] +  2 * wma1[5] +  wma1[6])  / 28;
   
   			Trigger[0] = 2 * wma1[0] - wma2[0];
   			Predict[0] = (4 * Trigger[0] + 3 * Trigger[1] + 2 * Trigger[2] + Trigger[3]) / 10;   
			
   			if (Predict[0] < Trigger[0]) PlotBrushes[0][0] = ColorU; 
   			else
      			if (Predict[0] > Trigger[0]) PlotBrushes[0][0] = ColorD;   			
		}

		#region Properties

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> Predict
		{
			get { return Values[0]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> Trigger
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
		private AUN_Indi.Ehlers.EhlersPredictiveMovingAverage[] cacheEhlersPredictiveMovingAverage;
		public AUN_Indi.Ehlers.EhlersPredictiveMovingAverage EhlersPredictiveMovingAverage(Brush colorD, Brush colorU)
		{
			return EhlersPredictiveMovingAverage(Input, colorD, colorU);
		}

		public AUN_Indi.Ehlers.EhlersPredictiveMovingAverage EhlersPredictiveMovingAverage(ISeries<double> input, Brush colorD, Brush colorU)
		{
			if (cacheEhlersPredictiveMovingAverage != null)
				for (int idx = 0; idx < cacheEhlersPredictiveMovingAverage.Length; idx++)
					if (cacheEhlersPredictiveMovingAverage[idx] != null && cacheEhlersPredictiveMovingAverage[idx].ColorD == colorD && cacheEhlersPredictiveMovingAverage[idx].ColorU == colorU && cacheEhlersPredictiveMovingAverage[idx].EqualsInput(input))
						return cacheEhlersPredictiveMovingAverage[idx];
			return CacheIndicator<AUN_Indi.Ehlers.EhlersPredictiveMovingAverage>(new AUN_Indi.Ehlers.EhlersPredictiveMovingAverage(){ ColorD = colorD, ColorU = colorU }, input, ref cacheEhlersPredictiveMovingAverage);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersPredictiveMovingAverage EhlersPredictiveMovingAverage(Brush colorD, Brush colorU)
		{
			return indicator.EhlersPredictiveMovingAverage(Input, colorD, colorU);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersPredictiveMovingAverage EhlersPredictiveMovingAverage(ISeries<double> input , Brush colorD, Brush colorU)
		{
			return indicator.EhlersPredictiveMovingAverage(input, colorD, colorU);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersPredictiveMovingAverage EhlersPredictiveMovingAverage(Brush colorD, Brush colorU)
		{
			return indicator.EhlersPredictiveMovingAverage(Input, colorD, colorU);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersPredictiveMovingAverage EhlersPredictiveMovingAverage(ISeries<double> input , Brush colorD, Brush colorU)
		{
			return indicator.EhlersPredictiveMovingAverage(input, colorD, colorU);
		}
	}
}

#endregion
