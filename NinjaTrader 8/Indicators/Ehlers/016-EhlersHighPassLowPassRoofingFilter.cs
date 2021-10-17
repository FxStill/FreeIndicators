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
	public class EhlersHighPassLowPassRoofingFilter : Indicator
	{
		private Series<double> hb;
		private int MINBAR = 5;
		private double a1, a2, c1, c2, c3;
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"The HighPass-LowPass Roofing Filter: John Ehlers, Cycle Analytics For Traders, pg.78";
				Name										= "EhlersHighPassLowPassRoofingFilter";
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
				HPLength					= 48;
				SSFLength					= 10;
				ColorD					= Brushes.Red;
				ColorU					= Brushes.LimeGreen;				
				AddPlot(Brushes.ForestGreen, "Rb");
			}
			else if (State == State.Configure)
			{
			   double twoPiPrd = 2 * Math.PI / HPLength;
			   double alpha1   = (Math.Cos(twoPiPrd) + Math.Sin(twoPiPrd) - 1) / Math.Cos(twoPiPrd);
			   double alpha2   = Math.Exp(-Math.Sqrt(2) * Math.PI / SSFLength);
   			   double beta     = 2 * alpha2 * Math.Cos(Math.Sqrt(2) * Math.PI / SSFLength);
	           c2       = beta;
	           c3       = -Math.Pow(alpha2, 2);
    	       c1       = 1 - c2 - c3;
        	   a1       = 1 - alpha1 / 2;
          	   a2       = 1 - alpha1;				
			}
			else if (State == State.DataLoaded)
			{				
				hb = new Series<double>(this);
			}
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar <= MINBAR) return;
			
		    hb[0] = a1 * (Close[0] - Close[1]) + a2 * hb[1];
   			Rb[0] = c1 * (hb[0] + hb[1]) / 2 + c2 * Rb[1] + c3 * Rb[2];			
			
   			if (Rb[0] < 0) PlotBrushes[0][0] = ColorD;
   			else
      			if (Rb[0] > 0) PlotBrushes[0][0] = ColorU; 				
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="HPLength", Order=1, GroupName="Parameters")]
		public int HPLength
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="SSFLength", Order=2, GroupName="Parameters")]
		public int SSFLength
		{ get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> Rb
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
		private AUN_Indi.Ehlers.EhlersHighPassLowPassRoofingFilter[] cacheEhlersHighPassLowPassRoofingFilter;
		public AUN_Indi.Ehlers.EhlersHighPassLowPassRoofingFilter EhlersHighPassLowPassRoofingFilter(int hPLength, int sSFLength, Brush colorD, Brush colorU)
		{
			return EhlersHighPassLowPassRoofingFilter(Input, hPLength, sSFLength, colorD, colorU);
		}

		public AUN_Indi.Ehlers.EhlersHighPassLowPassRoofingFilter EhlersHighPassLowPassRoofingFilter(ISeries<double> input, int hPLength, int sSFLength, Brush colorD, Brush colorU)
		{
			if (cacheEhlersHighPassLowPassRoofingFilter != null)
				for (int idx = 0; idx < cacheEhlersHighPassLowPassRoofingFilter.Length; idx++)
					if (cacheEhlersHighPassLowPassRoofingFilter[idx] != null && cacheEhlersHighPassLowPassRoofingFilter[idx].HPLength == hPLength && cacheEhlersHighPassLowPassRoofingFilter[idx].SSFLength == sSFLength && cacheEhlersHighPassLowPassRoofingFilter[idx].ColorD == colorD && cacheEhlersHighPassLowPassRoofingFilter[idx].ColorU == colorU && cacheEhlersHighPassLowPassRoofingFilter[idx].EqualsInput(input))
						return cacheEhlersHighPassLowPassRoofingFilter[idx];
			return CacheIndicator<AUN_Indi.Ehlers.EhlersHighPassLowPassRoofingFilter>(new AUN_Indi.Ehlers.EhlersHighPassLowPassRoofingFilter(){ HPLength = hPLength, SSFLength = sSFLength, ColorD = colorD, ColorU = colorU }, input, ref cacheEhlersHighPassLowPassRoofingFilter);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersHighPassLowPassRoofingFilter EhlersHighPassLowPassRoofingFilter(int hPLength, int sSFLength, Brush colorD, Brush colorU)
		{
			return indicator.EhlersHighPassLowPassRoofingFilter(Input, hPLength, sSFLength, colorD, colorU);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersHighPassLowPassRoofingFilter EhlersHighPassLowPassRoofingFilter(ISeries<double> input , int hPLength, int sSFLength, Brush colorD, Brush colorU)
		{
			return indicator.EhlersHighPassLowPassRoofingFilter(input, hPLength, sSFLength, colorD, colorU);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersHighPassLowPassRoofingFilter EhlersHighPassLowPassRoofingFilter(int hPLength, int sSFLength, Brush colorD, Brush colorU)
		{
			return indicator.EhlersHighPassLowPassRoofingFilter(Input, hPLength, sSFLength, colorD, colorU);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersHighPassLowPassRoofingFilter EhlersHighPassLowPassRoofingFilter(ISeries<double> input , int hPLength, int sSFLength, Brush colorD, Brush colorU)
		{
			return indicator.EhlersHighPassLowPassRoofingFilter(input, hPLength, sSFLength, colorD, colorU);
		}
	}
}

#endregion
