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
	public class EhlersPhaseCalculation : Indicator
	{
		private int MINBAR;
		private double M_PI_2;
		private int l;
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"The Phase Calculation: John Ehlers, Stocks and Commodities Magazine 11/1996";
				Name										= "EhlersPhaseCalculation";
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
				Length					= 15;
				ColorD					= Brushes.Red;
				ColorU					= Brushes.LimeGreen;				
				AddPlot(Brushes.DarkBlue, "Phase");
				AddPlot(Brushes.DodgerBlue, "Signal");
			}
			else if (State == State.Configure)
			{
				MINBAR = Length + 1;
				M_PI_2 = Math.PI / 2;
				l = (int) Length / 2;
			}
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar <= MINBAR) return;
			
   			double weight = 0.0, radians = 0.0, realP = 0.0, imagP = 0.0;
   
   			for (int i = 0; i < Length; i++) {
      			weight = Close[i];
      			radians = 2 * Math.PI * i / Length;
      			realP += Math.Cos(radians) * weight;
      			imagP += Math.Sin(radians) * weight;
   			} 			
			
   			Phase[0] = (Math.Abs(realP) > 0.001)? 
						Math.Atan(imagP / realP) : 
						Math.PI * Math.Sign(imagP) / 2;
		   if (realP < 0) 
       			Phase[0] = Phase[0] + M_PI_2;
   		   if (Phase[0] < 0) 
       			Phase[0] = Phase[0] + M_PI_2;
   		   if (Phase[0] > Math.PI * 2) 
       			Phase[0] = Phase[0] - M_PI_2;
   		   Phase[0] = 180 * Phase[0] / Math.PI;
   		   Signal[0] = SMA(Phase, l)[0];	
		   
   		   if (Phase[0] < Signal[0]) PlotBrushes[0][0] = ColorU; 
   		   else
      			if (Phase[0] > Signal[0]) PlotBrushes[0][0] = ColorD; 		   
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Length", Order=1, GroupName="Parameters")]
		public int Length
		{ get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> Phase
		{
			get { return Values[0]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> Signal
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
		private AUN_Indi.Ehlers.EhlersPhaseCalculation[] cacheEhlersPhaseCalculation;
		public AUN_Indi.Ehlers.EhlersPhaseCalculation EhlersPhaseCalculation(int length, Brush colorD, Brush colorU)
		{
			return EhlersPhaseCalculation(Input, length, colorD, colorU);
		}

		public AUN_Indi.Ehlers.EhlersPhaseCalculation EhlersPhaseCalculation(ISeries<double> input, int length, Brush colorD, Brush colorU)
		{
			if (cacheEhlersPhaseCalculation != null)
				for (int idx = 0; idx < cacheEhlersPhaseCalculation.Length; idx++)
					if (cacheEhlersPhaseCalculation[idx] != null && cacheEhlersPhaseCalculation[idx].Length == length && cacheEhlersPhaseCalculation[idx].ColorD == colorD && cacheEhlersPhaseCalculation[idx].ColorU == colorU && cacheEhlersPhaseCalculation[idx].EqualsInput(input))
						return cacheEhlersPhaseCalculation[idx];
			return CacheIndicator<AUN_Indi.Ehlers.EhlersPhaseCalculation>(new AUN_Indi.Ehlers.EhlersPhaseCalculation(){ Length = length, ColorD = colorD, ColorU = colorU }, input, ref cacheEhlersPhaseCalculation);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersPhaseCalculation EhlersPhaseCalculation(int length, Brush colorD, Brush colorU)
		{
			return indicator.EhlersPhaseCalculation(Input, length, colorD, colorU);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersPhaseCalculation EhlersPhaseCalculation(ISeries<double> input , int length, Brush colorD, Brush colorU)
		{
			return indicator.EhlersPhaseCalculation(input, length, colorD, colorU);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersPhaseCalculation EhlersPhaseCalculation(int length, Brush colorD, Brush colorU)
		{
			return indicator.EhlersPhaseCalculation(Input, length, colorD, colorU);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersPhaseCalculation EhlersPhaseCalculation(ISeries<double> input , int length, Brush colorD, Brush colorU)
		{
			return indicator.EhlersPhaseCalculation(input, length, colorD, colorU);
		}
	}
}

#endregion
