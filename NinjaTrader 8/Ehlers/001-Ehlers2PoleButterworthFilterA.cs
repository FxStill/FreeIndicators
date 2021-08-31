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
	public class Ehlers2PoleButterworthFilterA : Indicator
	{
		public double a, b, c1, c2, c3;
		private int MinBar;
		private double sqrt2;	
		public enum STAT {
  	 		Cycle,
   			Cybernetic			
		};
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"The 2 Pole Butterworth Filter: John Ehlers, Cybernetic Analysis For Stocks And Futures, pg.192   && Cycle Analytics for Traders Advanced Technical Trading Concepts, pg.32 ";
				Name										= "Ehlers2PoleButterworthFilterA";
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
				Length                                      = 15;
				Status                                      = STAT.Cycle;
			    bColor                                      = Brushes.DodgerBlue;
				sColor                                      = Brushes.Crimson;				
				AddPlot(Brushes.ForestGreen, "Butterworth2Filter");
			}
			else if (State == State.Configure)
			{
				sqrt2 = Math.Sqrt(2);					
   				a = Math.Exp(-sqrt2 * Math.PI / Length);
   				b = 2 * a * Math.Cos(sqrt2 * Math.PI / Length);
   				c2 = b;
   				c3 = - Math.Pow(a, 2);
				
				switch (Status) {
					case STAT.Cybernetic:
   						c1 = (1 - c2 -c3) / 4;	
						break;
					case STAT.Cycle:
						c1 = 1 - c2 -c3;
						break;
				}
				
				MinBar = 5;
			}
		}

		protected override void OnBarUpdate()
		{
		    if (CurrentBar <= MinBar) return;
			switch (Status) {
				case STAT.Cybernetic:
			   		bf[0] = c1 * (Median[0] + 2 * Median[1] + Median[3]) + c2 * bf[1] + c3 * bf[2];
					break;
				case STAT.Cycle:
					bf[0] = c1 * Median[0] + c2 * bf[1] + c3 * bf[2];
					break;
			}			   
   			if (bf[0] < Median[0]) PlotBrushes[0][0] = bColor;
   			else
      			if (bf[0] > Median[0]) 	   
			   		PlotBrushes[0][0] = sColor;
		}

		#region Properties

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> bf
		{
			get { return Values[0]; }
		}
		
		[Range(5, int.MaxValue)]
		[Display(Name="Length", Description="Length / Period", Order=1, GroupName="Indicator")]
		public int Length		{ 	get ; set;	}	
		
		[Display(Name="Status", Description="Status", Order=2, GroupName="Indicator")]
		public STAT Status		{ 	get ; set;	}			
		
        [NinjaScriptProperty]
		[XmlIgnore] 
		[Display(ResourceType = typeof(Custom.Resource), Name="Buy Color", Description="Buy Color", Order=1, GroupName="View")]		
		public  Brush bColor
		{ get; set; }
		[Browsable(false)]
		public string bColorSerialize
		{
			get { return Serialize.BrushToString(bColor); }
			set { bColor = Serialize.StringToBrush(value); }
		}	
		
        [NinjaScriptProperty]
		[XmlIgnore]
		[Display(ResourceType = typeof(Custom.Resource), Name="Sell Color", Description="Sell Color", Order=2, GroupName="View")]		
		public  Brush sColor
		{get; set;}				
		[Browsable(false)]
		public string sColorSerialize
		{
			get { return Serialize.BrushToString(sColor); }
			set { sColor = Serialize.StringToBrush(value); }
		}		
		
		#endregion

	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private AUN_Indi.Ehlers.Ehlers2PoleButterworthFilterA[] cacheEhlers2PoleButterworthFilterA;
		public AUN_Indi.Ehlers.Ehlers2PoleButterworthFilterA Ehlers2PoleButterworthFilterA(Brush bColor, Brush sColor)
		{
			return Ehlers2PoleButterworthFilterA(Input, bColor, sColor);
		}

		public AUN_Indi.Ehlers.Ehlers2PoleButterworthFilterA Ehlers2PoleButterworthFilterA(ISeries<double> input, Brush bColor, Brush sColor)
		{
			if (cacheEhlers2PoleButterworthFilterA != null)
				for (int idx = 0; idx < cacheEhlers2PoleButterworthFilterA.Length; idx++)
					if (cacheEhlers2PoleButterworthFilterA[idx] != null && cacheEhlers2PoleButterworthFilterA[idx].bColor == bColor && cacheEhlers2PoleButterworthFilterA[idx].sColor == sColor && cacheEhlers2PoleButterworthFilterA[idx].EqualsInput(input))
						return cacheEhlers2PoleButterworthFilterA[idx];
			return CacheIndicator<AUN_Indi.Ehlers.Ehlers2PoleButterworthFilterA>(new AUN_Indi.Ehlers.Ehlers2PoleButterworthFilterA(){ bColor = bColor, sColor = sColor }, input, ref cacheEhlers2PoleButterworthFilterA);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.AUN_Indi.Ehlers.Ehlers2PoleButterworthFilterA Ehlers2PoleButterworthFilterA(Brush bColor, Brush sColor)
		{
			return indicator.Ehlers2PoleButterworthFilterA(Input, bColor, sColor);
		}

		public Indicators.AUN_Indi.Ehlers.Ehlers2PoleButterworthFilterA Ehlers2PoleButterworthFilterA(ISeries<double> input , Brush bColor, Brush sColor)
		{
			return indicator.Ehlers2PoleButterworthFilterA(input, bColor, sColor);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.AUN_Indi.Ehlers.Ehlers2PoleButterworthFilterA Ehlers2PoleButterworthFilterA(Brush bColor, Brush sColor)
		{
			return indicator.Ehlers2PoleButterworthFilterA(Input, bColor, sColor);
		}

		public Indicators.AUN_Indi.Ehlers.Ehlers2PoleButterworthFilterA Ehlers2PoleButterworthFilterA(ISeries<double> input , Brush bColor, Brush sColor)
		{
			return indicator.Ehlers2PoleButterworthFilterA(input, bColor, sColor);
		}
	}
}

#endregion
