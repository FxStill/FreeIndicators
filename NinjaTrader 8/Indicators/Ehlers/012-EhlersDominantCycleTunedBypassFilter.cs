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
	public class EhlersDominantCycleTunedBypassFilter : Indicator
	{
		public static double GetMedian(double[] sourceNumbers, int len) {
	           if (sourceNumbers == null || sourceNumbers.Length == 0)
    	          throw new System.Exception("Median of empty array not defined.");
			   if (sourceNumbers.Length < len)	len = sourceNumbers.Length;
			   double[] sortedPNumbers = new double[len + 1];
			   Array.Copy(sourceNumbers, 0, sortedPNumbers, 0, len);
	   	       Array.Sort(sortedPNumbers);
    	       int mid = len / 2;
        	   double median = (len % 2 != 0) ? (double)sortedPNumbers[mid] : ((double)sortedPNumbers[mid] + (double)sortedPNumbers[mid - 1]) / 2;
	           return median;
       	}		
		
		private Series<double> smoothHp, hp, dc;
		private double[] Q, I, Real, Imag, Ampl, OldQ, OldI, OlderQ, OlderI, OldReal, OldImag, OlderReal, OlderImag, OldAmpl, DB;
		private int MINBAR;
		private double a1, a2, l10;
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"The Dominant Cycle Tuned Bypass Filter: John Ehlers, Stocks & Commodities V. 26:3 (16-22)";
				Name										= "EhlersDominantCycleTunedBypassFilter";
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
				AddPlot(Brushes.DeepSkyBlue, "V2");
			}
			else if (State == State.Configure)
			{
				MINBAR = MaxPeriod + 1;
				smoothHp = new Series<double>(this);
				hp = new Series<double>(this);
				dc = new Series<double>(this);
				Q = new double[MaxPeriod + 1];
				I = new double[MaxPeriod + 1];
				Real = new double[MaxPeriod + 1];
				Imag = new double[MaxPeriod + 1];
				Ampl = new double[MaxPeriod + 1]; 
				OldQ = new double[MaxPeriod + 1]; 
				OldI = new double[MaxPeriod + 1]; 
				OlderQ = new double[MaxPeriod + 1]; 
				OlderI = new double[MaxPeriod + 1]; 
				OldReal = new double[MaxPeriod + 1]; 
				OldImag = new double[MaxPeriod + 1]; 
				OlderReal = new double[MaxPeriod + 1]; 
				OlderImag = new double[MaxPeriod + 1]; 
				OldAmpl = new double[MaxPeriod + 1]; 
				DB = new double[MaxPeriod + 1];
   				a1 = (1 - Math.Sin(2 * Math.PI / HpPeriod)) / Math.Cos(2 * Math.PI / HpPeriod);
   				a2 = 0.5 * (1 + a1);
				l10 = Math.Log(10);
			}
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar <= MINBAR) return;
			
   			hp[0] =  a2 * (Median[0] - Median[1]) + a1 * hp[1];
   			smoothHp[0] = (hp[0] + 2 * hp[1] + 3 * hp[2] + 3 * hp[3] + 2 * hp[4] + hp[5]) / 12;
   			double delta = -0.015 * CurrentBar + 0.5;
   			if (delta < 0.15) delta =  0.15;			
			
   			double num = 0.0, denom = 0.0; 
   			double maxAmpl = 0.0;
   			double s1 = smoothHp[0] - smoothHp[1];
   			double beta, gamma, alpha;		
			
   			for (int n = MinPeriod; n <= MaxPeriod; n++) {
      			beta = Math.Cos(2 * Math.PI / n);
      			gamma = 1 / Math.Cos(4 * Math.PI * delta / n);
      			alpha = gamma - Math.Sqrt(Math.Pow(gamma, 2) - 1);
      			Q[n] = (n / 6.283185) * s1;
      			I[n] = smoothHp[0];
      			Real[n] = 0.5 * (1 - alpha) * (I[n] - OlderI[n]) + beta * (1+ alpha) * OldReal[n] - alpha * OlderReal[n];
      			Imag[n] = 0.5 * (1 - alpha)  * (Q[n] - OlderQ[n]) + beta * (1 + alpha) * OldImag[n] - alpha * OlderImag[n];
      			Ampl[n] = Math.Pow(Real[n], 2) + Math.Pow(Imag[n], 2);   
   			}//for (int n = minperiod; n <= maxperiod; n++)  
			
		    double   MaxAmpl = Ampl[MedianPeriod];
   			for (int n = MinPeriod; n <= MaxPeriod; n++) {
			      OlderI[n] = OldI[n];
			      OldI[n] = I[n];
			      OlderQ[n] = OldQ[n];
			      OldQ[n] = Q[n];
			      OlderReal[n] = OldReal[n];
      			  OldReal[n] = Real[n];
      			  OlderImag[n] = OldImag[n];
			      OldImag[n] = Imag[n];
      			  OldAmpl[n] = Ampl[n];
			      if(Ampl[n] > MaxAmpl) MaxAmpl = Ampl[n];
   			}			
			
   			for(int n = MinPeriod; n <= MaxPeriod; n++) {
			      if(MaxAmpl != 0 && (Ampl[n] / MaxAmpl) > 0)
			         DB[n] = -MedianPeriod * Math.Log(0.01 / (1 - 0.99 * Ampl[n] / MaxAmpl)) / l10;
			      if(DB[n] > DecibelPeriod) DB[n] = DecibelPeriod;
      
			      if(DB[n] <= 3) {
			         num   = num   + n * (DecibelPeriod - DB[n]);
			         denom = denom +     (DecibelPeriod - DB[n]);
			      }       
			} 	
			
   			if(denom != 0) dc[0] = num / denom;  
			double domCyc = GetMedian(dc, MedianPeriod);
   			if (domCyc < MinPeriod) domCyc = DecibelPeriod;
		    beta = Math.Cos(2 * Math.PI / domCyc);
			gamma = 1 / Math.Cos(4 * Math.PI * delta / domCyc);
		    alpha = gamma - Math.Sqrt(Math.Pow(gamma, 2) - 1);			
			
   			V1[0] = 0.5 * (1 - alpha) * (smoothHp[0] - smoothHp[1]) + beta * (1 + alpha) * V1[1] - alpha * V1[2];
   			V2[0] = (domCyc / 6.28) * (V1[0] - V1[1]);    
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
		private AUN_Indi.Ehlers.EhlersDominantCycleTunedBypassFilter[] cacheEhlersDominantCycleTunedBypassFilter;
		public AUN_Indi.Ehlers.EhlersDominantCycleTunedBypassFilter EhlersDominantCycleTunedBypassFilter(int minPeriod, int maxPeriod, int hpPeriod, int medianPeriod, int decibelPeriod)
		{
			return EhlersDominantCycleTunedBypassFilter(Input, minPeriod, maxPeriod, hpPeriod, medianPeriod, decibelPeriod);
		}

		public AUN_Indi.Ehlers.EhlersDominantCycleTunedBypassFilter EhlersDominantCycleTunedBypassFilter(ISeries<double> input, int minPeriod, int maxPeriod, int hpPeriod, int medianPeriod, int decibelPeriod)
		{
			if (cacheEhlersDominantCycleTunedBypassFilter != null)
				for (int idx = 0; idx < cacheEhlersDominantCycleTunedBypassFilter.Length; idx++)
					if (cacheEhlersDominantCycleTunedBypassFilter[idx] != null && cacheEhlersDominantCycleTunedBypassFilter[idx].MinPeriod == minPeriod && cacheEhlersDominantCycleTunedBypassFilter[idx].MaxPeriod == maxPeriod && cacheEhlersDominantCycleTunedBypassFilter[idx].HpPeriod == hpPeriod && cacheEhlersDominantCycleTunedBypassFilter[idx].MedianPeriod == medianPeriod && cacheEhlersDominantCycleTunedBypassFilter[idx].DecibelPeriod == decibelPeriod && cacheEhlersDominantCycleTunedBypassFilter[idx].EqualsInput(input))
						return cacheEhlersDominantCycleTunedBypassFilter[idx];
			return CacheIndicator<AUN_Indi.Ehlers.EhlersDominantCycleTunedBypassFilter>(new AUN_Indi.Ehlers.EhlersDominantCycleTunedBypassFilter(){ MinPeriod = minPeriod, MaxPeriod = maxPeriod, HpPeriod = hpPeriod, MedianPeriod = medianPeriod, DecibelPeriod = decibelPeriod }, input, ref cacheEhlersDominantCycleTunedBypassFilter);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersDominantCycleTunedBypassFilter EhlersDominantCycleTunedBypassFilter(int minPeriod, int maxPeriod, int hpPeriod, int medianPeriod, int decibelPeriod)
		{
			return indicator.EhlersDominantCycleTunedBypassFilter(Input, minPeriod, maxPeriod, hpPeriod, medianPeriod, decibelPeriod);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersDominantCycleTunedBypassFilter EhlersDominantCycleTunedBypassFilter(ISeries<double> input , int minPeriod, int maxPeriod, int hpPeriod, int medianPeriod, int decibelPeriod)
		{
			return indicator.EhlersDominantCycleTunedBypassFilter(input, minPeriod, maxPeriod, hpPeriod, medianPeriod, decibelPeriod);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersDominantCycleTunedBypassFilter EhlersDominantCycleTunedBypassFilter(int minPeriod, int maxPeriod, int hpPeriod, int medianPeriod, int decibelPeriod)
		{
			return indicator.EhlersDominantCycleTunedBypassFilter(Input, minPeriod, maxPeriod, hpPeriod, medianPeriod, decibelPeriod);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersDominantCycleTunedBypassFilter EhlersDominantCycleTunedBypassFilter(ISeries<double> input , int minPeriod, int maxPeriod, int hpPeriod, int medianPeriod, int decibelPeriod)
		{
			return indicator.EhlersDominantCycleTunedBypassFilter(input, minPeriod, maxPeriod, hpPeriod, medianPeriod, decibelPeriod);
		}
	}
}

#endregion
