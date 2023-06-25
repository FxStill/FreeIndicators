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
	public class EhlersRoofingFilter : Indicator
	{
		private Series<double> hp;
		private int MINBAR = 5;
		private double c1, c2, c3, a1, t1, t2, t3;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Ehlers Roofing Filter:John Ehlers, Cycle Analytics for Traders, pg.80-82";
				Name										= "EhlersRoofingFilter";
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
				HpLength					= 80;
				LpLength					= 40;
				ColorD					= Brushes.Red;
				ColorU					= Brushes.LimeGreen;				
				AddPlot(Brushes.DarkBlue, "Rfilt");
				AddPlot(Brushes.DodgerBlue, "Trigger");
				AddLine(Brushes.Goldenrod, 0, "ZerroLine");
			}
			else if (State == State.Configure)
			{
				double p2 = 1 / Math.PI;
   				double twoPiPrd = p2 * 2 * Math.PI / HpLength;
   				a1 = (Math.Cos(twoPiPrd) + Math.Sin(twoPiPrd) - 1) / Math.Cos(twoPiPrd);
				p2 = Math.Sqrt(2);
   				double a2 = Math.Exp(-p2 * Math.PI / LpLength);
   				double beta = 2 * a2 * Math.Cos(p2 * Math.PI / LpLength);
   				c2 = beta;
   				c3 = -a2 * a2;
   				c1 = 1 - c2 - c3;
   				t1 = Math.Pow(1 - (a1 / 2), 2);
   				t2 = 1 - a1;
   				t3 = Math.Pow(t2, 2);
   				t2 *= 2;				
			}
			else if (State == State.DataLoaded)
			{				
				hp = new Series<double>(this);
			}
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar < MINBAR) return;
			
				
		   hp[0] =  t1 * (Close[0] - 2 * Close[1] + Close[2]) + t2 * hp[1] - t3 * hp[2];
   
		   Rfilt[0] = c1 * ((hp[0] + hp[1]) / 2) + c2 * Rfilt[1] + c3 * Rfilt[2];
		   Trigger[0] = Rfilt[2];
   
		   if (Rfilt[0] < 0) PlotBrushes[0][0] = ColorD; 
		   else
				if (Rfilt[0] > 0) PlotBrushes[0][0] = ColorU;  			
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="HpLength", Order=1, GroupName="Parameters")]
		public int HpLength
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="LpLength", Order=2, GroupName="Parameters")]
		public int LpLength
		{ get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> Rfilt
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
		private AUN_Indi.Ehlers.EhlersRoofingFilter[] cacheEhlersRoofingFilter;
		public AUN_Indi.Ehlers.EhlersRoofingFilter EhlersRoofingFilter(int hpLength, int lpLength, Brush colorD, Brush colorU)
		{
			return EhlersRoofingFilter(Input, hpLength, lpLength, colorD, colorU);
		}

		public AUN_Indi.Ehlers.EhlersRoofingFilter EhlersRoofingFilter(ISeries<double> input, int hpLength, int lpLength, Brush colorD, Brush colorU)
		{
			if (cacheEhlersRoofingFilter != null)
				for (int idx = 0; idx < cacheEhlersRoofingFilter.Length; idx++)
					if (cacheEhlersRoofingFilter[idx] != null && cacheEhlersRoofingFilter[idx].HpLength == hpLength && cacheEhlersRoofingFilter[idx].LpLength == lpLength && cacheEhlersRoofingFilter[idx].ColorD == colorD && cacheEhlersRoofingFilter[idx].ColorU == colorU && cacheEhlersRoofingFilter[idx].EqualsInput(input))
						return cacheEhlersRoofingFilter[idx];
			return CacheIndicator<AUN_Indi.Ehlers.EhlersRoofingFilter>(new AUN_Indi.Ehlers.EhlersRoofingFilter(){ HpLength = hpLength, LpLength = lpLength, ColorD = colorD, ColorU = colorU }, input, ref cacheEhlersRoofingFilter);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersRoofingFilter EhlersRoofingFilter(int hpLength, int lpLength, Brush colorD, Brush colorU)
		{
			return indicator.EhlersRoofingFilter(Input, hpLength, lpLength, colorD, colorU);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersRoofingFilter EhlersRoofingFilter(ISeries<double> input , int hpLength, int lpLength, Brush colorD, Brush colorU)
		{
			return indicator.EhlersRoofingFilter(input, hpLength, lpLength, colorD, colorU);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersRoofingFilter EhlersRoofingFilter(int hpLength, int lpLength, Brush colorD, Brush colorU)
		{
			return indicator.EhlersRoofingFilter(Input, hpLength, lpLength, colorD, colorU);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersRoofingFilter EhlersRoofingFilter(ISeries<double> input , int hpLength, int lpLength, Brush colorD, Brush colorU)
		{
			return indicator.EhlersRoofingFilter(input, hpLength, lpLength, colorD, colorU);
		}
	}
}

#endregion
