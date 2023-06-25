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
	public class EhlersSwissArmyKnifeIndicator : Indicator
	{
		private Series<double> v1;
		
		private int MINBAR;
		private double c0 = 1.0, c1 = 0.0, b0 = 1.0, b1 = 0.0, b2 = 0.0, a1 = 0.0, a2 = 0.0;
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"The Swiss Army Knife Indicator: John Ehlers, Stocks & Commodities V. 24:1 (28-31, 50-53)";
				Name										= "EhlersSwissArmyKnifeIndicator";
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
				Length					= 20;
				Delta					= 0.1;
				Type                    = TSMOOTH.Hp;
				ColorD					= Brushes.Red;
				ColorU					= Brushes.LimeGreen;				
				AddPlot(Brushes.DarkBlue, "Saki");
			}
			else if (State == State.Configure)
			{
				MINBAR = Length + 1;
				
   				double alpha = 0.0, beta = 0.0, gamma = 0.0;
			    double twoPiPrd = 2 * Math.PI / Length;
   				double fourPiPrd = 4 * Math.PI * Delta / Length ;		
				
   				switch(Type){
      				case TSMOOTH.Ema: 
         				alpha = (Math.Cos(twoPiPrd) + Math.Sin(twoPiPrd) - 1) / Math.Cos(twoPiPrd);
         				b0 = alpha;
         				a1 = 1 - alpha;      
         				break;
      				case TSMOOTH.Sma:
         				c1 = 1.0 / Length;
         				b0 = 1.0 / Length;
         				a1 = 1;    
         				break; 
      				case TSMOOTH.Gauss: 
         				beta = 2.415 * (1 - Math.Cos(twoPiPrd));
         				alpha = -beta + Math.Sqrt(Math.Pow(beta,2) + (2 * beta));
         				c0 = Math.Pow(alpha,2);
         				a1 = 2.0 * (1.0 - alpha);
         				a2 = -Math.Pow(1.0 - alpha, 2);
         				break; 
      				case TSMOOTH.Butter: 
         				beta = 2.415 * (1.0 - Math.Cos(twoPiPrd));
         				alpha = -beta + Math.Sqrt(Math.Pow(beta,2) + (2 * beta));
         				c0 = Math.Pow(alpha,2) / 4.0;
         				b1 = 2.0;
         				b2 = 1.0;
         				a1 = 2.0 * (1 - alpha);
         				a2 = -Math.Pow(1.0 - alpha, 2);
         				break; 
      				case TSMOOTH.Smooth: 
         				c0 = 0.25;
         				b1 = 2.0;
         				b2 = 1.0;      
         				break; 
      				case TSMOOTH.Hp: 
         				alpha = (Math.Cos(twoPiPrd) + Math.Sin(twoPiPrd) - 1) / Math.Cos(twoPiPrd);
         				c0 = 1.0 - alpha / 2.0;
         				b1 = -1.0;
         				a1 = 1.0 - alpha;
         				break; 
      				case TSMOOTH.Php2: 
         				beta = 2.415 * (1.0 - Math.Cos(twoPiPrd));
         				alpha = -beta + Math.Sqrt(Math.Pow(beta,2) + (2.0 * beta));
         				c0 = Math.Pow(1 - alpha / 2.0, 2);
         				b1 = -2.0;
         				b2 = 1.0;
         				a1 = 2.0 * (1.0 - alpha);
         				a2 = -Math.Pow(1.0 - alpha, 2);
         				break; 
      				case TSMOOTH.Bp: 
         				beta = Math.Cos(twoPiPrd);
         				gamma = 1.0 / Math.Cos(fourPiPrd);
         				alpha = gamma - Math.Sqrt(Math.Pow(gamma,2) - 1.0);
         				c0 = (1 - alpha) / 2;
         				b2 = -1.0;
         				a1 = beta * (1.0 + alpha);
         				a2 = -alpha;      
         				break; 
      				case TSMOOTH.Bs: 
         				beta = Math.Cos(twoPiPrd);
         				gamma = 1.0 / Math.Cos(fourPiPrd);
         				alpha = gamma - Math.Sqrt(Math.Pow(gamma,2) - 1.0);
         				c0 = (1.0 + alpha) / 2.0;
         				b1 = -2.0 * beta;
         				b2 = 1.0;
         				a1 = beta * (1.0 + alpha);
         				a2 = -alpha;      
         				break; 
	   				default:
		   				break;
   				}
				
			}
			else if (State == State.DataLoaded)
			{				
				v1 = new Series<double>(this);
			}
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar <= MINBAR) return;
			
   			Saki[0] = c0 * (b0 * Median[0] + b1 * Median[1] + b2 * Median[2]) + 
			          a1 * Saki[1] + a2 * Saki[2] - c1 * Median[Length];
   
   			if (Type == TSMOOTH.Hp || Type == TSMOOTH.Php2 || Type == TSMOOTH.Bp) {
      			if (Saki[0] < 0) PlotBrushes[0][0] = ColorD;
      			else
         			if (Saki[0] > 0) PlotBrushes[0][0] = ColorU; 
   			} else {
      			if (Median[0] < Saki[0]) PlotBrushes[0][0] = ColorD;
      			else
         			if (Median[0] > Saki[0]) PlotBrushes[0][0] = ColorU;
   			}
			
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Length", Order=1, GroupName="Parameters")]
		public int Length
		{ get; set; }

		[NinjaScriptProperty]
		[Range(0.01, double.MaxValue)]
		[Display(Name="Delta", Order=2, GroupName="Parameters")]
		public double Delta
		{ get; set; }
		
		[NinjaScriptProperty]
		[Display(Name="Indicator Type", Order=2, GroupName="Parameters")]
		public TSMOOTH Type
		{ get; set; }		

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> Saki
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

	public enum TSMOOTH {
						    Ema, 
							Sma, 
							Gauss, 
							Butter, 
							Smooth, 
							Hp, 
							Php2, 
							Bp, 
							Bs
						};

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private AUN_Indi.Ehlers.EhlersSwissArmyKnifeIndicator[] cacheEhlersSwissArmyKnifeIndicator;
		public AUN_Indi.Ehlers.EhlersSwissArmyKnifeIndicator EhlersSwissArmyKnifeIndicator(int length, double delta, TSMOOTH type, Brush colorD, Brush colorU)
		{
			return EhlersSwissArmyKnifeIndicator(Input, length, delta, type, colorD, colorU);
		}

		public AUN_Indi.Ehlers.EhlersSwissArmyKnifeIndicator EhlersSwissArmyKnifeIndicator(ISeries<double> input, int length, double delta, TSMOOTH type, Brush colorD, Brush colorU)
		{
			if (cacheEhlersSwissArmyKnifeIndicator != null)
				for (int idx = 0; idx < cacheEhlersSwissArmyKnifeIndicator.Length; idx++)
					if (cacheEhlersSwissArmyKnifeIndicator[idx] != null && cacheEhlersSwissArmyKnifeIndicator[idx].Length == length && cacheEhlersSwissArmyKnifeIndicator[idx].Delta == delta && cacheEhlersSwissArmyKnifeIndicator[idx].Type == type && cacheEhlersSwissArmyKnifeIndicator[idx].ColorD == colorD && cacheEhlersSwissArmyKnifeIndicator[idx].ColorU == colorU && cacheEhlersSwissArmyKnifeIndicator[idx].EqualsInput(input))
						return cacheEhlersSwissArmyKnifeIndicator[idx];
			return CacheIndicator<AUN_Indi.Ehlers.EhlersSwissArmyKnifeIndicator>(new AUN_Indi.Ehlers.EhlersSwissArmyKnifeIndicator(){ Length = length, Delta = delta, Type = type, ColorD = colorD, ColorU = colorU }, input, ref cacheEhlersSwissArmyKnifeIndicator);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersSwissArmyKnifeIndicator EhlersSwissArmyKnifeIndicator(int length, double delta, TSMOOTH type, Brush colorD, Brush colorU)
		{
			return indicator.EhlersSwissArmyKnifeIndicator(Input, length, delta, type, colorD, colorU);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersSwissArmyKnifeIndicator EhlersSwissArmyKnifeIndicator(ISeries<double> input , int length, double delta, TSMOOTH type, Brush colorD, Brush colorU)
		{
			return indicator.EhlersSwissArmyKnifeIndicator(input, length, delta, type, colorD, colorU);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersSwissArmyKnifeIndicator EhlersSwissArmyKnifeIndicator(int length, double delta, TSMOOTH type, Brush colorD, Brush colorU)
		{
			return indicator.EhlersSwissArmyKnifeIndicator(Input, length, delta, type, colorD, colorU);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersSwissArmyKnifeIndicator EhlersSwissArmyKnifeIndicator(ISeries<double> input , int length, double delta, TSMOOTH type, Brush colorD, Brush colorU)
		{
			return indicator.EhlersSwissArmyKnifeIndicator(input, length, delta, type, colorD, colorU);
		}
	}
}

#endregion
