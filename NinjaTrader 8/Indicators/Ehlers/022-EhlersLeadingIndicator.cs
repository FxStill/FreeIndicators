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
	public class EhlersLeadingIndicator : Indicator
	{
		private Series<double> lb;
		private int MINBAR = 5;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"The Leading Indicator: John Ehlers, Cybernetic Analysis For Stocks And Futures, pg.235";
				Name										= "EhlersLeadingIndicator";
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
				Alpha1					= 0.25;
				Alpha2					= 0.33;
				ColorD					= Brushes.Red;
				ColorU					= Brushes.LimeGreen;
				
				AddPlot(Brushes.DarkBlue, "NetLead");
				AddPlot(Brushes.DodgerBlue, "Lema");
			}
			else if (State == State.Configure)
			{
			}
			else if (State == State.DataLoaded)
			{				
				lb = new Series<double>(this);
			}
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar <= MINBAR) return;
			
   			lb[0] = 2 * Median[0] + (Alpha1 - 2) * Median[1] + (1 - Alpha1) * lb[1];

   			NetLead[0] = Alpha2 * lb[0] + (1 - Alpha2) * NetLead[1];
   			Lema[0] = 0.5 * Median[0] + 0.5 * Lema[1];		
			
   			if (NetLead[0] < Lema[0]) PlotBrushes[0][0] = ColorD; 
   			else
      			if (NetLead[0] > Lema[0]) PlotBrushes[0][0] = ColorU; 			
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(0.1, double.MaxValue)]
		[Display(Name="Alpha1", Order=1, GroupName="Parameters")]
		public double Alpha1
		{ get; set; }

		[NinjaScriptProperty]
		[Range(0.1, double.MaxValue)]
		[Display(Name="Alpha2", Order=2, GroupName="Parameters")]
		public double Alpha2
		{ get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> NetLead
		{
			get { return Values[0]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> Lema
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
		private AUN_Indi.Ehlers.EhlersLeadingIndicator[] cacheEhlersLeadingIndicator;
		public AUN_Indi.Ehlers.EhlersLeadingIndicator EhlersLeadingIndicator(double alpha1, double alpha2, Brush colorD, Brush colorU)
		{
			return EhlersLeadingIndicator(Input, alpha1, alpha2, colorD, colorU);
		}

		public AUN_Indi.Ehlers.EhlersLeadingIndicator EhlersLeadingIndicator(ISeries<double> input, double alpha1, double alpha2, Brush colorD, Brush colorU)
		{
			if (cacheEhlersLeadingIndicator != null)
				for (int idx = 0; idx < cacheEhlersLeadingIndicator.Length; idx++)
					if (cacheEhlersLeadingIndicator[idx] != null && cacheEhlersLeadingIndicator[idx].Alpha1 == alpha1 && cacheEhlersLeadingIndicator[idx].Alpha2 == alpha2 && cacheEhlersLeadingIndicator[idx].ColorD == colorD && cacheEhlersLeadingIndicator[idx].ColorU == colorU && cacheEhlersLeadingIndicator[idx].EqualsInput(input))
						return cacheEhlersLeadingIndicator[idx];
			return CacheIndicator<AUN_Indi.Ehlers.EhlersLeadingIndicator>(new AUN_Indi.Ehlers.EhlersLeadingIndicator(){ Alpha1 = alpha1, Alpha2 = alpha2, ColorD = colorD, ColorU = colorU }, input, ref cacheEhlersLeadingIndicator);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersLeadingIndicator EhlersLeadingIndicator(double alpha1, double alpha2, Brush colorD, Brush colorU)
		{
			return indicator.EhlersLeadingIndicator(Input, alpha1, alpha2, colorD, colorU);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersLeadingIndicator EhlersLeadingIndicator(ISeries<double> input , double alpha1, double alpha2, Brush colorD, Brush colorU)
		{
			return indicator.EhlersLeadingIndicator(input, alpha1, alpha2, colorD, colorU);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersLeadingIndicator EhlersLeadingIndicator(double alpha1, double alpha2, Brush colorD, Brush colorU)
		{
			return indicator.EhlersLeadingIndicator(Input, alpha1, alpha2, colorD, colorU);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersLeadingIndicator EhlersLeadingIndicator(ISeries<double> input , double alpha1, double alpha2, Brush colorD, Brush colorU)
		{
			return indicator.EhlersLeadingIndicator(input, alpha1, alpha2, colorD, colorU);
		}
	}
}

#endregion
