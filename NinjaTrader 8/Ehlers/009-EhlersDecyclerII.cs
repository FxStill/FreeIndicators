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
	public class EhlersDecyclerII : Indicator
	{
		private Series<double> hp;
		private double a1, a2, a3;
		private double wu, wd;
		private int MINBAR = 5;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Stocks & Commodities V. 33:09 (12–15)";
				Name										= "EhlersDecyclerII";
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
				HPPeriod					                = 125;
				Wd                                          = 0.2;
				ColorD					                    = Brushes.Red;
				ColorU					                    = Brushes.LimeGreen;				
				AddPlot(Brushes.SlateGray, "Decycle");
				AddPlot(Brushes.Orange,    "DecycleOffsetUp");
				AddPlot(Brushes.Orange,    "DecycleOffsetDown");
			}
			else if (State == State.Configure)
			{
			}
			else if (State == State.DataLoaded)
			{				
				hp = new Series<double>(this);
				double alpha = (Math.Cos(((.707 * 360 / HPPeriod) * Math.PI) / 180) + 
								 Math.Sin(((.707 * 360 / HPPeriod) * Math.PI) / 180) - 1) / 
								 Math.Cos(((.707 * 360 / HPPeriod) * Math.PI) / 180);
				a1 =  1 - alpha;
				a2 =  1 - alpha / 2;
				a2 *= a2;
				a3 =  a1 * a1;
				wu = 1 + Wd / 200;
				wd = 1 - Wd / 200;

			}
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar <= MINBAR) {
				hp[0] = Close[0];
				return;
			}		
			
			hp[0] = a2 * (Close[0] - 2 * Close[1] + Close[2]) + 2 * a1 * hp[1] - a3 * hp[2];
			
			Decycle[0]           = Close[0] - hp[0];
			DecycleOffsetUp[0]   = wu       * Decycle[0];
			DecycleOffsetDown[0] = wd       * Decycle[0];
			
   			if (DecycleOffsetUp[0] < Close[0]) PlotBrushes[0][0] = ColorU; 
   			else
      			if (DecycleOffsetDown[0] > Close[0]) PlotBrushes[0][0] = ColorD; 			


		}

		#region Properties
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="HPPeriod", Description="High-pass filter period", Order=1, GroupName="Parameters")]
		public int HPPeriod
		{ get; set; }
		
		[NinjaScriptProperty]
		[Display(Name="Channel width", Description="Channel width in percent", Order=2, GroupName="Parameters")]
		public double Wd
		{ get; set; }		
		
		[NinjaScriptProperty]
		[XmlIgnore]
		[Display(Name="ColorD", Description="Color for bear", Order=3, GroupName="Parameters")]
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
		[Display(Name="ColorU", Description="Color for bool", Order=4, GroupName="Parameters")]
		public Brush ColorU
		{ get; set; }

		[Browsable(false)]
		public string ColorUSerializable
		{
			get { return Serialize.BrushToString(ColorU); }
			set { ColorU = Serialize.StringToBrush(value); }
		}			
		

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> Decycle
		{
			get { return Values[0]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> DecycleOffsetUp
		{
			get { return Values[1]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> DecycleOffsetDown
		{
			get { return Values[2]; }
		}
		#endregion

	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private AUN_Indi.Ehlers.EhlersDecyclerII[] cacheEhlersDecyclerII;
		public AUN_Indi.Ehlers.EhlersDecyclerII EhlersDecyclerII(int hPPeriod, double wd, Brush colorD, Brush colorU)
		{
			return EhlersDecyclerII(Input, hPPeriod, wd, colorD, colorU);
		}

		public AUN_Indi.Ehlers.EhlersDecyclerII EhlersDecyclerII(ISeries<double> input, int hPPeriod, double wd, Brush colorD, Brush colorU)
		{
			if (cacheEhlersDecyclerII != null)
				for (int idx = 0; idx < cacheEhlersDecyclerII.Length; idx++)
					if (cacheEhlersDecyclerII[idx] != null && cacheEhlersDecyclerII[idx].HPPeriod == hPPeriod && cacheEhlersDecyclerII[idx].Wd == wd && cacheEhlersDecyclerII[idx].ColorD == colorD && cacheEhlersDecyclerII[idx].ColorU == colorU && cacheEhlersDecyclerII[idx].EqualsInput(input))
						return cacheEhlersDecyclerII[idx];
			return CacheIndicator<AUN_Indi.Ehlers.EhlersDecyclerII>(new AUN_Indi.Ehlers.EhlersDecyclerII(){ HPPeriod = hPPeriod, Wd = wd, ColorD = colorD, ColorU = colorU }, input, ref cacheEhlersDecyclerII);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersDecyclerII EhlersDecyclerII(int hPPeriod, double wd, Brush colorD, Brush colorU)
		{
			return indicator.EhlersDecyclerII(Input, hPPeriod, wd, colorD, colorU);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersDecyclerII EhlersDecyclerII(ISeries<double> input , int hPPeriod, double wd, Brush colorD, Brush colorU)
		{
			return indicator.EhlersDecyclerII(input, hPPeriod, wd, colorD, colorU);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersDecyclerII EhlersDecyclerII(int hPPeriod, double wd, Brush colorD, Brush colorU)
		{
			return indicator.EhlersDecyclerII(Input, hPPeriod, wd, colorD, colorU);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersDecyclerII EhlersDecyclerII(ISeries<double> input , int hPPeriod, double wd, Brush colorD, Brush colorU)
		{
			return indicator.EhlersDecyclerII(input, hPPeriod, wd, colorD, colorU);
		}
	}
}

#endregion
