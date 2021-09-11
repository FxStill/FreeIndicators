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
	public class EhlersDiscreteFourierTransform : Indicator
	{
		private double a1, a2;
		private Series<double> smoothHp;
		private Series<double> hp; 
		private double[] Pwr;
        private double[] DB;
		private double ml1;	
		private double pi2;
//		private double pi4;
//		private double sqrt2;		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"The Discrete Fourier Transform Indicator, https://www.mesasoftware.com/papers/FourierTransformForTraders.pdf";
				Name										= "EhlersDiscreteFourierTransform";
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
				AddPlot(Brushes.Orange, "DomCycle");
			}
			else if (State == State.Configure)
			{
   				a1 = (1 - Math.Sin(2 * Math.PI / HpPeriod)) / Math.Cos(2 * Math.PI / HpPeriod);
   				a2 = 0.5 * (1 + a1);	
				Pwr	= new double[MaxPeriod+1];
				DB 	= new double[MaxPeriod+1];		
				smoothHp 	= new Series<double>(this);
				hp       	= new Series<double>(this);			
				
				ml1 = Math.Log(10);	
				pi2 = 2 * Math.PI;
//				pi4 = 4 * Math.PI;
//				sqrt2 = Math.Sqrt(2);				
			}
		}

		protected override void OnBarUpdate()
		{
			   if(CurrentBar < MaxPeriod + 7)  return;
			   hp[0]       =  a2 * (Median[0] - Median[1]) + a1 * hp[1];
   			   smoothHp[0] = (hp[0] + 2 * hp[1] + 3 * hp[2] + 3 * hp[3] + 2 * hp[4] + hp[5]) / 12;
			   double cs, ss;
			   
   			   for (int i = MinPeriod; i < MaxPeriod; i++) {
      				cs = ss = 0.0;
      				for( int n = 0; n < MaxPeriod - 1; n++) {
        				cs = cs + smoothHp[n] * Math.Cos(pi2 * n / i);
        				ss = ss + smoothHp[n] * Math.Sin(pi2 * n / i);
      				}
      				Pwr[i] = Math.Pow(cs, 2) + Math.Pow(ss, 2);
   				}	
			   
   				double MaxPwr = Pwr[8];
   				for (int i = MinPeriod; i < MaxPeriod; i++) {
      				if (Pwr[i] > MaxPwr)  MaxPwr = Pwr[i];
   				}			   
			   Array.Clear(DB, 0, DB.Length);
			   double num = 0.0, denom = 0.0; 	
   			   for(int n = MinPeriod; n < MaxPeriod; n++) {
      				if(MaxPwr != 0 && Pwr[n] != 0)
         				DB[n] = -MedianPeriod * Math.Log(0.01 / (1 - 0.99 * Pwr[n] / MaxPwr)) / ml1;
      				if(DB[n] > DecibelPeriod) 	DB[n] = DecibelPeriod;
				    if(DB[n] <= 3) {
         				num   = num   + n * (DecibelPeriod - DB[n]);
         				denom = denom +     (DecibelPeriod - DB[n]);
      				}
   			   } // for(int n = Minperiod; n <= Maxperiod; n++) 			   
   			   if(denom != 0) DomCycle[0] = num / denom; 
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
		public Series<double> DomCycle
		{
			get { return Values[0]; }
		}
		
		[Browsable(false)] 
		public Series<double> SmoothHp{ 
			get{ return smoothHp; } 
		} 
		[Browsable(false)] 
		public Series<double> Hp{ 
			get{ return hp; } 
		} 	
		#endregion

	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private AUN_Indi.Ehlers.EhlersDiscreteFourierTransform[] cacheEhlersDiscreteFourierTransform;
		public AUN_Indi.Ehlers.EhlersDiscreteFourierTransform EhlersDiscreteFourierTransform(int minPeriod, int maxPeriod, int hpPeriod, int medianPeriod, int decibelPeriod)
		{
			return EhlersDiscreteFourierTransform(Input, minPeriod, maxPeriod, hpPeriod, medianPeriod, decibelPeriod);
		}

		public AUN_Indi.Ehlers.EhlersDiscreteFourierTransform EhlersDiscreteFourierTransform(ISeries<double> input, int minPeriod, int maxPeriod, int hpPeriod, int medianPeriod, int decibelPeriod)
		{
			if (cacheEhlersDiscreteFourierTransform != null)
				for (int idx = 0; idx < cacheEhlersDiscreteFourierTransform.Length; idx++)
					if (cacheEhlersDiscreteFourierTransform[idx] != null && cacheEhlersDiscreteFourierTransform[idx].MinPeriod == minPeriod && cacheEhlersDiscreteFourierTransform[idx].MaxPeriod == maxPeriod && cacheEhlersDiscreteFourierTransform[idx].HpPeriod == hpPeriod && cacheEhlersDiscreteFourierTransform[idx].MedianPeriod == medianPeriod && cacheEhlersDiscreteFourierTransform[idx].DecibelPeriod == decibelPeriod && cacheEhlersDiscreteFourierTransform[idx].EqualsInput(input))
						return cacheEhlersDiscreteFourierTransform[idx];
			return CacheIndicator<AUN_Indi.Ehlers.EhlersDiscreteFourierTransform>(new AUN_Indi.Ehlers.EhlersDiscreteFourierTransform(){ MinPeriod = minPeriod, MaxPeriod = maxPeriod, HpPeriod = hpPeriod, MedianPeriod = medianPeriod, DecibelPeriod = decibelPeriod }, input, ref cacheEhlersDiscreteFourierTransform);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersDiscreteFourierTransform EhlersDiscreteFourierTransform(int minPeriod, int maxPeriod, int hpPeriod, int medianPeriod, int decibelPeriod)
		{
			return indicator.EhlersDiscreteFourierTransform(Input, minPeriod, maxPeriod, hpPeriod, medianPeriod, decibelPeriod);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersDiscreteFourierTransform EhlersDiscreteFourierTransform(ISeries<double> input , int minPeriod, int maxPeriod, int hpPeriod, int medianPeriod, int decibelPeriod)
		{
			return indicator.EhlersDiscreteFourierTransform(input, minPeriod, maxPeriod, hpPeriod, medianPeriod, decibelPeriod);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersDiscreteFourierTransform EhlersDiscreteFourierTransform(int minPeriod, int maxPeriod, int hpPeriod, int medianPeriod, int decibelPeriod)
		{
			return indicator.EhlersDiscreteFourierTransform(Input, minPeriod, maxPeriod, hpPeriod, medianPeriod, decibelPeriod);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersDiscreteFourierTransform EhlersDiscreteFourierTransform(ISeries<double> input , int minPeriod, int maxPeriod, int hpPeriod, int medianPeriod, int decibelPeriod)
		{
			return indicator.EhlersDiscreteFourierTransform(input, minPeriod, maxPeriod, hpPeriod, medianPeriod, decibelPeriod);
		}
	}
}

#endregion
