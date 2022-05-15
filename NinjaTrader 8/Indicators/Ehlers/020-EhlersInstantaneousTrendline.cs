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
	public class EhlersInstantaneousTrendline : Indicator
	{
		private Series<double> ib;
		private int MINBAR = 5;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"The Instantaneous Trendline: John Ehlers, Rocket Science For Traders, pg.109-110";
				Name										= "EhlersInstantaneousTrendline";
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
				ShowSmoothPrice					= false;
				ColorD					= Brushes.Red;
				ColorU					= Brushes.LimeGreen;					
				AddPlot(Brushes.DarkBlue, "Trendline");
				AddPlot(Brushes.DodgerBlue, "Smoothprice");
			}
			else if (State == State.Configure)
			{
			}
			else if (State == State.DataLoaded)
			{				
				ib = new Series<double>(this);
			}
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar <= MINBAR) return;
			
			double sp = EhlersHilbertTransform(Input).SmoothPeriod[0];		
   			double dcPeriod = Math.Ceiling(sp + 0.5);
   			if (dcPeriod < MINBAR) dcPeriod = MINBAR;
   			for (int i = 0; i < dcPeriod; i++) {
      			ib[0] += Median[i];
   			}	
			ib[0] = ib[0] / dcPeriod;
			
   			Trendline[0] = (4 * ib[0] + 3 * ib[1] + 2 * ib[2] + ib[3]) / 10; 
   			double sth = (4 * Median[0] + 3 * Median[1] + 2 * Median[2] + Median[3]) / 10;
			
   			if (sth < Trendline[0]) PlotBrushes[0][0] = ColorD; 
   			else
      			if (sth > Trendline[0]) PlotBrushes[0][0] = ColorU;   
   
   			if (ShowSmoothPrice) Smoothprice[0] = sth;   			
			

		}

		#region Properties
		[NinjaScriptProperty]
		[Display(Name="ShowSmoothPrice", Order=1, GroupName="Parameters")]
		public bool ShowSmoothPrice
		{ get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> Trendline
		{
			get { return Values[0]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> Smoothprice
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
		private AUN_Indi.Ehlers.EhlersInstantaneousTrendline[] cacheEhlersInstantaneousTrendline;
		public AUN_Indi.Ehlers.EhlersInstantaneousTrendline EhlersInstantaneousTrendline(bool showSmoothPrice, Brush colorD, Brush colorU)
		{
			return EhlersInstantaneousTrendline(Input, showSmoothPrice, colorD, colorU);
		}

		public AUN_Indi.Ehlers.EhlersInstantaneousTrendline EhlersInstantaneousTrendline(ISeries<double> input, bool showSmoothPrice, Brush colorD, Brush colorU)
		{
			if (cacheEhlersInstantaneousTrendline != null)
				for (int idx = 0; idx < cacheEhlersInstantaneousTrendline.Length; idx++)
					if (cacheEhlersInstantaneousTrendline[idx] != null && cacheEhlersInstantaneousTrendline[idx].ShowSmoothPrice == showSmoothPrice && cacheEhlersInstantaneousTrendline[idx].ColorD == colorD && cacheEhlersInstantaneousTrendline[idx].ColorU == colorU && cacheEhlersInstantaneousTrendline[idx].EqualsInput(input))
						return cacheEhlersInstantaneousTrendline[idx];
			return CacheIndicator<AUN_Indi.Ehlers.EhlersInstantaneousTrendline>(new AUN_Indi.Ehlers.EhlersInstantaneousTrendline(){ ShowSmoothPrice = showSmoothPrice, ColorD = colorD, ColorU = colorU }, input, ref cacheEhlersInstantaneousTrendline);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersInstantaneousTrendline EhlersInstantaneousTrendline(bool showSmoothPrice, Brush colorD, Brush colorU)
		{
			return indicator.EhlersInstantaneousTrendline(Input, showSmoothPrice, colorD, colorU);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersInstantaneousTrendline EhlersInstantaneousTrendline(ISeries<double> input , bool showSmoothPrice, Brush colorD, Brush colorU)
		{
			return indicator.EhlersInstantaneousTrendline(input, showSmoothPrice, colorD, colorU);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersInstantaneousTrendline EhlersInstantaneousTrendline(bool showSmoothPrice, Brush colorD, Brush colorU)
		{
			return indicator.EhlersInstantaneousTrendline(Input, showSmoothPrice, colorD, colorU);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersInstantaneousTrendline EhlersInstantaneousTrendline(ISeries<double> input , bool showSmoothPrice, Brush colorD, Brush colorU)
		{
			return indicator.EhlersInstantaneousTrendline(input, showSmoothPrice, colorD, colorU);
		}
	}
}

#endregion
