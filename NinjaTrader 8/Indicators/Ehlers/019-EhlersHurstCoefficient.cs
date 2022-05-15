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
	public class EhlersHurstCoefficient : Indicator
	{
		private Series<double> hurst;
		private Series<double> dimen;
		private int MINBAR;
		private double a, b, c1, c2, c3;
		private int hl;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"The Hurst Coefficient: John Ehlers, Cycle Analytics For Traders, pg.67-68";
				Name										= "EhlersHurstCoefficient";
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
				Length					= 30;
				Ssflength					= 20;
				ColorD					= Brushes.Red;
				ColorU					= Brushes.LimeGreen;				
				AddPlot(Brushes.DarkBlue, "SmoothHurst");
				AddLine(Brushes.Goldenrod, 0.5, "ZerroLevel");
			}
			else if (State == State.Configure)
			{
				MINBAR = Length;
   				a = Math.Exp(-Math.Sqrt(2) * Math.PI  / Ssflength);
				b = 2 * a * Math.Cos(Math.Sqrt(2) * Math.PI / Ssflength); 
   				c2 = b;
   				c3 = -Math.Pow(a, 2);
   				c1 = 1 - c2 - c3;
   				hl = (int)Math.Ceiling((double)Length / 2);				

			}
			else if (State == State.DataLoaded)
			{				
				hurst = new Series<double>(this);
				dimen = new Series<double>(this);
			}
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar <= MINBAR) return;
			
			double h = Close[HighestBar(Close, Length)];			
			double l = Close[LowestBar(Close, Length)];
			double n3 = (h - l) / Length;
			
		    double hh = Close[0];
   		    double ll = Close[0];

		    for (int i = 0; i < hl; i++) {
      			if (Close[i] > hh) hh = Close[i];
      			if (Close[i] < ll) ll = Close[i];
   		    }	
   			double n1 = (hh - ll) / hl;
   
   			hh = Close[hl];
   			ll = Close[hl];
   
   			for (int i = hl; i < Length; i++) {
      			if (Close[i] > hh) hh = Close[i];
      			if (Close[i] < ll) ll = Close[i];
   			}
   			double n2 = (hh - ll) / hl;			
			
   			dimen[0] = (n1 > 0 && n2 > 0 && n3 > 0) ? 
            	       0.5 * ( (Math.Log(n1 + n2) - Math.Log(n3)) / Math.Log(2) + dimen[1]) : 0;
                   
   			hurst[0] = 2 - dimen[0];  
 
   
   			SmoothHurst[0] = c1 * (hurst[0] + hurst[1]) / 2 + c2 * SmoothHurst[1] + c3 * SmoothHurst[2];			
			
   			if (SmoothHurst[0] < 0.5) PlotBrushes[0][0] = ColorD;
   			else
      			if (SmoothHurst[0] > 0.5) PlotBrushes[0][0] = ColorU;
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Length", Order=1, GroupName="Parameters")]
		public int Length
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Ssflength", Description="SuperSmoother filter Length", Order=2, GroupName="Parameters")]
		public int Ssflength
		{ get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> SmoothHurst
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
		private AUN_Indi.Ehlers.EhlersHurstCoefficient[] cacheEhlersHurstCoefficient;
		public AUN_Indi.Ehlers.EhlersHurstCoefficient EhlersHurstCoefficient(int length, int ssflength, Brush colorD, Brush colorU)
		{
			return EhlersHurstCoefficient(Input, length, ssflength, colorD, colorU);
		}

		public AUN_Indi.Ehlers.EhlersHurstCoefficient EhlersHurstCoefficient(ISeries<double> input, int length, int ssflength, Brush colorD, Brush colorU)
		{
			if (cacheEhlersHurstCoefficient != null)
				for (int idx = 0; idx < cacheEhlersHurstCoefficient.Length; idx++)
					if (cacheEhlersHurstCoefficient[idx] != null && cacheEhlersHurstCoefficient[idx].Length == length && cacheEhlersHurstCoefficient[idx].Ssflength == ssflength && cacheEhlersHurstCoefficient[idx].ColorD == colorD && cacheEhlersHurstCoefficient[idx].ColorU == colorU && cacheEhlersHurstCoefficient[idx].EqualsInput(input))
						return cacheEhlersHurstCoefficient[idx];
			return CacheIndicator<AUN_Indi.Ehlers.EhlersHurstCoefficient>(new AUN_Indi.Ehlers.EhlersHurstCoefficient(){ Length = length, Ssflength = ssflength, ColorD = colorD, ColorU = colorU }, input, ref cacheEhlersHurstCoefficient);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersHurstCoefficient EhlersHurstCoefficient(int length, int ssflength, Brush colorD, Brush colorU)
		{
			return indicator.EhlersHurstCoefficient(Input, length, ssflength, colorD, colorU);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersHurstCoefficient EhlersHurstCoefficient(ISeries<double> input , int length, int ssflength, Brush colorD, Brush colorU)
		{
			return indicator.EhlersHurstCoefficient(input, length, ssflength, colorD, colorU);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersHurstCoefficient EhlersHurstCoefficient(int length, int ssflength, Brush colorD, Brush colorU)
		{
			return indicator.EhlersHurstCoefficient(Input, length, ssflength, colorD, colorU);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersHurstCoefficient EhlersHurstCoefficient(ISeries<double> input , int length, int ssflength, Brush colorD, Brush colorU)
		{
			return indicator.EhlersHurstCoefficient(input, length, ssflength, colorD, colorU);
		}
	}
}

#endregion
