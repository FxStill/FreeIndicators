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
	public class EhlersSinewaveIndicator : Indicator
	{
		private Series<double> smooth;
		private Series<double> cycle;
		private Series<double> Q1;
		private Series<double> I1;
		private Series<double> deltaPhase;
		private Series<double> InstPeriod;
		private Series<double> per;
		
		private int MINBAR = 7;
		
		private double ha1, ha2, ha22, tpi;
		private int length, sz;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"The Sinewave Indicator:John Ehlers, Cybernetic Analysis For Stocks And Futures., pg.154-155";
				Name										= "EhlersSinewaveIndicator";
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
				Alpha1					= 0.07;
				Length					= 5;
				AddPlot(Brushes.DodgerBlue, "Sine");
				AddPlot(Brushes.Red, "Lsine");
			}
			else if (State == State.Configure)
			{
   				ha1 = Math.Pow(1 - 0.5 * Alpha1, 2);
   				ha2 = 1 - Alpha1;
   				ha22 = Math.Pow(ha2, 2);
   				ha2 *= 2;
   				length = (Length > 7)? 7: Length;
   				sz = 0;		
				tpi = 2 * Math.PI;
				
			}
			else if (State == State.DataLoaded)
			{				
				smooth = new Series<double>(this);
				cycle = new Series<double>(this);
				Q1 = new Series<double>(this);
				I1 = new Series<double>(this);
				deltaPhase = new Series<double>(this);
				InstPeriod = new Series<double>(this);
				per = new Series<double>(this);
			}
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar <= MINBAR) return;
			
   			smooth[0] = (Median[0] + 2 * Median[1] + 2 * Median[2] + Median[3]) / 6;
   			cycle[0]  =  ha1 * (smooth[0] - 2 * smooth[1] + smooth[2]) + 
            	         ha2 * cycle[1] - ha22 * cycle[2];
   			Q1[0]     = (0.0962 * cycle[0] + 0.5769 * cycle[2] - 0.5769 * cycle[4] -
            	         0.0962 * cycle[6]) * (0.5 + 0.08 * InstPeriod[1] );  
   			I1[0]     = cycle[3];
   			if (Q1[0] != 0 && Q1[1] != 0) {
      			deltaPhase[0] = (I1[0] / Q1[0] - I1[1] / Q1[1]) / 
                		        (1 + I1[0] * I1[1] / (Q1[0] * Q1[1])  );
   			} else deltaPhase[0] = 0;
			
   			if (deltaPhase[0] < 0.1) deltaPhase[0] = 0.1;
   			if (deltaPhase[0] > 1.1) deltaPhase[0] = 1.1;     
   			double medianDelta = GetMedian(deltaPhase, length);
   			double dc = (medianDelta != 0)? 6.28318 / medianDelta + 0.5: 15;
   			InstPeriod[0] = 0.33 * dc + 0.67 * InstPeriod[1];
   			per[0] = 0.15 * InstPeriod[0] + 0.85 * per[1];
   			int dcPeriod = (int)Math.Ceiling(per[0]);
   			double real = 0.0, imag = 0.0;
			for(int pos = 0;  pos < dcPeriod; pos++) { 
      			real += Math.Sin(tpi * pos / dcPeriod) * cycle[pos];
      			imag += Math.Cos(tpi * pos / dcPeriod) * cycle[pos];
   			}
   			double dcPhase = (Math.Abs(imag) > 0.001)? Math.Atan(real / imag) * 180 / Math.PI: 90 * Math.Sign(real);
   			dcPhase += 90;
   			dcPhase = (imag < 0)? dcPhase + 180: dcPhase;
   			dcPhase = (dcPhase > 315)? dcPhase - 360: dcPhase;
			
   			Sine[0]  = Math.Sin( dcPhase * Math.PI / 180);
   			Lsine[0] = Math.Sin((dcPhase + 45) * Math.PI / 180);    
			
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(0.01, double.MaxValue)]
		[Display(Name="Alpha1", Order=1, GroupName="Parameters")]
		public double Alpha1
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Length", Order=2, GroupName="Parameters")]
		public int Length
		{ get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> Sine
		{
			get { return Values[0]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> Lsine
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
		private AUN_Indi.Ehlers.EhlersSinewaveIndicator[] cacheEhlersSinewaveIndicator;
		public AUN_Indi.Ehlers.EhlersSinewaveIndicator EhlersSinewaveIndicator(double alpha1, int length)
		{
			return EhlersSinewaveIndicator(Input, alpha1, length);
		}

		public AUN_Indi.Ehlers.EhlersSinewaveIndicator EhlersSinewaveIndicator(ISeries<double> input, double alpha1, int length)
		{
			if (cacheEhlersSinewaveIndicator != null)
				for (int idx = 0; idx < cacheEhlersSinewaveIndicator.Length; idx++)
					if (cacheEhlersSinewaveIndicator[idx] != null && cacheEhlersSinewaveIndicator[idx].Alpha1 == alpha1 && cacheEhlersSinewaveIndicator[idx].Length == length && cacheEhlersSinewaveIndicator[idx].EqualsInput(input))
						return cacheEhlersSinewaveIndicator[idx];
			return CacheIndicator<AUN_Indi.Ehlers.EhlersSinewaveIndicator>(new AUN_Indi.Ehlers.EhlersSinewaveIndicator(){ Alpha1 = alpha1, Length = length }, input, ref cacheEhlersSinewaveIndicator);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersSinewaveIndicator EhlersSinewaveIndicator(double alpha1, int length)
		{
			return indicator.EhlersSinewaveIndicator(Input, alpha1, length);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersSinewaveIndicator EhlersSinewaveIndicator(ISeries<double> input , double alpha1, int length)
		{
			return indicator.EhlersSinewaveIndicator(input, alpha1, length);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersSinewaveIndicator EhlersSinewaveIndicator(double alpha1, int length)
		{
			return indicator.EhlersSinewaveIndicator(Input, alpha1, length);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersSinewaveIndicator EhlersSinewaveIndicator(ISeries<double> input , double alpha1, int length)
		{
			return indicator.EhlersSinewaveIndicator(input, alpha1, length);
		}
	}
}

#endregion
