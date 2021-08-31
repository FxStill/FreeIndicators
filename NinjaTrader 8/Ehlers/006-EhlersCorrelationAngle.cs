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
	public class EhlersCorrelationAngle : Indicator
	{
		private Series<double> price;
		private int MinBar;
		private double pi2, pi05, pi32, pi9;
// pi/2  = 90
// pi    = 180
// 3pi/2 = 270		
// 2pi   = 360		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"The Correlation Cycle Indicator: John Ehlers, Stocks & Commodities V. 38:06 (8–15) ";
				Name										= "EhlersCorrelationAngle";
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
				Period					                    = 20;
				InputPeriod					                = 0;
				ViewAngle                                   = false;
				ViewState                                   = false;
				AddPlot(Brushes.ForestGreen, "Real");
				AddPlot(Brushes.Crimson, "Image");
				AddPlot(Brushes.DodgerBlue, "PriceChart");
				AddPlot(Brushes.DarkOrchid, "Angle");
				AddPlot(Brushes.Orange, "MarketState");

			}
			else if (State == State.Configure)
			{
				pi2  = Math.PI * 2;
				pi05 = Math.PI / 2;
				pi32 = Math.PI * 3 / 2;
				pi9  = 9 * Math.PI / 180;
				MinBar = Period + 1;
				price = new Series<double>(this);
				if (!ViewAngle && ViewState) {
					Log("To display \"ViewState\" enable \"ViewAngle\"", LogLevel.Warning);
					ViewState = false;
				}
			}
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar <= MinBar) return;
			
   			double sx = 0.0, sy = 0.0, sxx = 0.0, syy = 0.0, sxy = 0.0, nsy = 0.0, nsyy = 0.0, nsxy = 0.0;
   			double x, y, ny;
			
			if (InputPeriod == 0)   {
				price[0] = Close[0];
			} else {
				price[0] = Math.Sin(6.28 * CurrentBar / InputPeriod);
				PriceChart[0] = price[0];				
			}
			
			for(int i = 1; i <= Period; i++) {
      			x = price[i - 1];
      			y =   Math.Cos(pi2 * (i - 1) / Period);
      			ny = -Math.Sin(pi2 * (i - 1) / Period);
			    sx   = sx + x;
      			sy   = sy + y;
      			nsy  = nsy + ny;
      			sxx  = sxx + Math.Pow(x, 2);
      			syy  = syy + Math.Pow(y, 2);
      			nsyy = nsyy + Math.Pow(ny, 2);
      			sxy  = sxy + x * y;
			    nsxy = nsxy + x * ny;
   			}// for(int i = 1; i < length; i++)
   			if ( (Period * sxx - Math.Pow(sx, 2) > 0) && (Period * syy - Math.Pow(sy, 2) > 0) )
      			Real[0] = (Period * sxy - sx * sy) / 
				              Math.Sqrt((Period * sxx - Math.Pow(sx, 2)) * (Period * syy - sx * sy ));
   
   			if ( (Period * sxx - Math.Pow(sx, 2) > 0) && (Period * nsyy - Math.Pow(nsy, 2) > 0) )
      			Imag[0] = (Period * nsxy - sx * nsy) / 
				              Math.Sqrt((Period * sxx - Math.Pow(sx, 2)) * (Period * nsyy - Math.Pow(nsy, 2)));
			if (ViewAngle) {
				if (Imag[0] != 0.0)  
					Angle[0] = pi05 + Math.Atan(Real[0] / Imag[0]);
				if (Imag[0] > 0)  
					Angle[0] = Angle[0] - Math.PI;
				if (Angle[1] - Angle[0] < pi32 && Angle[0] < Angle[1]) 
					Angle[0] = Angle[1];
				if (ViewState) {
					MarketState[0] = 0;
					if(Math.Abs(Angle[0] - Angle[1]) < pi9){
						 MarketState[0] = (Angle[0] < 0)? -1: 1;
					}// if(Math.Abs(Angle[0] - Angle[1]) < pi9)
				}// if (ViewState)
			}// if (ViewAngle)
			
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Period", Order=1, GroupName="Parameters")]
		public int Period
		{ get; set; }

		[NinjaScriptProperty]
		[Range(0, int.MaxValue)]
		[Display(Name="InputPeriod", Description="Uses price data if InputPeriod is set to 0", Order=2, GroupName="Parameters")]
		public int InputPeriod
		{ get; set; }
		
		[NinjaScriptProperty]
		[Display(Name="View Phasor", Description="The phasor display provides an early indication of trend onset and trend termination.", Order=3, GroupName="Parameters")]
		public bool ViewAngle
		{ get; set; }		
		
		[NinjaScriptProperty]
		[Display(Name="View Market State", Description="Is the market in a trend mode or a cycle mode?", Order=4, GroupName="Parameters")]
		public bool ViewState
		{ get; set; }			

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> Real
		{
			get { return Values[0]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> Imag
		{
			get { return Values[1]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> PriceChart
		{
			get { return Values[2]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> Angle
		{
			get { return Values[3]; }
		}
		[Browsable(false)]
		[XmlIgnore]
		public Series<double> MarketState
		{
			get { return Values[4]; }
		}		
		#endregion

	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private AUN_Indi.Ehlers.EhlersCorrelationAngle[] cacheEhlersCorrelationAngle;
		public AUN_Indi.Ehlers.EhlersCorrelationAngle EhlersCorrelationAngle(int period, int inputPeriod, bool viewAngle, bool viewState)
		{
			return EhlersCorrelationAngle(Input, period, inputPeriod, viewAngle, viewState);
		}

		public AUN_Indi.Ehlers.EhlersCorrelationAngle EhlersCorrelationAngle(ISeries<double> input, int period, int inputPeriod, bool viewAngle, bool viewState)
		{
			if (cacheEhlersCorrelationAngle != null)
				for (int idx = 0; idx < cacheEhlersCorrelationAngle.Length; idx++)
					if (cacheEhlersCorrelationAngle[idx] != null && cacheEhlersCorrelationAngle[idx].Period == period && cacheEhlersCorrelationAngle[idx].InputPeriod == inputPeriod && cacheEhlersCorrelationAngle[idx].ViewAngle == viewAngle && cacheEhlersCorrelationAngle[idx].ViewState == viewState && cacheEhlersCorrelationAngle[idx].EqualsInput(input))
						return cacheEhlersCorrelationAngle[idx];
			return CacheIndicator<AUN_Indi.Ehlers.EhlersCorrelationAngle>(new AUN_Indi.Ehlers.EhlersCorrelationAngle(){ Period = period, InputPeriod = inputPeriod, ViewAngle = viewAngle, ViewState = viewState }, input, ref cacheEhlersCorrelationAngle);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersCorrelationAngle EhlersCorrelationAngle(int period, int inputPeriod, bool viewAngle, bool viewState)
		{
			return indicator.EhlersCorrelationAngle(Input, period, inputPeriod, viewAngle, viewState);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersCorrelationAngle EhlersCorrelationAngle(ISeries<double> input , int period, int inputPeriod, bool viewAngle, bool viewState)
		{
			return indicator.EhlersCorrelationAngle(input, period, inputPeriod, viewAngle, viewState);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersCorrelationAngle EhlersCorrelationAngle(int period, int inputPeriod, bool viewAngle, bool viewState)
		{
			return indicator.EhlersCorrelationAngle(Input, period, inputPeriod, viewAngle, viewState);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersCorrelationAngle EhlersCorrelationAngle(ISeries<double> input , int period, int inputPeriod, bool viewAngle, bool viewState)
		{
			return indicator.EhlersCorrelationAngle(input, period, inputPeriod, viewAngle, viewState);
		}
	}
}

#endregion
