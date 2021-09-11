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
	public class EhlersFisherTransformIndicator : Indicator
	{
		private Series<double> v1;
		private int MINBAR;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"The Fisher Transform Indicator: John Ehlers, Cybernetic Analysis For Stocks And Futures, pg.7-8";
				Name										= "EhlersFisherTransformIndicator";
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
				Length					= 10;
				ColorD					= Brushes.Red;
				ColorU					= Brushes.LimeGreen;
				
				AddPlot(Brushes.ForestGreen, "Fb");
				AddPlot(Brushes.DodgerBlue, "F1");
			}
			else if (State == State.Configure)
			{
				MINBAR = Length + 1;
			}
			else if (State == State.DataLoaded)
			{				
				v1 = new Series<double>(this);
			}
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar <= MINBAR) return;
			
			double max = MAX(Median, Length)[0];
			double min = MIN(Median, Length)[0];
		    if (max != min)
      			v1[0] = (Median[0] - min)/(max - min) - 0.5 + 0.5 * v1[1];
   			else v1[0] = 0;
   			v1[0] = Math.Max(Math.Min(v1[0], 0.999), -0.999);  

			Fb[0] = 0.25 * Math.Log((1 + v1[0]) / (1 - v1[0])) + 0.5 * Fb[1];   
   			F1[0] = Fb[1];		
			
   			if (Fb[0] < F1[0]) PlotBrushes[0][0] = ColorD;
   			else
      			if (Fb[0] > F1[0]) PlotBrushes[0][0] = ColorU; 			
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Length", Order=1, GroupName="Parameters")]
		public int Length
		{ get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> Fb
		{
			get { return Values[0]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> F1
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
		private AUN_Indi.Ehlers.EhlersFisherTransformIndicator[] cacheEhlersFisherTransformIndicator;
		public AUN_Indi.Ehlers.EhlersFisherTransformIndicator EhlersFisherTransformIndicator(int length, Brush colorD, Brush colorU)
		{
			return EhlersFisherTransformIndicator(Input, length, colorD, colorU);
		}

		public AUN_Indi.Ehlers.EhlersFisherTransformIndicator EhlersFisherTransformIndicator(ISeries<double> input, int length, Brush colorD, Brush colorU)
		{
			if (cacheEhlersFisherTransformIndicator != null)
				for (int idx = 0; idx < cacheEhlersFisherTransformIndicator.Length; idx++)
					if (cacheEhlersFisherTransformIndicator[idx] != null && cacheEhlersFisherTransformIndicator[idx].Length == length && cacheEhlersFisherTransformIndicator[idx].ColorD == colorD && cacheEhlersFisherTransformIndicator[idx].ColorU == colorU && cacheEhlersFisherTransformIndicator[idx].EqualsInput(input))
						return cacheEhlersFisherTransformIndicator[idx];
			return CacheIndicator<AUN_Indi.Ehlers.EhlersFisherTransformIndicator>(new AUN_Indi.Ehlers.EhlersFisherTransformIndicator(){ Length = length, ColorD = colorD, ColorU = colorU }, input, ref cacheEhlersFisherTransformIndicator);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersFisherTransformIndicator EhlersFisherTransformIndicator(int length, Brush colorD, Brush colorU)
		{
			return indicator.EhlersFisherTransformIndicator(Input, length, colorD, colorU);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersFisherTransformIndicator EhlersFisherTransformIndicator(ISeries<double> input , int length, Brush colorD, Brush colorU)
		{
			return indicator.EhlersFisherTransformIndicator(input, length, colorD, colorU);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.AUN_Indi.Ehlers.EhlersFisherTransformIndicator EhlersFisherTransformIndicator(int length, Brush colorD, Brush colorU)
		{
			return indicator.EhlersFisherTransformIndicator(Input, length, colorD, colorU);
		}

		public Indicators.AUN_Indi.Ehlers.EhlersFisherTransformIndicator EhlersFisherTransformIndicator(ISeries<double> input , int length, Brush colorD, Brush colorU)
		{
			return indicator.EhlersFisherTransformIndicator(input, length, colorD, colorU);
		}
	}
}

#endregion
