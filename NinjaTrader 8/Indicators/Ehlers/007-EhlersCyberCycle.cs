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
	public class EhlersCyberCycle : Indicator
	{
		private Series<double> smooth;
		private int MinBar = 5;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"The Cyber Cycle:\nJohn Ehlers, Cybernetic Analysis For Stocks And Futures, pg.34";
				Name										= "EhlersCyberCycle";
				Calculate									= Calculate.OnBarClose;
				IsOverlay									= false;
				DisplayInDataBox							= true;
				DrawOnPricePanel							= false;
				DrawHorizontalGridLines						= true;
				DrawVerticalGridLines						= true;
				PaintPriceMarkers							= true;
				ScaleJustification							= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				//Disable this property if your indicator requires custom values that cumulate with each new market data event. 
				//See Help Guide for additional information.
				IsSuspendedWhileInactive					= true;
				Alpha					= 0.07;
				CycleD					= Brushes.Red;
				CycleU					= Brushes.LimeGreen;
				AddPlot(Brushes.ForestGreen, "Cycle");
				AddPlot(Brushes.Orange, "Trigger");
			}
			else if (State == State.Configure)
			{
				smooth = new Series<double>(this);
			}
		}
		protected override void OnBarUpdate()
		{
			if (CurrentBar <= MinBar) return;
			smooth[0] = (Median[0] + 2 * Median[1] + 2 * Median[2] + Median[3]) / 6;
			Cycle[0] = Math.Pow(1 - (0.5 * Alpha), 2) * (smooth[0] - 2 * smooth[1] + smooth[2]) + 
                       2 * (1   - Alpha)     * Cycle[1]   - Math.Pow(1 - Alpha, 2) * Cycle[2];
			Trigger[0] = Cycle[1];
   			   if (Cycle[0] < Trigger[0]) PlotBrushes[0][0] = CycleD;
   			   else
      		   	if (Cycle[0] > Trigger[0]) 	   
			   		PlotBrushes[0][0] = CycleU;

		}

		#region Properties
		[NinjaScriptProperty]
		[Range(0.01, double.MaxValue)]
		[Display(Name="Alpha", Description="Alpha", Order=1, GroupName="Parameters")]
		public double Alpha
		{ get; set; }

		[NinjaScriptProperty]
		[XmlIgnore]
		[Display(Name="CycleD", Description="Cycle Dwn", Order=2, GroupName="Parameters")]
		public Brush CycleD
		{ get; set; }

		[Browsable(false)]
		public string CycleDSerializable
		{
			get { return Serialize.BrushToString(CycleD); }
			set { CycleD = Serialize.StringToBrush(value); }
		}			

		[NinjaScriptProperty]
		[XmlIgnore]
		[Display(Name="CycleU", Description="Cycle Up", Order=3, GroupName="Parameters")]
		public Brush CycleU
		{ get; set; }

		[Browsable(false)]
		public string CycleUSerializable
		{
			get { return Serialize.BrushToString(CycleU); }
			set { CycleU = Serialize.StringToBrush(value); }
		}			

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> Cycle
		{
			get { return Values[0]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> Trigger
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
		private AUN_Indi.Ehlers.EhlersCyberCycle[] cacheEhlersCyberCycle;
		public AUN_Indi.Ehlers.EhlersCyberCycle EhlersCyberCycle(double alpha, Brush cycleD, Brush cycleU)
		{
			return EhlersCyberCycle(Input, alpha, cycleD, cycleU);
		}

		public AUN_Indi.Ehlers.EhlersCyberCycle EhlersCyberCycle(ISeries<double> input, double alpha, Brush cycleD, Brush cycleU)
		{
			if (cacheEhlersCyberCycle != null)
				for (int idx = 0; idx < cacheEhlersCyberCycle.Length; idx++)
					if (cacheEhlersCyberCycle[idx] != null && cacheEhlersCyberCycle[idx].Alpha == alpha && cacheEhlersCyberCycle[idx].CycleD == cycleD && cacheEhlersCyberCycle[idx].CycleU == cycleU && cacheEhlersCyberCycle[idx].EqualsInput(input))
						return cacheEhlersCyberCycle[idx];
			return CacheIndicator<AUN_Indi.Ehlers.EhlersCyberCycle>(new AUN_Indi.Ehlers.EhlersCyberCycle(){ Alpha = alpha, CycleD = cycleD, CycleU = cycleU }, input, ref cacheEhlersCyberCycle);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersCyberCycle EhlersCyberCycle(double alpha, Brush cycleD, Brush cycleU)
		{
			return indicator.EhlersCyberCycle(Input, alpha, cycleD, cycleU);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersCyberCycle EhlersCyberCycle(ISeries<double> input , double alpha, Brush cycleD, Brush cycleU)
		{
			return indicator.EhlersCyberCycle(input, alpha, cycleD, cycleU);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersCyberCycle EhlersCyberCycle(double alpha, Brush cycleD, Brush cycleU)
		{
			return indicator.EhlersCyberCycle(Input, alpha, cycleD, cycleU);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersCyberCycle EhlersCyberCycle(ISeries<double> input , double alpha, Brush cycleD, Brush cycleU)
		{
			return indicator.EhlersCyberCycle(input, alpha, cycleD, cycleU);
		}
	}
}

#endregion
